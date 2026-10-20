"""在临时工程编译真实缓冲源码，只核对TODO 4及有效追加，不改学员文件。"""
import hashlib
import json
import subprocess
import tempfile
from datetime import datetime
from pathlib import Path
from xml.sax.saxutils import escape

task = Path(__file__).resolve().parents[1]
sources = [task / "Task049LineBuffer.cs", task / "Task049BufferObservation.cs", task / "Task049ClientExercise.cs"]
before = {p.name: hashlib.sha256(p.read_bytes()).hexdigest() for p in sources}
program = r'''
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;
using TcpUdp.Exercises.Task049;

public class Program
{
    private static readonly List<string> Checks = new List<string>();
    private static void Check(string name, bool passed)
    {
        if (!passed) throw new Exception(name);
        Checks.Add(name);
    }
    private static void Add(Task049LineBuffer buffer, string text)
    {
        byte[] data = Encoding.UTF8.GetBytes(text);
        buffer.Append(data, data.Length);
    }
    private static bool Rejects(Task049LineBuffer buffer)
    {
        try { string message; buffer.TryTakeMessage(out message); return false; }
        catch (InvalidDataException) { return true; }
    }
    public static void Main()
    {
        Task049LineBuffer buffer = new Task049LineBuffer();
        string message;
        Check("空缓冲返回false和空字符串", !buffer.TryTakeMessage(out message) && message == "" && buffer.Count == 0);
        Add(buffer, "35");
        Check("半条保留", !buffer.TryTakeMessage(out message) && message == "" && buffer.Count == 2);
        Add(buffer, ".6\nOK\nST");
        Check("取第一条温度并保留后续", buffer.TryTakeMessage(out message) && message == "35.6" && buffer.Count == 5);
        Check("取第二条确认并保留半条", buffer.TryTakeMessage(out message) && message == "OK" && buffer.Count == 2);
        Check("末尾半条继续等待", !buffer.TryTakeMessage(out message) && message == "" && buffer.Count == 2);
        Add(buffer, "OPPED\n");
        Check("后续补齐半条", buffer.TryTakeMessage(out message) && message == "STOPPED" && buffer.Count == 0);
        Check("取完后不会重复返回", !buffer.TryTakeMessage(out message) && message == "");
        byte[] reused = new byte[] { 0x4F, 0x4B, 0xEE, 0xFF };
        buffer.Append(reused, 2);
        buffer.Append(reused, 0);
        Add(buffer, "\n");
        Check("有效数量和零数量不混入数组尾部", buffer.TryTakeMessage(out message) && message == "OK" && reused[2] == 0xEE && buffer.Count == 0);
        Add(buffer, new String('A', 256));
        Check("恰好256且无LF仍等待", !buffer.TryTakeMessage(out message) && buffer.Count == 256);
        Add(buffer, "\n");
        Check("256字节加LF允许取出", buffer.TryTakeMessage(out message) && message.Length == 256 && buffer.Count == 0);
        Add(buffer, new String('A', 257));
        Check("257字节无LF拒绝且未消费", Rejects(buffer) && buffer.Count == 257);
        buffer.Clear();
        Add(buffer, new String('A', 257) + "\n");
        Check("257字节带LF拒绝且未消费", Rejects(buffer) && buffer.Count == 258);
        buffer.Clear();
        Add(buffer, "\n");
        Check("空消息拒绝", Rejects(buffer) && buffer.Count == 1);
        buffer.Clear();
        for (int i = 0; i < 100; i++) Add(buffer, "OK\n");
        int count = 0;
        bool allCorrect = true;
        while (buffer.TryTakeMessage(out message)) { count++; if (message != "OK") allCorrect = false; }
        Check("总缓冲超过256但每条合法仍逐条提取", allCorrect && count == 100 && buffer.Count == 0);
        Add(buffer, "OK\n" + new String('A', 257));
        Check("先取合法前缀不被后续超长误拒绝", buffer.TryTakeMessage(out message) && message == "OK" && buffer.Count == 257);
        Check("后续超长在下一次提取拒绝", Rejects(buffer));
        buffer.Clear();
        byte[] unicode = Encoding.UTF8.GetBytes("温度\n");
        buffer.Append(unicode, 2);
        bool incomplete = !buffer.TryTakeMessage(out message);
        byte[] rest = new byte[unicode.Length - 2];
        Array.Copy(unicode, 2, rest, 0, rest.Length);
        buffer.Append(rest, rest.Length);
        Check("完整消息后再解码不破坏跨批UTF8字符", incomplete && buffer.TryTakeMessage(out message) && message == "温度" && buffer.Count == 0);
        Add(buffer, "unfinished");
        buffer.Clear();
        Check("清理后没有旧半条", buffer.Count == 0 && !buffer.TryTakeMessage(out message));
        Task049BufferObservation.Run();
        Console.WriteLine("CHECK_RESULT=" + JsonSerializer.Serialize(Checks));
    }
}
'''
with tempfile.TemporaryDirectory(prefix="task049-todo4-") as directory:
    folder = Path(directory)
    includes = "".join('<Compile Include="' + escape(str(p), {'"': '&quot;'}) + '" />' for p in sources[:2])
    project = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><Nullable>enable</Nullable></PropertyGroup><ItemGroup>' + includes + '</ItemGroup></Project>'
    (folder / "BufferReview.csproj").write_text(project, encoding="utf-8")
    (folder / "Program.cs").write_text(program, encoding="utf-8")
    build_command = ["dotnet", "build", str(folder / "BufferReview.csproj"), "--nologo"]
    build = subprocess.run(build_command, capture_output=True, text=True, encoding="utf-8", timeout=60)
    print(build.stdout)
    if build.returncode:
        raise RuntimeError(build.stderr)
    run_command = ["dotnet", str(folder / "bin/Debug/net10.0/BufferReview.dll")]
    run = subprocess.run(run_command, capture_output=True, text=True, encoding="utf-8", timeout=10)
    print(run.stdout)
    if run.returncode:
        raise RuntimeError(run.stderr)
    checks = json.loads(next(line.removeprefix("CHECK_RESULT=") for line in run.stdout.splitlines() if line.startswith("CHECK_RESULT=")))
    after = {p.name: hashlib.sha256(p.read_bytes()).hexdigest() for p in sources}
    assert before == after
    report = {"recorded_at": datetime.now().astimezone().isoformat(), "scope": "教练完成TODO 4后的局部验证，不作为整题验收或学员自主掌握。", "build_command": build_command, "build_exit_code": build.returncode, "build_stdout": build.stdout, "run_command": run_command, "run_exit_code": run.returncode, "run_stdout": run.stdout, "checks": checks, "passed": len(checks), "source_hashes": after, "sources_unchanged_during_verification": before == after}
    output = task / "Verification/todo4-assisted-2026-10-20.json"
    output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print("REPORT=" + str(output))
