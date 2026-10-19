"""Check TASK-040 using its real entry and repeated calls on one object and array."""

import hashlib
import json
from pathlib import Path
import subprocess
import sys
import tempfile
from xml.sax.saxutils import escape

sys.stdout.reconfigure(encoding="utf-8")
folder = Path(__file__).resolve().parent
source = folder.parent / "Task040Exercise.cs"
project = folder.parent.parent.parent / "DeviceCommunication.csproj"
entry = project.parent / "Program.cs"
before = json.loads((folder / "review-2026-10-19-before.json").read_text(encoding="utf-8"))
report = {"date": "2026-10-19", "task": "TASK-040", "runs": [],
          "coach_assistance": "Changed only Console.Write to Console.WriteLine inside the existing learner loop.",
          "prior_evidence": "review-2026-10-19-before.json"}
hashes = {str(p): hashlib.sha256(p.read_bytes()).hexdigest() for p in [source, project, entry]}


def run(command, cwd):
    result = subprocess.run(command, cwd=cwd, capture_output=True, timeout=60)
    def decode_output(value):
        try:
            return value.decode("utf-8")
        except UnicodeDecodeError:
            return value.decode("cp936")
    result.stdout = decode_output(result.stdout)
    result.stderr = decode_output(result.stderr)
    report["runs"].append({"command": command, "cwd": str(cwd), "exit_code": result.returncode,
                           "stdout": result.stdout, "stderr": result.stderr})
    assert result.returncode == 0, result.stdout + result.stderr
    return result.stdout.splitlines()


rows = ["0,170,AA", "1,1,01", "2,3,03", "3,0,00", "4,100,64", "5,95,5F"]
run(["dotnet", "build", str(project), "--nologo"], project.parent)
original = run(["dotnet", "run", "--project", str(project), "--no-build"], project.parent)
checks = {"real_entry_six_bytes": original == ["Hello, World!", "TASK-040：只处理接收数组中的有效字节", "8,6"] + rows}

with tempfile.TemporaryDirectory(prefix="task040-review-") as temporary:
    temp = Path(temporary)
    csproj = temp / "Review.csproj"
    csproj.write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings></PropertyGroup><ItemGroup><Compile Include="' + escape(str(source), {'"': '&quot;'}) + '" /></ItemGroup></Project>', encoding="utf-8")
    (temp / "Program.cs").write_text('''using DeviceCommunication.Exercises.Task040;
using System.Reflection;
Task040Exercise exercise = new Task040Exercise();
MethodInfo method = typeof(Task040Exercise).GetMethod("PrintReceivedBytes", BindingFlags.Instance | BindingFlags.NonPublic)!;
byte[] data = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0x64, 0x5F, 0xEE, 0xFF };
byte[] initial = (byte[])data.Clone();
foreach (int count in new int[] { 6, 3, 0, 6 })
{
    Console.WriteLine("BEGIN=" + count);
    method.Invoke(exercise, new object[] { data, count });
    Console.WriteLine("END=" + count);
    Console.WriteLine("ARRAY_UNCHANGED=" + data.SequenceEqual(initial));
}
''', encoding="utf-8")
    run(["dotnet", "build", str(csproj), "--nologo"], temp)
    actual = run(["dotnet", "run", "--project", str(csproj), "--no-build"], temp)
    expected = []
    for count in [6, 3, 0, 6]:
        expected += ["BEGIN=" + str(count), "8," + str(count)] + rows[:count] + ["END=" + str(count), "ARRAY_UNCHANGED=True"]
    checks["same_object_same_array_6_3_0_6"] = actual == expected

after_source = source.read_text(encoding="utf-8-sig")
expected_fix = before["source"].replace('Console.Write(i +', 'Console.WriteLine(i +', 1)
checks["coach_changed_only_line_break"] = after_source == expected_fix
checks["project_and_entry_unchanged"] = all(hashes[str(p)] == before["hashes"][str(p)] for p in [project, entry])
checks["no_further_input_changes"] = all(hashlib.sha256(Path(p).read_bytes()).hexdigest() == value for p, value in hashes.items())
report.update({"checks": checks, "all_passed": all(checks.values()), "source_after": after_source,
               "hashes_before_assistance": before["hashes"], "hashes_after_assistance": hashes})
(folder / "review-2026-10-19-final.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print(json.dumps({"checks": checks, "all_passed": report["all_passed"]}, ensure_ascii=False))
assert report["all_passed"]
