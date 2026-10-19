"""Review TASK-042 frame timing and retention against the task's agreed inputs."""

import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys
import tempfile
from xml.sax.saxutils import escape

sys.stdout.reconfigure(encoding="utf-8")
folder = Path(__file__).resolve().parent
source = folder.parent / "Task042Exercise.cs"
project = folder.parent.parent.parent / "DeviceCommunication.csproj"
entry = project.parent / "Program.cs"
hashes = {str(p): hashlib.sha256(p.read_bytes()).hexdigest() for p in [source, project, entry]}
report = {"date": "2026-10-19", "task": "TASK-042", "source": source.read_text(encoding="utf-8-sig"), "runs": []}


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
    return out


a = "AA 01 03 00 64 5F"
b = "AA 02 03 00 65 60"


def state(frames, remaining_bytes, printed):
    return {"frames": frames, "remaining": len(remaining_bytes.split()), "bytes": remaining_bytes, "printed": printed}


expected = [
    [state([], "AA 01", 0), state([], "AA 01 03 00 64", 0), state([a], "", 1)],
    [state([a, b], "", 2), state([], "", 2)],
    [state([a], "AA 02", 1), state([b], "", 2), state([], "", 2)],
    [state([], "AA 01", 0), state([], "AA 01 03 00 64", 0), state([a], "", 1), state([b], "", 2), state([], "", 2)],
]
run(["dotnet", "build", str(project), "--nologo"], project.parent)
output = run(["dotnet", "run", "--project", str(project), "--no-build"], project.parent)
groups = []
current = None
for line in output.splitlines():
    if line.startswith("场景"):
        groups.append([])
    elif line.startswith("输入："):
        current = {"frames": []}
        groups[-1].append(current)
    elif re.match(r"^完整帧\d+：", line):
        current["frames"].append(line.split("：", 1)[1].strip())
    elif line.startswith("缓冲剩余数量："):
        current["remaining"] = int(line.split("：", 1)[1])
    elif line.startswith("缓冲剩余字节："):
        current["bytes"] = line.split("：", 1)[1].strip()
    elif line.startswith("本组已输出帧数："):
        current["printed"] = int(line.split("：", 1)[1])
assert len(groups) == 4 and [len(g) for g in groups] == [3, 2, 3, 5]

with tempfile.TemporaryDirectory(prefix="task042-review-") as temporary:
    temp = Path(temporary)
    csproj = temp / "Review.csproj"
    csproj.write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings></PropertyGroup><ItemGroup><Compile Include="' + escape(str(source), {'"': '&quot;'}) + '" /></ItemGroup></Project>', encoding="utf-8")
    (temp / "Program.cs").write_text('''using DeviceCommunication.Exercises.Task042;
using System.Reflection;
using System.Text.Json;
Task042Exercise exercise = new Task042Exercise();
Type type = typeof(Task042Exercise);
MethodInfo start = type.GetMethod("StartScenario", BindingFlags.Instance | BindingFlags.NonPublic)!;
MethodInfo receive = type.GetMethod("ReceiveChunk", BindingFlags.Instance | BindingFlags.NonPublic)!;
FieldInfo buffer = type.GetField("_receivedBytes", BindingFlags.Instance | BindingFlags.NonPublic)!;
FieldInfo printed = type.GetField("_printedFrameCount", BindingFlags.Instance | BindingFlags.NonPublic)!;
byte[] input = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0x64, 0x5F, 0xAA, 0x02, 0x03, 0x00, 0x65, 0x60 };
byte[] initial = (byte[])input.Clone();
TextWriter original = Console.Out;
StringWriter captured = new StringWriter();
Console.SetOut(captured);
start.Invoke(exercise, new object[] { "diagnostic" });
object identity = buffer.GetValue(exercise)!;
receive.Invoke(exercise, new object[] { input, 12, "combined" });
int remainingAfter12 = ((List<byte>)buffer.GetValue(exercise)!).Count;
int framesAfter12 = (int)printed.GetValue(exercise)!;
receive.Invoke(exercise, new object[] { input, 0, "zero" });
int remainingAfterZero = ((List<byte>)buffer.GetValue(exercise)!).Count;
int framesAfterZero = (int)printed.GetValue(exercise)!;
Console.SetOut(original);
Console.WriteLine(JsonSerializer.Serialize(new { remainingAfter12, framesAfter12, remainingAfterZero, framesAfterZero,
    inputsUnchanged = input.SequenceEqual(initial), sameList = Object.ReferenceEquals(identity, buffer.GetValue(exercise)), captured = captured.ToString() }));
''', encoding="utf-8")
    run(["dotnet", "build", str(csproj), "--nologo"], temp)
    diagnostic = json.loads(run(["dotnet", "run", "--project", str(csproj), "--no-build"], temp))

checks = {
    "split_A_waits_and_extracts_once": groups[0] == expected[0],
    "combined_AB_drained_immediately": groups[1] == expected[1],
    "tail_prefix_preserved_and_completed": groups[2] == expected[2],
    "split_AB_result_correct": groups[3] == expected[3],
    "zero_not_needed_to_drain_complete_frames": all(not group[-1]["frames"] for group in groups[1:]),
    "final_frame_bytes_once_and_in_order": [frame for chunk in groups[0] for frame in chunk["frames"]] == [a] and all([frame for chunk in group for frame in chunk["frames"]] == [a, b] for group in groups[1:]),
    "input_array_and_list_identity_preserved": diagnostic["inputsUnchanged"] and diagnostic["sameList"],
    "learner_inputs_unchanged": all(hashlib.sha256(Path(p).read_bytes()).hexdigest() == h for p, h in hashes.items()),
}
dod = {"1": checks["split_A_waits_and_extracts_once"],
       "2": checks["combined_AB_drained_immediately"] and checks["tail_prefix_preserved_and_completed"] and checks["input_array_and_list_identity_preserved"],
       "3": groups[1][0]["frames"] == [frame for chunk in groups[3][:4] for frame in chunk["frames"]] and checks["final_frame_bytes_once_and_in_order"]}
report.update({"checks": checks, "dod": dod, "actual_groups": groups, "expected_groups": expected,
               "diagnostic": diagnostic, "hashes_before_and_after": hashes, "all_passed": all(checks.values())})
(folder / "review-2026-10-19.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print(json.dumps({"checks": checks, "dod": dod, "diagnostic_counts": {key: diagnostic[key] for key in ["remainingAfter12", "framesAfter12", "remainingAfterZero", "framesAfterZero"]}}, ensure_ascii=False))
assert checks["learner_inputs_unchanged"]
