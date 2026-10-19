from pathlib import Path
import hashlib
import json
import subprocess
import tempfile
from xml.sax.saxutils import escape

root = Path(__file__).resolve().parents[5]
folder = Path(__file__).resolve().parent
project = root / 'Stage03-DeviceCommunication/DeviceCommunication.Wpf'
destination = folder / 'startup-2026-10-19.json'
if destination.exists():
    raise RuntimeError('Refusing to overwrite preparation evidence')
command = ['dotnet', 'build', str(project / 'DeviceCommunication.Wpf.csproj'), '--nologo']
build = subprocess.run(command, capture_output=True, timeout=45)
if build.returncode != 0:
    raise RuntimeError(build.stdout.decode('utf-8', errors='replace'))
dll = project / 'bin/Debug/net10.0-windows/DeviceCommunication.Wpf.dll'
with tempfile.TemporaryDirectory(prefix='task046-startup-') as temporary:
    location = Path(temporary)
    xml = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0-windows</TargetFramework><UseWPF>true</UseWPF><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="DeviceCommunication.Wpf"><HintPath>' + escape(str(dll)) + '</HintPath></Reference><PackageReference Include="System.IO.Ports" Version="10.0.0" /></ItemGroup></Project>'
    (location / 'Review.csproj').write_text(xml, encoding='utf-8')
    (location / 'Program.cs').write_text((folder / 'StartupReview.cs.txt').read_text(encoding='utf-8'), encoding='utf-8')
    review_command = ['dotnet', 'run', '--project', str(location / 'Review.csproj'), '--verbosity', 'quiet', '--', str(folder)]
    review = subprocess.run(review_command, capture_output=True, timeout=45)
output = review.stdout.decode('utf-8', errors='replace')
lines = [line for line in output.splitlines() if line.startswith('{')]
parsed = json.loads(lines[-1]) if lines else None
baseline = json.loads((folder / 'baseline-2026-10-19.json').read_text(encoding='utf-8'))
unchanged = all(hashlib.sha256((root / path).read_bytes()).hexdigest() == digest for path, digest in baseline.items())
record = {'date': '2026-10-19', 'scope': 'Coach scaffold startup and closure only; no serial communication or learner DoD tests.',
          'build': {'command': command, 'exit_code': build.returncode, 'stdout': build.stdout.decode('utf-8', errors='replace')},
          'startup': {'command': review_command, 'exit_code': review.returncode, 'stdout': output, 'stderr': review.stderr.decode('utf-8', errors='replace'), 'result': parsed},
          'preserved_file_count': len(baseline), 'previous_work_unchanged': unchanged, 'dod_accepted': False}
destination.write_text(json.dumps(record, ensure_ascii=False, indent=2), encoding='utf-8')
print(json.dumps({'build_exit_code': build.returncode, 'startup_exit_code': review.returncode, 'result': parsed, 'previous_work_unchanged': unchanged}, ensure_ascii=True))
assert review.returncode == 0 and parsed is not None and parsed['loaded'] and parsed['closed'] and unchanged
