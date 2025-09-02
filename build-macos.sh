#!/bin/bash

echo "Building Victory Road for macOS..."

# Ensure PDF templates exist
echo "Checking PDF templates..."
if [ ! -f "VictoryRoad.UI/play-pokemon-deck-list-85x11.pdf" ] || [ ! -f "VictoryRoad.UI/play-pokemon-deck-list-a4.pdf" ]; then
    echo "ERROR: PDF templates not found in VictoryRoad.UI folder"
    echo "Please ensure both play-pokemon-deck-list-85x11.pdf and play-pokemon-deck-list-a4.pdf are present"
    exit 1
fi

# Clean previous builds
rm -rf publish/macos

# Build for macOS x64 (Intel)
dotnet publish VictoryRoad.UI/VictoryRoad.UI.csproj -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish/macos/x64

# Build for macOS ARM64 (Apple Silicon)
dotnet publish VictoryRoad.UI/VictoryRoad.UI.csproj -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish/macos/arm64

# Create app bundles
echo "Creating macOS app bundles..."

# Intel version
mkdir -p "publish/macos/VictoryRoad-Intel.app/Contents/MacOS"
mkdir -p "publish/macos/VictoryRoad-Intel.app/Contents/Resources"
cp publish/macos/x64/VictoryRoad.UI "publish/macos/VictoryRoad-Intel.app/Contents/MacOS/VictoryRoad"
cp publish/macos/x64/play-pokemon-deck-list-85x11.pdf "publish/macos/VictoryRoad-Intel.app/Contents/MacOS/"
cp publish/macos/x64/play-pokemon-deck-list-a4.pdf "publish/macos/VictoryRoad-Intel.app/Contents/MacOS/"
chmod +x "publish/macos/VictoryRoad-Intel.app/Contents/MacOS/VictoryRoad"

# Apple Silicon version
mkdir -p "publish/macos/VictoryRoad-AppleSilicon.app/Contents/MacOS"
mkdir -p "publish/macos/VictoryRoad-AppleSilicon.app/Contents/Resources"
cp publish/macos/arm64/VictoryRoad.UI "publish/macos/VictoryRoad-AppleSilicon.app/Contents/MacOS/VictoryRoad"
cp publish/macos/arm64/play-pokemon-deck-list-85x11.pdf "publish/macos/VictoryRoad-AppleSilicon.app/Contents/MacOS/"
cp publish/macos/arm64/play-pokemon-deck-list-a4.pdf "publish/macos/VictoryRoad-AppleSilicon.app/Contents/MacOS/"
chmod +x "publish/macos/VictoryRoad-AppleSilicon.app/Contents/MacOS/VictoryRoad"

# Create Info.plist for both apps
cat > "publish/macos/VictoryRoad-Intel.app/Contents/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>VictoryRoad</string>
    <key>CFBundleIdentifier</key>
    <string>com.victoryroad.deckbuilder</string>
    <key>CFBundleName</key>
    <string>Victory Road</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0.0</string>
    <key>CFBundleVersion</key>
    <string>1</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
EOF

cp "publish/macos/VictoryRoad-Intel.app/Contents/Info.plist" "publish/macos/VictoryRoad-AppleSilicon.app/Contents/Info.plist"

echo ""
echo "Build complete! Output files:"
echo "- macOS Intel: publish/macos/VictoryRoad-Intel.app"
echo "- macOS Apple Silicon: publish/macos/VictoryRoad-AppleSilicon.app"
echo ""
echo "Note: These apps are not code-signed. Users may need to right-click and select 'Open' the first time."
echo "To distribute via DMG, you can use create-dmg or similar tools."