$ErrorActionPreference = "Stop"

if ((Get-Command "docker-compose.exe" -ErrorAction SilentlyContinue) -eq $null) { 
   Write-Host -Object "Docker is not installed. Please install docker before running again" -ForegroundColor "Red"
   exit 1
}

$cmdPath = "docker-compose.exe"
$cmdArgList = @(
    "up"
)
& $cmdPath $cmdArgList