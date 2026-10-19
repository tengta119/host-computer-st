from pathlib import Path
import hashlib
import json
import subprocess
import tempfile
from xml.sax.saxutils import escape

root = Path(__file__).resolve().parents[5]
folder = Path(__file__).resolve().parent
project = root / 'Stage03-DeviceCommunication/DeviceCommunication'
dll = project / 'bin/Debug/net10.0/DeviceCommunication.dll'
destination = folder / 'startup-2026-10-19.json'
if destination.exists():
    raise RuntimeError('Refusing to overwrite evidence')
runs = []
for arguments, input_text in [(['--ports'], ''), ([], '\n'), (['--port', 'COM254'], 'o\ns\nr\nc\nq\n'), (['--simulator', 'COM254'], '')]:
    command = ['dotnet', str(dll)] + arguments
    result = subprocess.run(command, input=input_text.encode('utf-8'), capture_output=True, timeout=8)
    runs.append({'command': command, 'stdin': input_text, 'exit_code': result.returncode,
                 'stdout': result.stdout.decode('utf-8', errors='replace'), 'stderr': result.stderr.decode('utf-8', errors='replace')})
harness = r'''
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using DeviceCommunication.Exercises.Task044;
Task044Exercise exercise = new Task044Exercise();
Type type = typeof(Task044Exercise);
BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
MethodInfo receive = type.GetMethod("HandleReceivedBytes", flags)!;
FieldInfo bufferField = type.GetField("_receivedBytes", flags)!;
FieldInfo countField = type.GetField("_printedFrameCount", flags)!;
List<byte> originalBuffer = (List<byte>)bufferField.GetValue(exercise)!;
TextWriter realOutput = Console.Out;
StringWriter captured = new StringWriter();
Console.SetOut(captured);
List<object> checks = new List<object>();
bool allPassed = true;
void Check(byte[] input, int validCount, byte[] expected, int frames)
{
    byte[] original = (byte[])input.Clone();
    receive.Invoke(exercise, new object[] { input, validCount });
    List<byte> actual = (List<byte>)bufferField.GetValue(exercise)!;
    int actualFrames = (int)countField.GetValue(exercise)!;
    bool passed = input.SequenceEqual(original) && Object.ReferenceEquals(actual, originalBuffer)
        && actual.SequenceEqual(expected) && actualFrames == frames;
    allPassed = allPassed && passed;
    checks.Add(new { passed, validCount, actualFrames, remaining = Convert.ToHexString(actual.ToArray()) });
}
Check(new byte[] { 0x99, 0x88 }, 2, Array.Empty<byte>(), 0);
Check(new byte[] { 0x77, 0xAA, 0x01 }, 3, new byte[] { 0xAA, 0x01 }, 0);
Check(new byte[] { 0x03, 0x00, 0xFA, 0x5F, 0xEE }, 4, Array.Empty<byte>(), 1);
Check(new byte[] { 0xAA, 0x01, 0x03, 0x00, 0xFA, 0x5F, 0x99, 0xAA }, 8, new byte[] { 0xAA }, 2);
Check(new byte[] { 0x02, 0x03, 0x01, 0x04, 0x60 }, 5, Array.Empty<byte>(), 3);
Check(new byte[] { 0xEE }, 0, Array.Empty<byte>(), 3);
Console.SetOut(realOutput);
Console.WriteLine(JsonSerializer.Serialize(new { allPassed, checks, stdout = captured.ToString() }));
return allPassed ? 0 : 1;
'''
with tempfile.TemporaryDirectory(prefix='task044-scaffold-') as temporary:
    temporary_folder = Path(temporary)
    xml = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="DeviceCommunication"><HintPath>' + escape(str(dll)) + '</HintPath></Reference><PackageReference Include="System.IO.Ports" Version="10.0.0" /></ItemGroup></Project>'
    (temporary_folder / 'Check.csproj').write_text(xml, encoding='utf-8')
    (temporary_folder / 'Program.cs').write_text(harness, encoding='utf-8')
    command = ['dotnet', 'run', '--project', str(temporary_folder / 'Check.csproj'), '--verbosity', 'quiet']
    result = subprocess.run(command, capture_output=True, timeout=40)
    output = result.stdout.decode('utf-8', errors='replace')
    json_lines = [line for line in output.splitlines() if line.startswith('{')]
    buffer_checks = json.loads(json_lines[-1]) if json_lines else None
    diagnostic = {'command': command, 'exit_code': result.returncode, 'stdout': output,
                  'stderr': result.stderr.decode('utf-8', errors='replace'), 'harness_source': harness, 'result': buffer_checks}
baseline = json.loads((folder / 'baseline-2026-10-19.json').read_text(encoding='utf-8'))
old_unchanged = all(hashlib.sha256((root / path).read_bytes()).hexdigest() == digest for path, digest in baseline['old_cs_sha256'].items())
checks = {
    'ports_listing_exits_zero': runs[0]['exit_code'] == 0 and 'COM5' in runs[0]['stdout'],
    'empty_port_does_not_open': '\u672c\u6b21\u6ca1\u6709\u6253\u5f00\u4efb\u4f55\u7aef\u53e3' in runs[1]['stdout'],
    'learner_menu_is_honest_placeholder': 'TODO 1' in runs[2]['stdout'] and runs[2]['exit_code'] == 0,
    'simulator_missing_port_reports_failure': '\u6a21\u62df\u8bbe\u5907\u5931\u8d25' in runs[3]['stdout'] and runs[3]['exit_code'] == 0,
    'provided_buffer_six_cases_passed': buffer_checks is not None and buffer_checks['allPassed'] and diagnostic['exit_code'] == 0,
    'old_task_sources_unchanged': old_unchanged,
}
record = {'date': '2026-10-19', 'build': {'command': 'dotnet build .\\Stage03-DeviceCommunication\\DeviceCommunication\\DeviceCommunication.csproj --nologo', 'exit_code': 0, 'warnings': 0, 'errors': 0},
          'runs': runs, 'provided_buffer_diagnostic': diagnostic, 'checks': checks, 'actual_serial_pair_created': False,
          'serial_roundtrip_verified': False, 'learner_todos_completed': False, 'dod_accepted': False}
destination.write_text(json.dumps(record, ensure_ascii=False, indent=2), encoding='utf-8')
print(json.dumps({'checks': checks, 'evidence': str(destination), 'dod_accepted': False}, ensure_ascii=True, indent=2))
assert all(checks.values())
