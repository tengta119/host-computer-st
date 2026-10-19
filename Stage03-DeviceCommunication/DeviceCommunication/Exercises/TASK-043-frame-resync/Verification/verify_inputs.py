from pathlib import Path
import hashlib
import json
import subprocess
import tempfile
from xml.sax.saxutils import escape


root = Path(__file__).resolve().parents[5]
project = root / 'Stage03-DeviceCommunication/DeviceCommunication'
source = project / 'Exercises/TASK-043-frame-resync/Task043Exercise.cs'
entry = project / 'Program.cs'
dll = project / 'bin/Debug/net10.0/DeviceCommunication.dll'
destination = Path(__file__).parent / 'input-preservation-final-2026-10-19.json'
if destination.exists():
    raise RuntimeError('Refusing to overwrite evidence')
before = {str(p.relative_to(root)): hashlib.sha256(p.read_bytes()).hexdigest() for p in (source, entry, dll)}
harness = r'''
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using DeviceCommunication.Exercises.Task043;

Task043Exercise exercise = new Task043Exercise();
Type type = typeof(Task043Exercise);
BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
MethodInfo receive = type.GetMethod("ReceiveChunk", flags)!;
FieldInfo bufferField = type.GetField("_receivedBytes", flags)!;
FieldInfo countField = type.GetField("_printedFrameCount", flags)!;
List<byte> originalBuffer = (List<byte>)bufferField.GetValue(exercise)!;
TextWriter realOutput = Console.Out;
StringWriter capturedOutput = new StringWriter();
Console.SetOut(capturedOutput);
List<object> checks = new List<object>();
bool allPassed = true;

void Check(byte[] input, int validCount, byte[] expectedBuffer, int expectedFrames, string name)
{
    byte[] originalInput = (byte[])input.Clone();
    receive.Invoke(exercise, new object[] { input, validCount, name });
    List<byte> buffer = (List<byte>)bufferField.GetValue(exercise)!;
    int frameCount = (int)countField.GetValue(exercise)!;
    bool inputUnchanged = input.SequenceEqual(originalInput);
    bool sameList = Object.ReferenceEquals(buffer, originalBuffer);
    bool stateMatches = buffer.SequenceEqual(expectedBuffer) && frameCount == expectedFrames;
    allPassed = allPassed && inputUnchanged && sameList && stateMatches;
    checks.Add(new { name, validCount, inputUnchanged, sameList, stateMatches, frameCount,
        actualBuffer = Convert.ToHexString(buffer.ToArray()), expectedBuffer = Convert.ToHexString(expectedBuffer) });
}

byte[] first = new byte[] { 0x77, 0xAA, 0x01, 0xEE };
Check(first, 3, new byte[] { 0xAA, 0x01 }, 0, "noise and partial A; trailing EE excluded");
Check(new byte[] { 0x03, 0x00, 0x64, 0x5F, 0xEE }, 4, Array.Empty<byte>(), 1, "complete A; trailing EE excluded");
Check(new byte[] { 0xAA, 0x02, 0x03, 0x00, 0x65, 0x60, 0x99, 0xAA, 0x03, 0xAA, 0x00, 0x66, 0x61, 0xEE },
    13, Array.Empty<byte>(), 3, "B then noise then C; trailing EE excluded");
Check(first, 0, Array.Empty<byte>(), 3, "same original array with zero valid bytes");
Check(new byte[] { 0x99, 0x88 }, 2, Array.Empty<byte>(), 3, "noise only");
Check(new byte[] { 0xAA, 0x03, 0xAA, 0x00, 0x66, 0x61 }, 6, Array.Empty<byte>(), 4, "C again after clearing noise");
Console.SetOut(realOutput);
Console.WriteLine(JsonSerializer.Serialize(new { allPassed, checks, capturedOutput = capturedOutput.ToString() }));
return allPassed ? 0 : 1;
'''
with tempfile.TemporaryDirectory(prefix='task043-input-check-') as temporary:
    folder = Path(temporary)
    project_text = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="DeviceCommunication"><HintPath>' + escape(str(dll)) + '</HintPath></Reference></ItemGroup></Project>'
    (folder / 'InputCheck.csproj').write_text(project_text, encoding='utf-8')
    (folder / 'Program.cs').write_text(harness, encoding='utf-8')
    command = ['dotnet', 'run', '--project', str(folder / 'InputCheck.csproj'), '--verbosity', 'quiet']
    result = subprocess.run(command, capture_output=True, timeout=40)
    stdout = result.stdout.decode('utf-8', errors='replace')
    stderr = result.stderr.decode('utf-8', errors='replace')
    lines = [line for line in stdout.splitlines() if line.startswith('{')]
    observed = json.loads(lines[-1]) if lines else None
after = {str(p.relative_to(root)): hashlib.sha256(p.read_bytes()).hexdigest() for p in (source, entry, dll)}
record = {'date': '2026-10-19', 'command': command, 'harness_source': harness, 'project_source': project_text,
          'exit_code': result.returncode, 'stdout': stdout, 'stderr': stderr, 'observed': observed,
          'sha256_before': before, 'sha256_after': after, 'source_entry_and_dll_unchanged': before == after}
destination.write_text(json.dumps(record, ensure_ascii=False, indent=2), encoding='utf-8')
print(json.dumps({'evidence': str(destination), 'exit_code': result.returncode,
                  'all_checks_passed': observed is not None and observed['allPassed'],
                  'input_cases': len(observed['checks']) if observed else 0,
                  'source_entry_and_dll_unchanged': before == after, 'stderr': stderr}, ensure_ascii=True))
