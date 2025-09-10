# Victory Road - Pokemon TCG Deck List Generator

## Overview

![Victory Road Screenshot](/img/capture.png)

Victory Road is a desktop application designed to help Pokemon Trading Card Game players create and manage their deck lists for official Play! Pokemon tournaments. The application allows players to input their 60-card decks and export them as properly formatted PDF deck lists that comply with tournament requirements.

## Features

- **Manual deck entry** with support for Pokemon, Trainer, and Energy cards
- **Import deck lists** directly from Pokemon TCG Live format
- **Automatic deck validation** ensuring exactly 60 cards
- **PDF export** using official Play! Pokemon deck list templates
- **Support for different divisions** (Junior, Senior, Masters)
- **Support for different formats** (Standard, Expanded)
- **Real-time card count tracking** and validation

## Supported Platforms

- **Windows** (Windows 10 and later)
- **macOS** (macOS 10.14 and later)
- **Linux** (Ubuntu 20.04 and later, other distributions may work)

## Technology Stack

- **C# / .NET 8** - Core application framework
- **Avalonia UI** - Cross-platform desktop UI framework
- **iTextSharp** - PDF generation and manipulation
- **ReactiveUI** - MVVM framework for reactive programming

## Architecture

The application follows a clean architecture pattern with separation of concerns:

- **VictoryRoad.Core** - Business logic, models, and services
- **VictoryRoad.UI** - User interface layer with ViewModels and Views
- **MVVM Pattern** - Model-View-ViewModel architecture for UI separation

## Building and Running

Requires .NET 9 SDK or later. Clone the repository and run:

### Development Build
```bash
dotnet restore
dotnet build
dotnet run --project VictoryRoad.UI
```

### Production Build (All Windows Platforms)
To build standalone executables for all Windows platforms (x64, x86, and ARM64):

```powershell
.\build-windows.ps1
```

This will create optimized executables for:
- **Windows x64**: `publish\windows\x64\VictoryRoad.UI.exe`
- **Windows x86**: `publish\windows\x86\VictoryRoad.UI.exe`
- **Windows ARM64**: `publish\windows\arm64\VictoryRoad.UI.exe` (native for Surface Pro X, etc.)

All executables are self-contained and don't require .NET runtime installation.

## License

This project is open source. Please check the LICENSE file for details.

## Disclaimer

This project is not affiliated with or endorsed by Nintendo or The Pokémon Company.