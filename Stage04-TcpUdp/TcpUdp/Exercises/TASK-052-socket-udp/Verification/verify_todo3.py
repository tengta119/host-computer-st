"""Verify only the coach-assisted receive method against a local Socket peer."""
from pathlib import Path
from xml.sax.saxutils import escape
from datetime import datetime
import json
import subprocess
import tempfile

folder = Path(__file__).resolve().parent
root = folder.parents[4]
project = root / "Stage04-TcpUdp/TcpUdp/TcpUdp.csproj"
with tempfile.TemporaryDirectory(prefix="task052-todo3-") as tmp:
    tmp = Path(tmp)
    build_command = ["dotnet", "build", str(project), "--nologo", "--output", str(tmp / "app")]
    build = subprocess.run(build_command, capture_output=True, timeout=60)
    assert build.returncode == 0, build.stdout.decode("utf-8", errors="replace")
    host = tmp / "host"
    host.mkdir()
    (host / "Host.csproj").write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="TcpUdp"><HintPath>' + escape(str(tmp / "app/TcpUdp.dll")) + '</HintPath></Reference></ItemGroup></Project>', encoding="utf-8")
    (host / "Program.cs").write_text((folder / "Todo3Review.cs.txt").read_text(encoding="utf-8"), encoding="utf-8")
    review_command = ["dotnet", "run", "--project", str(host / "Host.csproj"), "--verbosity", "quiet"]
    review = subprocess.run(review_command, capture_output=True, timeout=45)
stdout = review.stdout.decode("utf-8", errors="replace")
parsed = json.loads(stdout.splitlines()[-1]) if review.returncode == 0 else None
record = {"recorded_at": datetime.now().astimezone().isoformat(), "scope": "Coach-assisted TODO 3 receive method only; no full task acceptance.", "build": {"command": build_command, "exit_code": build.returncode, "stdout": build.stdout.decode("utf-8", errors="replace")}, "review": {"command": review_command, "exit_code": review.returncode, "stdout": stdout, "stderr": review.stderr.decode("utf-8", errors="replace"), "result": parsed}}
(folder / "todo3-assisted-2026-10-20.json").write_text(json.dumps(record, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print(json.dumps({"build_exit_code": build.returncode, "review_exit_code": review.returncode, "result": parsed}, ensure_ascii=True))
assert review.returncode == 0 and parsed is not None
