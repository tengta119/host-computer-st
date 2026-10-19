"""Run the submitted program and compare an isolated original-sample copy."""

import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile

sys.stdout.reconfigure(encoding="utf-8")
evidence_dir = Path(__file__).resolve().parent
exercise_dir = evidence_dir.parent
project_dir = exercise_dir.parent.parent
source = exercise_dir / "Task039Exercise.cs"
project = project_dir / "DeviceCommunication.csproj"
entry = project_dir / "Program.cs"
inputs = [source, project, entry]
before = {str(p): hashlib.sha256(p.read_bytes()).hexdigest() for p in inputs}
submitted = source.read_bytes()
old = b"0x00, 0x65, 0x5F"
new = b"0x00, 0x64, 0x5F"
assert submitted.count(old) == 1, "Review the changed array initializer before rerunning."
baseline = submitted.replace(old, new, 1)
report = {
    "date": "2026-10-19",
    "task": "TASK-039",
    "baseline_note": "Coach restored only the fifth array initializer in a temporary copy; this is not evidence of a historical learner run.",
    "source_sha256_before": before,
    "submitted_source": submitted.decode("utf-8-sig"),
    "baseline_replacement": {"from": old.decode(), "to": new.decode()},
    "runs": [],
}


def run(command, cwd):
    result = subprocess.run(command, cwd=cwd, capture_output=True, text=True,
                            encoding="utf-8", errors="replace", timeout=60)
    report["runs"].append({"command": command, "cwd": str(cwd),
                           "exit_code": result.returncode,
                           "stdout": result.stdout, "stderr": result.stderr})
    assert result.returncode == 0, result.stdout + result.stderr
    return result.stdout.splitlines()


header = ["Hello, World!", "TASK-039：认识字节与十六进制数据", "100", "64", "6"]
rows_original = ["0: 170,0xAA", "1: 1,0x01", "2: 3,0x03", "3: 0,0x00", "4: 100,0x64", "5: 95,0x5F"]
rows_changed = rows_original.copy()
rows_changed[4] = "4: 101,0x65"

run(["dotnet", "build", str(project), "--nologo"], project_dir)
actual_changed = run(["dotnet", "run", "--project", str(project), "--no-build"], project_dir)
with tempfile.TemporaryDirectory(prefix="task039-review-") as temporary:
    copy_dir = Path(temporary)
    copy_project = copy_dir / project.name
    shutil.copy2(project, copy_project)
    shutil.copy2(entry, copy_dir / entry.name)
    (copy_dir / source.name).write_bytes(baseline)
    run(["dotnet", "build", str(copy_project), "--nologo"], copy_dir)
    actual_original = run(["dotnet", "run", "--project", str(copy_project), "--no-build"], copy_dir)

after = {str(p): hashlib.sha256(p.read_bytes()).hexdigest() for p in inputs}
checks = {
    "submitted_output_matches_changed_sample": actual_changed == header + rows_changed,
    "isolated_output_matches_original_sample": actual_original == header + rows_original,
    "only_array_index_four_output_changed": [i for i, pair in enumerate(zip(actual_original, actual_changed)) if pair[0] != pair[1]] == [9],
    "learner_inputs_unchanged": before == after,
}
report["checks"] = checks
report["source_sha256_after"] = after
report["all_passed"] = all(checks.values())
(evidence_dir / "review-2026-10-19.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print(json.dumps({"checks": checks, "all_passed": report["all_passed"]}, ensure_ascii=False))
assert report["all_passed"]
