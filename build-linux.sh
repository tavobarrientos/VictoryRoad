#!/bin/bash

# Victory Road Build Script for Linux
# Repository: https://github.com/tavobarrientos/VictoryRoad

echo "Building Victory Road for Linux..."
echo "Repository: https://github.com/tavobarrientos/VictoryRoad"
echo ""

# Ensure PDF templates exist
echo "Checking PDF templates..."
if [ ! -f "VictoryRoad.UI/play-pokemon-deck-list-85x11.pdf" ] || [ ! -f "VictoryRoad.UI/play-pokemon-deck-list-a4.pdf" ]; then
    echo "ERROR: PDF templates not found in VictoryRoad.UI folder"
    echo "Please ensure both play-pokemon-deck-list-85x11.pdf and play-pokemon-deck-list-a4.pdf are present"
    exit 1
fi

# Clean previous builds
rm -rf publish/linux

# Build for Linux x64
dotnet publish VictoryRoad.UI/VictoryRoad.UI.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish/linux/x64

# Build for Linux ARM
dotnet publish VictoryRoad.UI/VictoryRoad.UI.csproj -c Release -r linux-arm --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish/linux/arm

# Build for Linux ARM64
dotnet publish VictoryRoad.UI/VictoryRoad.UI.csproj -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish/linux/arm64

# Make executables
chmod +x publish/linux/x64/VictoryRoad.UI
chmod +x publish/linux/arm/VictoryRoad.UI
chmod +x publish/linux/arm64/VictoryRoad.UI

# Create AppImage for x64 (most common)
echo "Creating AppImage..."
mkdir -p AppDir/usr/bin
mkdir -p AppDir/usr/share/applications
mkdir -p AppDir/usr/share/icons/hicolor/256x256/apps

cp publish/linux/x64/VictoryRoad.UI AppDir/usr/bin/
cp publish/linux/x64/play-pokemon-deck-list-85x11.pdf AppDir/usr/bin/
cp publish/linux/x64/play-pokemon-deck-list-a4.pdf AppDir/usr/bin/

# Create desktop file
cat > AppDir/usr/share/applications/victoryroad.desktop << EOF
[Desktop Entry]
Name=Victory Road
Comment=Pokemon TCG Deck List Generator
Exec=VictoryRoad.UI
Icon=victoryroad
Type=Application
Categories=Game;CardGame;
X-AppImage-Version=1.0.0
X-AppImage-Arch=x86_64
X-AppImage-Name=Victory Road
X-AppImage-HomePage=https://github.com/tavobarrientos/VictoryRoad
EOF

# Create a simple icon (you can replace this with a proper icon later)
cat > AppDir/usr/share/icons/hicolor/256x256/apps/victoryroad.png << EOF
# Placeholder for icon - replace with actual PNG icon
EOF

echo ""
echo "Build complete! Output files:"
echo "- Linux x64: publish/linux/x64/VictoryRoad.UI"
echo "- Linux ARM: publish/linux/arm/VictoryRoad.UI"
echo "- Linux ARM64: publish/linux/arm64/VictoryRoad.UI"
echo ""
echo "These are standalone executables that can be run directly."
echo "To create an AppImage, use appimagetool with the AppDir directory."