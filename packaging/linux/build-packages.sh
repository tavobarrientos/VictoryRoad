#!/bin/bash
# Local script to build Linux packages for all distributions

set -e

VERSION=${1:-"1.0.0"}
ARCH=$(uname -m)

# Map architecture names
if [ "$ARCH" = "x86_64" ]; then
    RUNTIME="linux-x64"
    DEB_ARCH="amd64"
    RPM_ARCH="x86_64"
elif [ "$ARCH" = "aarch64" ]; then
    RUNTIME="linux-arm64"
    DEB_ARCH="arm64"
    RPM_ARCH="aarch64"
else
    echo "Unsupported architecture: $ARCH"
    exit 1
fi

echo "Building Victory Road packages v${VERSION} for ${ARCH}..."

# Clean and create working directory
rm -rf build-tmp
mkdir -p build-tmp
cd build-tmp

# Build the application
echo "Building application..."
dotnet publish ../../../VictoryRoad.UI/VictoryRoad.UI.csproj \
    -c Release \
    -r ${RUNTIME} \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -o app

# Make executable
chmod +x app/VictoryRoad.UI

# Create package structure
echo "Creating package structure..."
mkdir -p package/usr/bin
mkdir -p package/usr/share/applications
mkdir -p package/usr/share/doc/victoryroad
mkdir -p package/usr/share/icons/hicolor/256x256/apps

# Copy files
cp app/VictoryRoad.UI package/usr/bin/victoryroad
cp app/*.pdf package/usr/bin/ 2>/dev/null || true

# Create desktop file
cat > package/usr/share/applications/victoryroad.desktop << EOF
[Desktop Entry]
Name=Victory Road
Comment=Pokemon TCG Deck List Generator
Exec=/usr/bin/victoryroad
Icon=victoryroad
Type=Application
Categories=Game;CardGame;Utility;
Terminal=false
EOF

# Create README
cat > package/usr/share/doc/victoryroad/README << EOF
Victory Road - Pokemon TCG Deck List Generator
Version: ${VERSION}

A desktop application for creating and managing Pokemon TCG deck lists
for official Play! Pokemon tournaments.

Homepage: https://github.com/tavobarrientos/VictoryRoad
License: MIT
EOF

# Build DEB package
echo "Building DEB package..."
mkdir -p package/DEBIAN
cat > package/DEBIAN/control << EOF
Package: victoryroad
Version: ${VERSION}
Section: games
Priority: optional
Architecture: ${DEB_ARCH}
Maintainer: Victory Road Team <support@victoryroad.app>
Description: Pokemon TCG Deck List Generator
 Victory Road is a desktop application for creating and managing
 Pokemon Trading Card Game deck lists for official tournaments.
 Supports manual deck entry, import from TCG Live, and PDF export.
Depends: libicu-dev
Homepage: https://github.com/tavobarrientos/VictoryRoad
EOF

# Add post-install script
cat > package/DEBIAN/postinst << 'EOF'
#!/bin/sh
set -e
# Update desktop database
if which update-desktop-database >/dev/null 2>&1; then
    update-desktop-database -q /usr/share/applications
fi
exit 0
EOF
chmod 755 package/DEBIAN/postinst

dpkg-deb --build package ../victoryroad_${VERSION}_${DEB_ARCH}.deb
echo "Created: victoryroad_${VERSION}_${DEB_ARCH}.deb"

# Build RPM package (if rpmbuild is available)
if command -v rpmbuild &> /dev/null; then
    echo "Building RPM package..."
    
    # Create RPM build structure
    mkdir -p ~/rpmbuild/{BUILD,RPMS,SOURCES,SPECS,SRPMS}
    
    # Create tarball
    tar czf ~/rpmbuild/SOURCES/victoryroad-${VERSION}.tar.gz -C package .
    
    # Create spec file
    cat > ~/rpmbuild/SPECS/victoryroad.spec << EOF
Name:           victoryroad
Version:        ${VERSION}
Release:        1%{?dist}
Summary:        Pokemon TCG Deck List Generator
License:        MIT
URL:            https://github.com/tavobarrientos/VictoryRoad
Source0:        victoryroad-${VERSION}.tar.gz

BuildArch:      ${RPM_ARCH}
Requires:       libicu

%description
Victory Road is a desktop application for creating and managing
Pokemon Trading Card Game deck lists for official tournaments.
Supports manual deck entry, import from TCG Live, and PDF export.

%prep
%setup -q -c

%install
mkdir -p %{buildroot}
cp -r * %{buildroot}/

%files
%defattr(-,root,root,-)
/usr/bin/victoryroad
/usr/bin/*.pdf
/usr/share/applications/victoryroad.desktop
/usr/share/doc/victoryroad/README

%post
# Update desktop database
if [ -x /usr/bin/update-desktop-database ]; then
    /usr/bin/update-desktop-database -q /usr/share/applications 2>/dev/null || :
fi

%postun
# Update desktop database
if [ -x /usr/bin/update-desktop-database ]; then
    /usr/bin/update-desktop-database -q /usr/share/applications 2>/dev/null || :
fi

%changelog
* $(date '+%a %b %d %Y') Victory Road Team <support@victoryroad.app> - ${VERSION}-1
- Release version ${VERSION}
EOF
    
    rpmbuild -bb ~/rpmbuild/SPECS/victoryroad.spec
    cp ~/rpmbuild/RPMS/*/*.rpm ../victoryroad-${VERSION}-${RPM_ARCH}.rpm
    echo "Created: victoryroad-${VERSION}-${RPM_ARCH}.rpm"
