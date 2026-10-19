from pathlib import Path
import hashlib
import json
import re
import subprocess
import sys


root = Path(__file__).resolve().parents[5]
project = root / 'Stage03-DeviceCommunication/DeviceCommunication/DeviceCommunication.csproj'
source = Path(__file__).resolve().parent.parent / 'Task043Exercise.cs'
entry = project.parent / 'Program.cs'
destination = Path(__file__).resolve().parent / sys.argv[1]
if destination.exists():
    raise RuntimeError('Refusing to overwrite existing evidence')
before = {str(p.relative_to(root)): hashlib.sha256(p.read_bytes()).hexdigest() for p in (source, entry)}
source_snapshot = source.read_text(encoding='utf-8-sig')
build_command = ['dotnet', 'build', str(project), '--nologo']
build = subprocess.run(build_command, capture_output=True, timeout=40)
run_command = ['dotnet', str(project.parent / 'bin/Debug/net10.0/DeviceCommunication.dll')]
timed_out = False
stdout = stderr = ''
exit_code = None
if build.returncode == 0:
    process = subprocess.Popen(run_command, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    try:
        out, err = process.communicate(timeout=4)
    except subprocess.TimeoutExpired:
        timed_out = True
        process.kill()
        out, err = process.communicate()
    stdout, stderr = out.decode('utf-8', errors='replace'), err.decode('utf-8', errors='replace')
    exit_code = process.returncode

A = 'AA 01 03 00 64 5F'
B = 'AA 02 03 00 65 60'
C = 'AA 03 AA 00 66 61'
expected = [
    (0, '', 0), (0, '', 0),
    (0, '', 1), (0, '', 1),
    (2, 'AA 01', 0), (0, '', 1), (0, '', 1),
    (0, '', 2), (0, '', 2),
    (0, '', 1), (0, '', 1),
    (2, 'AA 01', 0), (1, 'AA', 1), (0, '', 2), (0, '', 2),
    (0, '', 0),
]
observed = [(int(n), data.strip(), int(count)) for n, data, count in re.findall(
    r'缓冲剩余数量：(\d+)\r?\n缓冲剩余字节：([^\r\n]*)\r?\n本组已输出帧数：(\d+)', stdout
)]
checkpoints = [{'number': i + 1, 'expected': item, 'actual': observed[i] if i < len(observed) else None,
                'passed': i < len(observed) and tuple(observed[i]) == item} for i, item in enumerate(expected)]
frames = [frame.strip() for frame in re.findall(r'完整帧\d+：([^\r\n]*)', stdout)]
expected_frames = [A, A, A, B, C, A, B]
after = {str(p.relative_to(root)): hashlib.sha256(p.read_bytes()).hexdigest() for p in (source, entry)}
record = {
    'date': '2026-10-19', 'source_snapshot': source_snapshot,
    'build': {'command': build_command, 'exit_code': build.returncode,
              'stdout': build.stdout.decode('utf-8', errors='replace'), 'stderr': build.stderr.decode('utf-8', errors='replace')},
    'run': {'command': run_command, 'timed_out': timed_out, 'timeout_seconds': 4,
            'terminated_by_reviewer': timed_out, 'exit_code': exit_code, 'stdout': stdout, 'stderr': stderr},
    'checkpoints': checkpoints, 'frames': frames, 'expected_frames': expected_frames,
    'frame_sequence_passed': frames == expected_frames,
    'source_sha256_before': before, 'source_sha256_after': after,
    'source_and_entry_unchanged': before == after,
    'all_runtime_checks_passed': build.returncode == 0 and not timed_out and exit_code == 0
                                 and all(c['passed'] for c in checkpoints) and frames == expected_frames and before == after,
}
destination.write_text(json.dumps(record, ensure_ascii=False, indent=2), encoding='utf-8')
print(json.dumps({'evidence': str(destination), 'build_exit_code': build.returncode, 'run_exit_code': exit_code,
                  'timed_out': timed_out, 'observations': len(observed), 'passed_checkpoints': sum(c['passed'] for c in checkpoints),
                  'failed_checkpoints': [c for c in checkpoints if not c['passed']],
                  'frames_match': record['frame_sequence_passed'], 'source_unchanged': before == after}, ensure_ascii=True, indent=2))
