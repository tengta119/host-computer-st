"""Focused verification of coach-assisted TODO 2; not whole-task acceptance."""
from pathlib import Path
from xml.sax.saxutils import escape
from datetime import datetime
import json
import subprocess
import tempfile

folder = Path(__file__).resolve().parent
root = folder.parents[4]
project = root / "Stage04-TcpUdp/TcpUdp.Wpf/TcpUdp.Wpf.csproj"
with tempfile.TemporaryDirectory(prefix="task051-todo2-") as temporary:
    location = Path(temporary)
    output = location / "app"
    command = ["dotnet", "build", str(project), "--nologo", "--output", str(output)]
    build = subprocess.run(command, capture_output=True, timeout=60)
    assert build.returncode == 0, build.stdout.decode("utf-8", errors="replace")
    host = location / "host"
    host.mkdir()
    dll = output / "TcpUdp.Wpf.dll"
    xml = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0-windows</TargetFramework><UseWPF>true</UseWPF><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="TcpUdp.Wpf"><HintPath>' + escape(str(dll)) + '</HintPath></Reference></ItemGroup></Project>'
    (host / "Review.csproj").write_text(xml, encoding="utf-8")
    (host / "Program.cs").write_text((folder / "Todo2Review.cs.txt").read_text(encoding="utf-8"), encoding="utf-8")
    review_command = ["dotnet", "run", "--project", str(host / "Review.csproj"), "--verbosity", "quiet"]
    review = subprocess.run(review_command, capture_output=True, timeout=60)
stdout = review.stdout.decode("utf-8", errors="replace")
lines = [line.removeprefix("TODO2_RESULT=") for line in stdout.splitlines() if line.startswith("TODO2_RESULT=")]
result = json.loads(lines[-1]) if lines else None
record = {"recorded_at": datetime.now().astimezone().isoformat(), "scope": "Coach-assisted TODO 2 only; no whole-task acceptance or independent learner mastery.", "build": {"command": command, "exit_code": build.returncode, "stdout": build.stdout.decode("utf-8", errors="replace")}, "review": {"command": review_command, "exit_code": review.returncode, "stdout": stdout, "stderr": review.stderr.decode("utf-8", errors="replace"), "result": result}}
(folder / "todo2-assisted-2026-10-20.json").write_text(json.dumps(record, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print(json.dumps({"build_exit_code": build.returncode, "review_exit_code": review.returncode, "result": result}, ensure_ascii=True))
assert review.returncode == 0 and result is not None and result["failure"] is None
