Markdown

# VxPresence

[![Downloads](https://img.shields.io/github/downloads/444vx/vxpresence/total?style=for-the-badge&color=blue)](https://github.com/444vx/vxpresence/releases)

A high-performance C# / WPF Discord Rich Presence Engine for Windows.

## Purpose and Core Philosophy

VxPresence was developed as a lightweight alternative to resource-intensive Rich Presence solutions that rely on browser extensions or external frameworks.

The core design principles are:
- Native OS Execution: Interacts directly with the Windows API (user32.dll) to resolve active processes without code injection.
- Privacy First: Granular user control over visible data, including application titles, active tabs, and telemetry.
- Resource Efficiency: Minimal CPU and RAM footprint designed to run continuously in the background.

## Key Features

- Automated Process Detection: Dynamically identifies active applications and cleans up window title strings.
- Media Contextualization: Automatically categorizes active media consumption across supported desktop applications and web platforms.
- Idle State Detection: Uses system-level idle monitoring via GetLastInputInfo to switch status to Away From Keyboard (AFK) after 5 minutes of inactivity.
- System Telemetry: Optional real-time reporting of active CPU and RAM utilization.
- Control Panel: Clean graphical interface providing toggles for system startup, telemetry display, and tab visibility.

## Technical Architecture

The project is structured into modular components:

```text
VxPresence/
├── Inspectors/         # Process classification and window title formatting
├── Native/             # Win32 API Interop (GetForegroundWindow, GetLastInputInfo)
├── Services/           # Discord RPC client wrapper
├── Telemetry/          # Hardware performance monitoring
├── App.xaml / .cs     # Application entry point
└── MainWindow.xaml     # Engine loop and user interface controls

Setup and Installation
Prerequisites

    Windows 10 or Windows 11 (64-bit)

    Discord Desktop Application

Building from Source

    Clone the repository:
    Bash

    git clone [https://github.com/444vx/vxpresence.git](https://github.com/444vx/vxpresence.git)
    cd vxpresence

    Set your Discord Application ID in MainWindow.xaml.cs:
    C#

    private const string DISCORD_CLIENT_ID = "";

    Build and execute:
    Bash

    dotnet build
    dotnet run

Configuration

Available application toggles:
Option	Function
Start with Windows	Writes execution path to current user registry startup.
Hardware Telemetry	Appends CPU and RAM usage to the Discord presence status.
Browser Tab Titles	Includes active browser tab titles in presence details.
Background Apps	Displays open desktop applications when the system desktop is focused.
License

This project is licensed under the MIT License.
