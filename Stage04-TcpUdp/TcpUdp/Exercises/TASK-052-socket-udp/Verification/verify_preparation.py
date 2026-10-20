"""Coach support checks only; learner TODOs intentionally remain incomplete."""
from pathlib import Path
from xml.sax.saxutils import escape
from datetime import datetime
import hashlib
import json
import socket
import subprocess
import tempfile
import time
import uuid

folder = Path(__file__).resolve().parent
root = folder.parents[4]
project = root / "Stage04-TcpUdp/TcpUdp/TcpUdp.csproj"
checks = []
commands = []

def check(name, condition):
    assert condition, name
    checks.append(name)

def free_port():
    for _ in range(50):
        with socket.socket() as probe, socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as udp_probe:
            probe.bind(("127.0.0.1", 0))
            port = probe.getsockname()[1]
            try:
                udp_probe.bind(("127.0.0.1", port))
                return port
            except OSError:
                pass
    raise RuntimeError("No free TCP/UDP port pair.")

with tempfile.TemporaryDirectory(prefix="task052-preparation-") as tmp:
    tmp = Path(tmp)
    command = ["dotnet", "build", str(project), "--nologo", "--output", str(tmp / "app")]
    commands.append(command)
    build = subprocess.run(command, capture_output=True, timeout=60)
    assert build.returncode == 0, build.stdout.decode("utf-8", errors="replace")
    host = tmp / "host"
    host.mkdir()
    (host / "Host.csproj").write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="TcpUdp"><HintPath>' + escape(str(tmp / "app/TcpUdp.dll")) + '</HintPath></Reference></ItemGroup></Project>', encoding="utf-8")
    (host / "Program.cs").write_text((folder / "PreparationHost.cs.txt").read_text(encoding="utf-8"), encoding="utf-8")
    host_build = ["dotnet", "build", str(host / "Host.csproj"), "--nologo", "--output", str(tmp / "fixture")]
    commands.append(host_build)
    fixture = subprocess.run(host_build, capture_output=True, timeout=60)
    assert fixture.returncode == 0, fixture.stdout.decode("utf-8", errors="replace")
    fixture_dll = str(tmp / "fixture/Host.dll")
    stub = subprocess.run(["dotnet", fixture_dll, "stubs"], capture_output=True, timeout=15)
    check("six_scaffold_checks", stub.returncode == 0 and len(json.loads(stub.stdout)) == 6)
    cli_dll = str(tmp / "app/TcpUdp.dll")
    for args, expected in [(["--help"], 0), (["--task052"], 1), (["--task052", "server", "0"], 1), (["--task052", "server", "5000", "invalid"], 1), (["--task052", "udp", "5000", "START_DEVICE"], 1), (["--task052", "socket"], 1)]:
        result = subprocess.run(["dotnet", cli_dll] + args, capture_output=True, timeout=15)
        check("cli_" + "_".join(args), result.returncode == expected)

    def line(stream):
        data = b""
        while not data.endswith(b"\n"):
            part = stream.recv(1)
            if not part:
                return data
            data += part
        return data

    for mode in ["normal", "silent", "duplicate", "stale-first", "foreign-first", "wrong-id-first", "malformed-first", "close-tcp"]:
        port = free_port()
        server_command = ["dotnet", fixture_dll, "server", str(port), mode]
        commands.append(server_command)
        server = subprocess.Popen(server_command, stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
        try:
            # Probe only our newly created fixture; all ports are ephemeral.
            deadline = time.monotonic() + 8
            while True:
                try:
                    tcp = socket.create_connection(("127.0.0.1", port), timeout=.5)
                    break
                except OSError:
                    assert time.monotonic() < deadline and server.poll() is None
                    time.sleep(.05)
            tcp.settimeout(3)
            with tcp:
                if mode == "close-tcp":
                    tcp.sendall(b"GET_TEMP\n")
                    check("tcp_eof_fault", tcp.recv(1) == b"")
                    continue
                # Command fragments and consecutive messages enter the real reused LF parser.
                tcp.sendall(b"START_")
                tcp.sendall(b"DEVICE\nGET_STATUS\n")
                check(mode + "_tcp_control_and_status", line(tcp) == b"OK\n" and line(tcp) == b"RUNNING\n")
                with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as udp:
                    udp.bind(("127.0.0.1", 0))
                    udp.settimeout(.25 if mode == "silent" else 3)
                    request_id = uuid.uuid4().hex
                    query = ("Q|" + request_id + "|GET_STATUS").encode()
                    expected = ("R|" + request_id + "|GET_STATUS|RUNNING").encode()
                    udp.sendto(query, ("127.0.0.1", port))
                    if mode in ["silent", "stale-first"]:
                        udp.settimeout(2.15 if mode == "stale-first" else .25)
                        try:
                            udp.recvfrom(1024)
                            raise AssertionError("Expected no first reply: " + mode)
                        except socket.timeout:
                            checks.append(mode + "_no_first_reply")
                        if mode == "silent":
                            continue
                        old = expected
                        request_id = uuid.uuid4().hex
                        expected = ("R|" + request_id + "|GET_STATUS|RUNNING").encode()
                        udp.sendto(("Q|" + request_id + "|GET_STATUS").encode(), ("127.0.0.1", port))
                        check("old_reply_after_previous_deadline", udp.recvfrom(1024)[0] == old)
                    if mode == "foreign-first":
                        foreign, source = udp.recvfrom(1024)
                        check("wrong_source_port_fixture", foreign == expected and source[1] != port)
                    if mode == "wrong-id-first":
                        wrong, source = udp.recvfrom(1024)
                        check("wrong_id_fixture", source[1] == port and wrong.split(b"|")[1] != request_id.encode())
                    if mode == "malformed-first":
                        check("overlength_fixture", len(udp.recvfrom(1024)[0]) == 129)
                        check("invalid_value_fixture", udp.recvfrom(1024)[0].endswith(b"|INVALID"))
                    reply, source = udp.recvfrom(1024)
                    check(mode + "_udp_shared_running_state", reply == expected and source == ("127.0.0.1", port))
                    if mode == "duplicate":
                        check("duplicate_fixture", udp.recvfrom(1024)[0] == expected)
        finally:
            try:
                stdout, stderr = server.communicate(input=b"\n", timeout=8)
                if server.returncode != 0:
                    print(mode, stdout.decode("utf-8", errors="replace").encode("ascii", errors="backslashreplace"), stderr.decode("utf-8", errors="replace").encode("ascii", errors="backslashreplace"))
                check(mode + "_cancel_and_release", server.returncode == 0 and b"SERVER_STOPPED_CLEANLY" in stdout)
            except BaseException:
                server.kill()
                server.communicate()
                raise
    baseline = json.loads((folder / "baseline-2026-10-20.json").read_text(encoding="utf-8"))
    unchanged = all(hashlib.sha256((root / p).read_bytes()).hexdigest() == digest for p, digest in baseline.items())
    check("protected_sources_unchanged", unchanged)
record = {"recorded_at": datetime.now().astimezone().isoformat(), "scope": "Coach simulator, CLI, incomplete scaffold and cancellation checks; not learner TODO or task acceptance.", "commands": commands, "build_exit_code": build.returncode, "build_stdout": build.stdout.decode("utf-8", errors="replace"), "fixture_build_stdout": fixture.stdout.decode("utf-8", errors="replace"), "scaffold_checks": json.loads(stub.stdout), "checks": checks, "protected_file_count": len(baseline), "protected_sources_unchanged": unchanged, "learner_dod_accepted": False}
(folder / "preparation-2026-10-20.json").write_text(json.dumps(record, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print(json.dumps({"build_exit_code": build.returncode, "checks_passed": len(checks), "scaffold_checks": len(record["scaffold_checks"]), "protected_file_count": len(baseline), "protected_sources_unchanged": unchanged}))
