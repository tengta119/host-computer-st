"""只验证教练骨架，不代替TASK-051学员TODO验收。"""
from pathlib import Path
from xml.sax.saxutils import escape
from datetime import datetime
import hashlib
import json
import subprocess
import tempfile

folder = Path(__file__).resolve().parent
root = folder.parents[4]
project = root / "Stage04-TcpUdp/TcpUdp.Wpf/TcpUdp.Wpf.csproj"
baseline = json.loads((folder / "baseline-2026-10-20.json").read_text(encoding="utf-8"))
with tempfile.TemporaryDirectory(prefix="task051-startup-") as temporary:
    location = Path(temporary)
    output = location / "app"
    command = ["dotnet", "build", str(project), "--nologo", "--output", str(output)]
    build = subprocess.run(command, capture_output=True, timeout=60)
    if build.returncode:
        raise RuntimeError(build.stdout.decode("utf-8", errors="replace"))
    host = location / "host"
    host.mkdir()
    dll = output / "TcpUdp.Wpf.dll"
    simulator = root / "Stage04-TcpUdp/TcpUdp/Exercises/TASK-051-fault-simulator/Task051Simulator.cs"
    xml = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0-windows</TargetFramework><UseWPF>true</UseWPF><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="TcpUdp.Wpf"><HintPath>' + escape(str(dll)) + '</HintPath></Reference><Compile Include="' + escape(str(simulator)) + '" /></ItemGroup></Project>'
    (host / "Review.csproj").write_text(xml, encoding="utf-8")
    (host / "Program.cs").write_text((folder / "StartupReview.cs.txt").read_text(encoding="utf-8"), encoding="utf-8")
    review_command = ["dotnet", "run", "--project", str(host / "Review.csproj"), "--verbosity", "quiet", "--", str(folder)]
    review = subprocess.run(review_command, capture_output=True, timeout=60)
text = review.stdout.decode("utf-8", errors="replace")
lines = [line.removeprefix("PREPARATION_RESULT=") for line in text.splitlines() if line.startswith("PREPARATION_RESULT=")]
parsed = json.loads(lines[-1]) if lines else None
unchanged = all(hashlib.sha256((root / path).read_bytes()).hexdigest() == digest for path, digest in baseline.items())
record = {"recorded_at": datetime.now().astimezone().isoformat(), "scope": "教练骨架窗口/布局、故障对端、超时分类辅助与取消/上限保护，不计学员TODO通过；未实际诱发连接建立超时。", "build": {"command": command, "exit_code": build.returncode, "stdout": build.stdout.decode("utf-8", errors="replace")}, "startup": {"command": review_command, "exit_code": review.returncode, "stdout": text, "stderr": review.stderr.decode("utf-8", errors="replace"), "result": parsed}, "preserved_file_count": len(baseline), "previous_work_unchanged": unchanged, "learner_dod_accepted": False}
(folder / "startup-2026-10-20.json").write_text(json.dumps(record, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print(json.dumps({"build_exit_code": build.returncode, "startup_exit_code": review.returncode, "result": parsed, "preserved_file_count": len(baseline), "previous_work_unchanged": unchanged}, ensure_ascii=True))
assert review.returncode == 0 and parsed is not None and parsed["failure"] is None and unchanged
