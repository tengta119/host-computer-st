param([string]$Executable, [string]$OutputPath)
$ErrorActionPreference = 'Stop'
if (Test-Path -LiteralPath $OutputPath) { throw 'Existing launch result must not be overwritten.' }
$absoluteExecutable = (Resolve-Path -LiteralPath $Executable).Path
$process = Start-Process -FilePath $absoluteExecutable -WindowStyle Hidden -PassThru
$inputIdle = $process.WaitForInputIdle(10000)
for ($attempt = 0; $attempt -lt 30; $attempt++) {
    $process.Refresh()
    if ($process.MainWindowHandle -ne 0 -or $process.HasExited) { break }
    Start-Sleep -Milliseconds 100
}
$process.Refresh()
$title = $process.MainWindowTitle
$handle = $process.MainWindowHandle.ToInt64()
$closeRequested = $process.CloseMainWindow()
$closed = $process.WaitForExit(10000)
$result = [pscustomobject]@{
    executable = $absoluteExecutable; pid = $process.Id; inputIdle = $inputIdle;
    title = $title; mainWindowHandle = $handle; closeRequested = $closeRequested;
    closed = $closed; exitCode = $(if ($closed) { $process.ExitCode } else { $null })
}
$result | ConvertTo-Json | Set-Content -LiteralPath $OutputPath -Encoding utf8
$result | ConvertTo-Json -Compress
if (-not ($inputIdle -and $handle -ne 0 -and $title.Contains('TASK-031') -and $closed -and $process.ExitCode -eq 0)) { exit 1 }
