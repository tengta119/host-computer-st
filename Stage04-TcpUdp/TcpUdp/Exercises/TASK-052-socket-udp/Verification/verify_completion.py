"""Execute the completed source, local fault peers and the real TASK-051 WPF window."""
from pathlib import Path
from xml.sax.saxutils import escape
from datetime import datetime
import hashlib
import json
import subprocess
import tempfile

folder = Path(__file__).resolve().parent
root = folder.parents[4]
source_roots = [root / 'Stage03-DeviceCommunication', root / 'Stage04-TcpUdp', root / 'WpfBae']
protected = {str(p.relative_to(root)): hashlib.sha256(p.read_bytes()).hexdigest() for stage in source_roots for p in stage.rglob('*') if p.is_file() and p.suffix in ['.cs', '.xaml', '.csproj'] and not {'bin', 'obj', 'TASK-052-socket-udp'}.intersection(p.parts)}
builds = []
with tempfile.TemporaryDirectory(prefix='task052-completion-') as tmp:
    tmp = Path(tmp)
    for name, project in [('console', 'Stage04-TcpUdp/TcpUdp/TcpUdp.csproj'), ('wpf', 'Stage04-TcpUdp/TcpUdp.Wpf/TcpUdp.Wpf.csproj')]:
        command = ['dotnet', 'build', str(root / project), '--nologo', '--output', str(tmp / name)]
        build = subprocess.run(command, capture_output=True, timeout=60)
        builds.append({'command': command, 'exit_code': build.returncode, 'stdout': build.stdout.decode('utf-8', errors='replace')})
        assert build.returncode == 0, builds[-1]
    host = tmp / 'host'
    host.mkdir()
    refs = ''.join('<Reference Include="' + name + '"><HintPath>' + escape(str(path)) + '</HintPath></Reference>' for name, path in [('TcpUdp', tmp / 'console/TcpUdp.dll'), ('TcpUdp.Wpf', tmp / 'wpf/TcpUdp.Wpf.dll')])
    # TcpUdp.Wpf already links Task049LineBuffer; alias the console assembly to avoid duplicate type resolution.
    refs = refs.replace('<Reference Include="TcpUdp">', '<Reference Include="TcpUdp"><Aliases>global,console</Aliases>')
    (host / 'Host.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0-windows</TargetFramework><UseWPF>true</UseWPF><Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings></PropertyGroup><ItemGroup>' + refs + '</ItemGroup></Project>', encoding='utf-8')
    source = (folder / 'CompletionReview.cs.txt').read_text(encoding='utf-8')
    source = 'extern alias console;\n' + source.replace('using TcpUdp.Exercises.Task049;', 'using Task049LineBuffer = console::TcpUdp.Exercises.Task049.Task049LineBuffer;')
    (host / 'Program.cs').write_text(source, encoding='utf-8')
    command = ['dotnet', 'run', '--project', str(host / 'Host.csproj'), '--verbosity', 'quiet']
    review = subprocess.run(command, capture_output=True, timeout=110)
stdout = review.stdout.decode('utf-8', errors='replace')
lines = [line.removeprefix('COMPLETION_RESULT=') for line in stdout.splitlines() if line.startswith('COMPLETION_RESULT=')]
result = json.loads(lines[-1]) if lines else None
unchanged = all(hashlib.sha256((root / p).read_bytes()).hexdigest() == digest for p, digest in protected.items())
record = {'recorded_at': datetime.now().astimezone().isoformat(), 'scope': 'Coach-completed TODOs with real local Socket/UDP and WPF integration; not independent learner mastery.', 'builds': builds, 'review': {'command': command, 'exit_code': review.returncode, 'stdout': stdout, 'stderr': review.stderr.decode('utf-8', errors='replace'), 'result': result}, 'protected_source_count': len(protected), 'protected_sources_unchanged': unchanged}
(folder / 'completion-2026-10-20.json').write_text(json.dumps(record, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(json.dumps({'review_exit_code': review.returncode, 'passed': len(result['checks']) if result else 0, 'failure': result['failure'] if result else stdout[-3000:], 'stderr': record['review']['stderr'][-2000:], 'protected_sources_unchanged': unchanged}, ensure_ascii=True))
assert review.returncode == 0 and result and result['failure'] is None and unchanged
