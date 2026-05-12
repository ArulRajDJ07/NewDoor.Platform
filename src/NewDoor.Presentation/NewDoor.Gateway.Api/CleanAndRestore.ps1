# PowerShell script to clean and restore NewDoor.Gateway.Api and dependencies
# This fixes the "Assets file doesn't have a target for 'net8.0/win-x64'" error

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "NewDoor.Gateway.Api Clean & Rebuild" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootPath = Resolve-Path (Join-Path $scriptPath "..\..\..") | Select-Object -ExpandProperty Path

Write-Host "Workspace root: $rootPath`n" -ForegroundColor Yellow

# Define project paths
$eventBusPath = Join-Path $rootPath "NewDoor.StreamingRuntime\NewDoor.EventBus"
$gatewayApiPath = Join-Path $rootPath "NewDoor.Presentation\NewDoor.Gateway.Api"

# Function to clean project
function Clean-Project {
    param($projectPath, $projectName)

    Write-Host "Cleaning $projectName..." -ForegroundColor Cyan

    $objPath = Join-Path $projectPath "obj"
    $binPath = Join-Path $projectPath "bin"

    if (Test-Path $objPath) {
        Remove-Item $objPath -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "  ✓ Removed obj folder" -ForegroundColor Green
    }

    if (Test-Path $binPath) {
        Remove-Item $binPath -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "  ✓ Removed bin folder" -ForegroundColor Green
    }
}

# Clean projects in dependency order
Write-Host "`nStep 1: Cleaning projects..." -ForegroundColor Yellow
Write-Host "─────────────────────────────`n" -ForegroundColor Yellow

Clean-Project -projectPath $eventBusPath -projectName "NewDoor.EventBus"
Clean-Project -projectPath $gatewayApiPath -projectName "NewDoor.Gateway.Api"

# Restore and build EventBus first (dependency)
Write-Host "`nStep 2: Restoring NewDoor.EventBus..." -ForegroundColor Yellow
Write-Host "─────────────────────────────────────`n" -ForegroundColor Yellow

$eventBusCsproj = Join-Path $eventBusPath "NewDoor.EventBus.csproj"
dotnet restore $eventBusCsproj

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ EventBus restore successful`n" -ForegroundColor Green

    Write-Host "Step 3: Building NewDoor.EventBus (Release/win-x64)..." -ForegroundColor Yellow
    Write-Host "───────────────────────────────────────────────────────`n" -ForegroundColor Yellow

    dotnet build $eventBusCsproj -c Release

    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ EventBus build successful`n" -ForegroundColor Green
    } else {
        Write-Host "  ✗ EventBus build failed`n" -ForegroundColor Red
        Write-Host "Press any key to exit..."
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        exit 1
    }
} else {
    Write-Host "  ✗ EventBus restore failed`n" -ForegroundColor Red
    Write-Host "Press any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

# Restore and build Gateway.Api
Write-Host "Step 4: Restoring NewDoor.Gateway.Api..." -ForegroundColor Yellow
Write-Host "────────────────────────────────────────`n" -ForegroundColor Yellow

$gatewayApiCsproj = Join-Path $gatewayApiPath "NewDoor.Gateway.Api.csproj"
dotnet restore $gatewayApiCsproj

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ Gateway.Api restore successful`n" -ForegroundColor Green

    Write-Host "Step 5: Building NewDoor.Gateway.Api (Release)..." -ForegroundColor Yellow
    Write-Host "──────────────────────────────────────────────────`n" -ForegroundColor Yellow

    dotnet build $gatewayApiCsproj -c Release

    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ Gateway.Api build successful`n" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "SUCCESS! All projects built successfully" -ForegroundColor Green
        Write-Host "========================================`n" -ForegroundColor Green
        Write-Host "You can now publish the project from Visual Studio.`n" -ForegroundColor Cyan
    } else {
        Write-Host "  ✗ Gateway.Api build failed`n" -ForegroundColor Red
    }
} else {
    Write-Host "  ✗ Gateway.Api restore failed`n" -ForegroundColor Red
}

Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
