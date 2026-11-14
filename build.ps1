[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'MeTimer.cs'
$output = Join-Path $root 'meTimer.exe'
$tempOutput = Join-Path $root 'MeTimer.build.exe'
$icon = Join-Path $root 'app.ico'
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'

if (-not (Test-Path -LiteralPath $csc)) {
    throw "找不到 .NET Framework C# 编译器: $csc"
}

if (-not (Test-Path -LiteralPath $source)) {
    throw "找不到源码: $source"
}

if (-not (Test-Path -LiteralPath $icon)) {
    throw "找不到应用图标: $icon"
}

$references = @(
    'System.dll',
    'System.Core.dll',
    'System.Drawing.dll',
    'System.Windows.Forms.dll'
) | ForEach-Object {
    '/reference:' + (Join-Path $env:WINDIR ("Microsoft.NET\Framework64\v4.0.30319\" + $_))
}

if (Test-Path -LiteralPath $tempOutput) {
    Remove-Item -LiteralPath $tempOutput -Force
}

$arguments = @(
    '/nologo',
    '/target:winexe',
    '/platform:anycpu',
    '/optimize+',
    '/debug-',
    '/codepage:65001',
    '/utf8output',
    ('/win32icon:' + $icon),
    ('/out:' + $tempOutput)
) + $references + @($source)

& $csc $arguments
if ($LASTEXITCODE -ne 0) {
    throw "编译失败，退出码 $LASTEXITCODE"
}

if (-not (Test-Path -LiteralPath $tempOutput)) {
    throw "编译未生成输出文件"
}

if (Test-Path -LiteralPath $output) {
    Remove-Item -LiteralPath $output -Force
}

Move-Item -LiteralPath $tempOutput -Destination $output -Force

$file = Get-Item -LiteralPath $output
Write-Host ("已生成: {0}" -f $file.FullName)
Write-Host ("大小: {0:N0} 字节 ({1:N1} KB)" -f $file.Length, ($file.Length / 1KB))
