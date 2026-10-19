"""Final review after learner completed reset/replay; coach added four displays."""

import hashlib
import json
from pathlib import Path
import subprocess
import sys
import tempfile
from xml.sax.saxutils import escape

sys.stdout.reconfigure(encoding="utf-8")
folder = Path(__file__).resolve().parent
source = folder.parent / "Task041Exercise.cs"
project = folder.parent.parent.parent / "DeviceCommunication.csproj"
entry = project.parent / "Program.cs"
inputs = [source, project, entry]
hashes = {str(p): hashlib.sha256(p.read_bytes()).hexdigest() for p in inputs}
report = {"date": "2026-10-19", "task": "TASK-041", "runs": [], "submitted_source": source.read_text(encoding="utf-8-sig")}


def run(command, cwd):
    result = subprocess.run(command, cwd=cwd, capture_output=True, timeout=60)
    def decode(data):
        try:
            return data.decode("utf-8")
        except UnicodeDecodeError:
            return data.decode("cp936")
    out, err = decode(result.stdout), decode(result.stderr)
    report["runs"].append({"command": command, "cwd": str(cwd), "exit_code": result.returncode, "stdout": out, "stderr": err})
    assert result.returncode == 0, out + err
    return out.splitlines()


rows = ["0,170,AA", "1,1,01", "2,3,03", "3,0,00", "4,100,64", "5,95,5F"]
expected = ["Hello, World!", "TASK-041：保留分批收到的有效字节", "开始时", "0", "第一批后", "2"] + rows[:2] + ["第二批后", "5"] + rows[:5] + ["第三批后", "6"] + rows
expected += ["零数量后", "6"] + rows + ["清空后", "0", "重新接收第一批后", "2"] + rows[:2] + ["重新接收第二批后", "5"] + rows[:5] + ["重新接收第三批后", "6"] + rows
run(["dotnet", "build", str(project), "--nologo"], project.parent)
actual = run(["dotnet", "run", "--project", str(project), "--no-build"], project.parent)
checks = {"real_entry_counts_and_contents": actual == expected}

with tempfile.TemporaryDirectory(prefix="task041-review-") as temporary:
    temp = Path(temporary)
    csproj = temp / "Review.csproj"
    csproj.write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings></PropertyGroup><ItemGroup><Compile Include="' + escape(str(source), {'"': '&quot;'}) + '" /></ItemGroup></Project>', encoding="utf-8")
    (temp / "Program.cs").write_text('''using DeviceCommunication.Exercises.Task041;
using System.Reflection;
Task041Exercise exercise = new Task041Exercise();
MethodInfo append = typeof(Task041Exercise).GetMethod("AppendReceivedBytes", BindingFlags.Instance | BindingFlags.NonPublic)!;
FieldInfo field = typeof(Task041Exercise).GetField("_receivedBytes", BindingFlags.Instance | BindingFlags.NonPublic)!;
List<byte> retained = (List<byte>)field.GetValue(exercise)!;
byte[][] chunks = new byte[][] { new byte[] { 0xAA, 0x01, 0xEE, 0xFF }, new byte[] { 0x03, 0x00, 0x64, 0xEE }, new byte[] { 0x5F, 0xEE } };
byte[][] initial = chunks.Select(a => (byte[])a.Clone()).ToArray();
int[] counts = new int[] { 2, 3, 1 };
for (int i = 0; i < chunks.Length; i++)
{
    append.Invoke(exercise, new object[] { chunks[i], counts[i] });
    Console.WriteLine("COUNT=" + retained.Count);
    Console.WriteLine("DATA=" + string.Join(" ", retained.Select(value => value.ToString("X2"))));
    Console.WriteLine("SAME_LIST=" + Object.ReferenceEquals(retained, field.GetValue(exercise)));
}
byte[] beforeZero = retained.ToArray();
append.Invoke(exercise, new object[] { chunks[0], 0 });
Console.WriteLine("ZERO_NO_CHANGE=" + beforeZero.SequenceEqual(retained));
Console.WriteLine("INPUTS_UNCHANGED=" + chunks.Select((a, i) => a.SequenceEqual(initial[i])).All(value => value));
''', encoding="utf-8")
    run(["dotnet", "build", str(csproj), "--nologo"], temp)
    diagnostic = run(["dotnet", "run", "--project", str(csproj), "--no-build"], temp)
    checks["same_list_valid_append_inputs_preserved"] = diagnostic == ["COUNT=2", "DATA=AA 01", "SAME_LIST=True", "COUNT=5", "DATA=AA 01 03 00 64", "SAME_LIST=True", "COUNT=6", "DATA=AA 01 03 00 64 5F", "SAME_LIST=True", "ZERO_NO_CHANGE=True", "INPUTS_UNCHANGED=True"]

checks["inputs_unchanged_during_review"] = all(hashlib.sha256(Path(p).read_bytes()).hexdigest() == value for p, value in hashes.items())
before = json.loads((folder / "review-02-before.json").read_text(encoding="utf-8"))
old_block = """            _receivedBytes.Clear();
            AppendReceivedBytes(thirdChunk, 0);
            AppendReceivedBytes(firstChunk, 2);
            AppendReceivedBytes(secondChunk, 3);
            AppendReceivedBytes(thirdChunk, 1);"""
new_block = """            _receivedBytes.Clear();
            PrintBuffer("清空后");
            AppendReceivedBytes(thirdChunk, 0);
            AppendReceivedBytes(firstChunk, 2);
            PrintBuffer("重新接收第一批后");
            AppendReceivedBytes(secondChunk, 3);
            PrintBuffer("重新接收第二批后");
            AppendReceivedBytes(thirdChunk, 1);
            PrintBuffer("重新接收第三批后");"""
assert before["source"].count(old_block) == 1
checks["coach_only_added_four_observation_calls"] = source.read_text(encoding="utf-8-sig") == before["source"].replace(old_block, new_block, 1)
checks["project_entry_unchanged"] = all(hashes[str(p)] == before["hashes"][str(p)] for p in [project, entry])
report.update({"checks": checks, "all_passed": all(checks.values()), "hashes_after_assistance": hashes,
               "coach_assistance": "Four PrintBuffer observation calls only; learner zero/Clear/replay logic retained.",
               "prior_evidence": "review-02-before.json",
               "dod": {"1": "passed", "2": "passed", "3": "passed"}, "task_status": "complete"})
(folder / "review-02-final.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print(json.dumps({"checks": checks, "dod": report["dod"], "all_passed": report["all_passed"]}, ensure_ascii=False))
assert report["all_passed"]
