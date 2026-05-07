$ErrorActionPreference = 'Stop'

function Get-VideoDedupEntries {
    $roots = @(
        'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\*',
        'HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*',
        'HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\*'
    )

    Get-ItemProperty -Path $roots -ErrorAction SilentlyContinue |
        Where-Object { $_.DisplayName -like '*VideoDedup*' } |
        Select-Object DisplayName, DisplayVersion, Publisher, PSChildName, PSPath, UninstallString, QuietUninstallString
}

function Convert-ToRegExePath([string]$psPath) {
    $psPath `
        -replace '^Microsoft\.PowerShell\.Core\\Registry::HKEY_LOCAL_MACHINE', 'HKLM' `
        -replace '^Microsoft\.PowerShell\.Core\\Registry::HKEY_CURRENT_USER', 'HKCU'
}

function Invoke-RegisteredUninstall([object]$app) {
    $cmd = if ($app.QuietUninstallString) { $app.QuietUninstallString } else { $app.UninstallString }

    if ([string]::IsNullOrWhiteSpace($cmd)) {
        return [pscustomobject]@{
            Name = $app.DisplayName
            Key  = $app.PSChildName
            Ran  = $false
            Note = 'No uninstall command registered'
        }
    }

    Write-Host ''
    Write-Host "Uninstalling: $($app.DisplayName) [$($app.PSChildName)]" -ForegroundColor Cyan
    Write-Host "Registered command: $cmd"

    try {
        if ($cmd -match '\{[0-9A-Fa-f\-]{36}\}') {
            $productCode = $Matches[0]
            $args = "/x $productCode /qn /norestart"
            $p = Start-Process -FilePath msiexec.exe -ArgumentList $args -Wait -PassThru -Verb RunAs
            return [pscustomobject]@{
                Name = $app.DisplayName
                Key  = $app.PSChildName
                Ran  = $true
                Note = "msiexec $args exited with code $($p.ExitCode)"
            }
        } else {
            $p = Start-Process -FilePath cmd.exe -ArgumentList "/c $cmd" -Wait -PassThru -Verb RunAs
            return [pscustomobject]@{
                Name = $app.DisplayName
                Key  = $app.PSChildName
                Ran  = $true
                Note = "cmd /c uninstall exited with code $($p.ExitCode)"
            }
        }
    }
    catch {
        return [pscustomobject]@{
            Name = $app.DisplayName
            Key  = $app.PSChildName
            Ran  = $false
            Note = "Failed to run uninstall: $($_.Exception.Message)"
        }
    }
}

Write-Host 'Scanning installed-app registry entries for VideoDedup...' -ForegroundColor Yellow
$before = Get-VideoDedupEntries

if (-not $before -or $before.Count -eq 0) {
    Write-Host 'No VideoDedup entries found.' -ForegroundColor Green
    return
}

Write-Host ''
Write-Host 'Found entries:' -ForegroundColor Yellow
$before | Format-Table DisplayName, DisplayVersion, Publisher, PSChildName -AutoSize

$go = Read-Host 'Run uninstall commands for all listed entries now? (Y/N)'
if ($go -notin @('Y','y')) {
    Write-Host 'Cancelled by user.' -ForegroundColor Yellow
    return
}

$results = foreach ($app in $before) { Invoke-RegisteredUninstall -app $app }

Write-Host ''
Write-Host 'Uninstall attempt results:' -ForegroundColor Yellow
$results | Format-Table Name, Key, Ran, Note -AutoSize

Write-Host ''
Write-Host 'Re-checking for remaining VideoDedup entries...' -ForegroundColor Yellow
$after = Get-VideoDedupEntries

if (-not $after -or $after.Count -eq 0) {
    Write-Host 'Success: no VideoDedup entries remain.' -ForegroundColor Green
    return
}

Write-Host ''
Write-Host 'Remaining entries (likely broken or orphaned):' -ForegroundColor Red
$after | Format-Table DisplayName, DisplayVersion, Publisher, PSChildName -AutoSize

$cleanup = Read-Host 'Backup and remove these remaining registry entries now? (Y/N)'
if ($cleanup -notin @('Y','y')) {
    Write-Host 'Leaving remaining entries untouched.' -ForegroundColor Yellow
    return
}

$backupDir = Join-Path $env:USERPROFILE ("Desktop\VideoDedup_RegistryBackup_" + (Get-Date -Format 'yyyyMMdd_HHmmss'))
New-Item -Path $backupDir -ItemType Directory -Force | Out-Null

foreach ($app in $after) {
    $regPath = Convert-ToRegExePath $app.PSPath
    $safeName = ($app.PSChildName -replace '[^\w\-]', '_')
    $regFile = Join-Path $backupDir ($safeName + '.reg')

    Write-Host ''
    Write-Host "Exporting backup: $regPath -> $regFile" -ForegroundColor Cyan
    & reg.exe export "$regPath" "$regFile" /y | Out-Null

    Write-Host "Removing orphan key: $($app.PSPath)" -ForegroundColor Cyan
    Remove-Item -Path $app.PSPath -Recurse -Force -ErrorAction Stop
}

Write-Host ''
Write-Host "Done. Backups saved in: $backupDir" -ForegroundColor Green
Write-Host 'If Apps & Features still shows stale entries, close and reopen Settings (or reboot once).' -ForegroundColor Green