else
    echo "rpmbuild not found, skipping RPM package"
fi

# Create Arch Linux PKGBUILD
echo "Creating Arch Linux PKGBUILD..."
mkdir -p arch
cat > arch/PKGBUILD << EOF
# Maintainer: Victory Road Team <support@victoryroad.app>
pkgname=victoryroad
pkgver=${VERSION}
pkgrel=1
pkgdesc="Pokemon TCG Deck List Generator"
arch=('${ARCH}')
url="https://github.com/tavobarrientos/VictoryRoad"
license=('MIT')
depends=('icu')
source=("victoryroad-\${pkgver}-${RUNTIME}.tar.gz")
sha256sums=('SKIP')

package() {
    cd "\${srcdir}"
    
    # Install binary
    install -Dm755 VictoryRoad.UI "\${pkgdir}/usr/bin/victoryroad"
    
    # Install PDF templates
    install -Dm644 *.pdf -t "\${pkgdir}/usr/share/victoryroad/"
    
    # Install desktop file
    install -Dm644 victoryroad.desktop "\${pkgdir}/usr/share/applications/victoryroad.desktop"
}
EOF

# Create desktop file for Arch package
cat > arch/victoryroad.desktop << EOF
[Desktop Entry]
Name=Victory Road
Comment=Pokemon TCG Deck List Generator
Exec=/usr/bin/victoryroad
Icon=victoryroad
Type=Application
Categories=Game;CardGame;Utility;
Terminal=false
EOF

# Create tarball for Arch
tar czf ../victoryroad-${VERSION}-${RUNTIME}.tar.gz -C app .
cp arch/PKGBUILD ../PKGBUILD-${ARCH}
echo "Created: PKGBUILD-${ARCH} and victoryroad-${VERSION}-${RUNTIME}.tar.gz"

# Create generic tarball
echo "Creating generic tarball..."
cd app
tar czf ../../victoryroad-${VERSION}-${RUNTIME}-generic.tar.gz *
cd ..
echo "Created: victoryroad-${VERSION}-${RUNTIME}-generic.tar.gz"

# Clean up
cd ..
rm -rf build-tmp

echo ""
echo "Package building complete!"
echo ""
echo "Installation instructions:"
echo ""
echo "Debian/Ubuntu:"
echo "  sudo dpkg -i victoryroad_${VERSION}_${DEB_ARCH}.deb"
echo "  sudo apt-get install -f  # If dependencies are missing"
echo ""
if command -v rpmbuild &> /dev/null; then
    echo "Fedora/RHEL/openSUSE:"
    echo "  sudo rpm -i victoryroad-${VERSION}-${RPM_ARCH}.rpm"
    echo "  # Or: sudo yum install victoryroad-${VERSION}-${RPM_ARCH}.rpm"
    echo "  # Or: sudo dnf install victoryroad-${VERSION}-${RPM_ARCH}.rpm"
    echo "  # Or: sudo zypper install victoryroad-${VERSION}-${RPM_ARCH}.rpm"
    echo ""
fi
echo "Arch Linux:"
echo "  # Copy PKGBUILD-${ARCH} to a build directory as PKGBUILD"
echo "  # Copy victoryroad-${VERSION}-${RUNTIME}.tar.gz to the same directory"
echo "  makepkg -si"
echo ""
echo "Generic Linux:"
echo "  tar xzf victoryroad-${VERSION}-${RUNTIME}-generic.tar.gz"
echo "  ./VictoryRoad.UI"