"""只验证教练提供的入口和模拟器，不实现或验收学员TODO。"""
import json
import socket
import subprocess
import time
from datetime import datetime
from pathlib import Path


task_dir = Path(__file__).resolve().parents[1]
project_dir = task_dir.parents[1]
assembly = project_dir / "bin/Debug/net10.0/TcpUdp.dll"
checks = []
server_logs = []


def check(name, condition, detail=""):
    checks.append({"name": name, "passed": bool(condition), "detail": detail})
    if not condition:
        raise AssertionError(name + ": " + str(detail))


def run_entry(arguments, input_text=None):
    result = subprocess.run(
        ["dotnet", str(assembly)] + arguments,
        input=input_text, capture_output=True, text=True, encoding="utf-8", timeout=8,
    )
    return result


def start_simulator(split=False):
    with socket.socket() as reservation:
        reservation.bind(("127.0.0.1", 0))
        port = reservation.getsockname()[1]
    arguments = ["dotnet", str(assembly), "--simulator", str(port)]
    if split:
        arguments.append("split")
    process = subprocess.Popen(arguments, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    deadline = time.monotonic() + 5
    while time.monotonic() < deadline:
        if process.poll() is not None:
            output, errors = process.communicate()
            raise RuntimeError((output + errors).decode("utf-8", errors="replace"))
        try:
            connection = socket.create_connection(("127.0.0.1", port), timeout=0.2)
            connection.settimeout(3)
            return process, port, connection, arguments
        except OSError:
            time.sleep(0.05)
    process.terminate()
    process.wait(timeout=3)
    raise TimeoutError("模拟器未及时开始监听。")


def stop_simulator(process, arguments):
    # 只终止本脚本创建的模拟器，不将此操作记为Ctrl+C正常退出证据。
    process.terminate()
    try:
        output, errors = process.communicate(timeout=3)
    except subprocess.TimeoutExpired:
        process.kill()
        output, errors = process.communicate(timeout=3)
    server_logs.append({
        "command": subprocess.list2cmdline(arguments),
        "stop": "验证脚本终止自己创建的进程，未验证交互式Ctrl+C。",
        "stdout": output.decode("utf-8", errors="replace"),
        "stderr": errors.decode("utf-8", errors="replace"),
    })


def read_lines(connection, count=1):
    data = bytearray()
    sizes = []
    while data.count(10) < count:
        chunk = connection.recv(1024)
        if not chunk:
            raise EOFError("回复尚未完整时连接已经关闭。")
        data.extend(chunk)
        sizes.append(len(chunk))
    return bytes(data), sizes


try:
    result = run_entry(["--help"])
    check("帮助入口正常退出", result.returncode == 0 and "TASK-049" in result.stdout)
    result = run_entry(["--client"], "o\n1\nq\n")
    check("未实现连接时如实显示占位并安全退出", result.returncode == 0 and "TODO 1尚未连接" in result.stdout and "请先连接" in result.stdout)
    result = run_entry(["--observe"])
    check("人工观察如实显示尚未完成而非虚报通过", result.returncode == 0 and "未实现的TODO不会被报告为通过" in result.stdout and "完整消息：" not in result.stdout)
    check("过多观察入口参数被拒绝", run_entry(["--observe", "extra"]).returncode == 1)
    check("无效端口被拒绝", run_entry(["--client", "0"]).returncode == 1)

    process, port, connection, arguments = start_simulator()
    try:
        with connection:
            for command, expected in [
                ("GET_TEMP", "35.6"), ("GET_STATUS", "STOPPED"),
                ("START_DEVICE", "OK"), ("GET_STATUS", "RUNNING"),
                ("STOP_DEVICE", "OK"), ("GET_STATUS", "STOPPED"),
                ("PING", "PONG"), ("UNKNOWN", "ERR_UNKNOWN_COMMAND"),
                ("", "ERR_EMPTY_COMMAND"),
            ]:
                connection.sendall((command + "\n").encode("ascii"))
                data, sizes = read_lines(connection)
                check("模拟器命令" + (command or "<空行>") + "=>" + expected, data == (expected + "\n").encode("ascii"))
            connection.sendall(b"GET_")
            connection.settimeout(0.15)
            try:
                premature = connection.recv(1024)
            except socket.timeout:
                premature = None
            check("没有LF的命令片段不提前回复", premature is None)
            connection.settimeout(3)
            connection.sendall(b"TEMP\n")
            check("模拟器合并分批命令", read_lines(connection)[0] == b"35.6\n")
            connection.sendall(b"GET_TEMP\nGET_STATUS\n")
            check("模拟器逐条处理同批命令", read_lines(connection, 2)[0] == b"35.6\nSTOPPED\n")
            connection.sendall(b"A" * 256 + b"\n")
            check("256字节命令长度边界仍被解析", read_lines(connection)[0] == b"ERR_UNKNOWN_COMMAND\n")
            connection.sendall(b"START_DEVICE\n")
            check("重连前启动设备", read_lines(connection)[0] == b"OK\n")
        with socket.create_connection(("127.0.0.1", port), timeout=3) as next_connection:
            next_connection.sendall(b"GET_STATUS\n")
            check("模拟设备状态跨连接保留", read_lines(next_connection)[0] == b"RUNNING\n")
        for name, payload in [("257字节", b"A" * 257), ("非ASCII", b"\x80"), ("CR", b"\r")]:
            with socket.create_connection(("127.0.0.1", port), timeout=3) as bad_connection:
                bad_connection.sendall(payload)
                check("模拟器拒绝" + name + "并关闭该连接", bad_connection.recv(1024) == b"")
        with socket.create_connection(("127.0.0.1", port), timeout=3) as final_connection:
            final_connection.sendall(b"GET_TEMP\n")
            check("非法输入后仍可重新连接查询", read_lines(final_connection)[0] == b"35.6\n")
    finally:
        stop_simulator(process, arguments)

    process, port, connection, arguments = start_simulator(split=True)
    try:
        with connection:
            connection.sendall(b"GET_TEMP\n")
            data, sizes = read_lines(connection)
            check("模拟器两次写入模式得到完整温度", data == b"35.6\n", {"actual_read_sizes": sizes, "note": "不要求网络读取批次等于写入批次。"})
    finally:
        stop_simulator(process, arguments)
finally:
    report = {
        "recorded_at": datetime.now().astimezone().isoformat(),
        "scope": "只验证教练提供的骨架入口及TCP模拟器，未实现或验收学员TODO。",
        "command": "python .\\Stage04-TcpUdp\\TcpUdp\\Exercises\\TASK-049-tcp-roundtrip\\Verification\\verify_preparation.py",
        "passed": sum(item["passed"] for item in checks),
        "total": len(checks), "checks": checks, "simulators": server_logs,
    }
    output_path = task_dir / "Verification/preparation-2026-10-20.json"
    output_path.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({"passed": report["passed"], "total": report["total"], "report": str(output_path)}, ensure_ascii=False))
