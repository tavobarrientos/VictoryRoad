# Build script for Victory Road - All Windows platforms
Write-Host "Building Victory Road for Windows..." -ForegroundColor Green
Write-Host ""

# Clean previous builds
if (Test-Path "publish\windows") {
    Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
    Remove-Item -Path "publish\windows" -Recurse -Force
}

# Ensure PDF templates exist
$pdfTemplates = @(
    "VictoryRoad.UI\play-pokemon-deck-list-85x11.pdf",
    "VictoryRoad.UI\play-pokemon-deck-list-a4.pdf"
)

Write-Host "Checking PDF templates..." -ForegroundColor Yellow
foreach ($pdf in $pdfTemplates) {
    if (-not (Test-Path $pdf)) {
        Write-Host "ERROR: PDF template not found: $pdf" -ForegroundColor Red
        Write-Host "Please ensure PDF templates are in the VictoryRoad.UI folder." -ForegroundColor Red
        exit 1
    }
}

# Build for Windows x64
Write-Host "Building for Windows x64..." -ForegroundColor Cyan
dotnet publish VictoryRoad.UI\VictoryRoad.UI.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o publish\windows\x64

# Build for Windows x86
Write-Host "Building for Windows x86..." -ForegroundColor Cyan
dotnet publish VictoryRoad.UI\VictoryRoad.UI.csproj `
    -c Release `
    -r win-x86 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o publish\windows\x86

# Build for Windows ARM64
Write-Host "Building for Windows ARM64..." -ForegroundColor Cyan
dotnet publish VictoryRoad.UI\VictoryRoad.UI.csproj `
    -c Release `
    -r win-arm64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o publish\windows\arm64

# Verify PDF templates are in output directories
Write-Host ""
Write-Host "Verifying PDF templates in output directories..." -ForegroundColor Yellow
$platforms = @("x64", "x86", "arm64")
$allGood = $true

foreach ($platform in $platforms) {
    $outputPath = "publish\windows\$platform"
    foreach ($pdfName in @("play-pokemon-deck-list-85x11.pdf", "play-pokemon-deck-list-a4.pdf")) {
        $pdfPath = Join-Path $outputPath $pdfName
        if (-not (Test-Path $pdfPath)) {
            Write-Host "WARNING: PDF template missing in $platform output: $pdfName" -ForegroundColor Yellow
            $allGood = $false
        }
    }
}

if ($allGood) {
    Write-Host "All PDF templates successfully included!" -ForegroundColor Green
}

Write-Host ""
Write-Host "Build complete! Output files:" -ForegroundColor Green
Write-Host "- Windows x64: publish\windows\x64\VictoryRoad.UI.exe" -ForegroundColor White
Write-Host "- Windows x86: publish\windows\x86\VictoryRoad.UI.exe" -ForegroundColor White
Write-Host "- Windows ARM64: publish\windows\arm64\VictoryRoad.UI.exe" -ForegroundColor White
Write-Host ""
Write-Host "These are standalone executables that can be distributed without requiring .NET runtime installation." -ForegroundColor Gray
Write-Host "PDF templates are included alongside the executables." -ForegroundColor Gray
Write-Host ""
Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")