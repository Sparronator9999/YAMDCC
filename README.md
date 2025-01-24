# YAMDCC - Yet Another MSI (Dragon) Center Clone

A fast, lightweight MSI Center alternative and fan control utility for MSI laptops.

**Please read the whole README (or at least the [Supported Laptops](#supported-laptops) and [FAQ](#faq) sections) before downloading.**

<details><summary>Table of contents <i>(click to expand</i>)</summary>

- [Disclaimers](#disclaimers)
- [Features](#features)
- [Screenshots](#screenshots)
- [Supported laptops](#supported-laptops)
  - [Community-tested laptops](#community-tested-laptops)
- [Comparison](#comparison)
- [Roadmap](#roadmap)
- [Download](#download)
- [Compile](#compile)
- [Issues](#issues)
- [Contributing](#contributing)
- [FAQ](#faq)
  - [What versions of Windows do you support?](#what-versions-of-windows-do-you-support)
  - [Can you please make a Linux version?](#can-you-please-make-a-linux-version)
  - [How does this program work?](#how-does-this-program-work)
  - [Why do I need administrator privileges to run this program?](#why-do-i-need-administrator-privileges-to-run-this-program)
  - [Why does this program need a kernel driver?](#why-does-this-program-need-a-kernel-driver)
  - [Doesn't WinRing0 have security issues?](#doesnt-winring0-have-security-issues)
  - [Help! My fan profiles aren't being applied!](#help-my-fan-profiles-arent-being-applied)
  - [My laptop isn't supported! What do I do?](#my-laptop-isnt-supported-what-do-i-do)
  - [Can you write a config for my laptop?](#can-you-write-a-config-for-my-laptop)
  - [Help! My laptop stopped booting/is doing weird stuff!](#help-my-laptop-stopped-bootingis-doing-weird-stuff)
  - [Dark mode?](#dark-mode)
  - [Why are you still using WinForms in 2024?](#why-are-you-still-using-winforms-in-2024)
  - [.NET (Core) 5/6/8/<insert latest .NET version>!](#net-core-568insert-latest-net-version)
- [License and copyright](#license-and-copyright)
- [Third-party libraries](#third-party-libraries)
</details>

## Disclaimers

- While this program is mostly complete, it is still **beta-quality software!**
- You may still encounter some bugs while using this program, although these should be rare.
- This program requires low-level access to some of your computer hardware to apply settings. While
  no issues should arise from this, **I (Sparronator9999) and any other contributers**
  **shall not be held responsible if this program fries your computer.**
- Additionally, if you do something silly with the program like turn off all your fans while
  running under full load, **we *will not* be held responsible for *any* damage you cause to your**
  **own hardware from your use of this program.**
- Linux is not yet supported. Please don't beg me for Linux support, it will come when I can be
  bothered (and when I figure out how to run background services/daemons on Linux).
- This program, repository and its authors are not affiliated with Micro-Star International Co., Ltd. in any way, shape, or form.

## Features

- **Fan control:** Change the fan curves for your CPU and GPU fans, including fan speeds,
  temperature thresholds, and Full Blast (a.k.a. Cooler Boost in MSI Center). This allows you to
  fix a curve that is not aggressive enough under full load, or to turn your fans off when your
  computer is idle.
- **Performance mode:** MSI laptops have their own performance mode setting (not to be confused
  with Windows' built-in power plans). You can change it here.
- **Charging threshold:** This program can limit how much your laptop's battery charges to, which
  can help reduce battery degradation, especially if you leave your laptop plugged in all the time.
- **Lightweight:** YAMDCC takes up less than two megabytes of disk space when installed, and is
  designed to be light on your laptop's CPU.
- **Configurable:** Almost all settings (including those not accessible through the config editor)
  can be changed with the power of XML.

## Screenshots

![Screenshot of the config editor's fan control tab](Media/YAMDCC-FanControlTab.png)

![Screenshot of the config editor's extra options tab](Media/YAMDCC-ExtrasTab.png)

## Supported Laptops

Currently, there are configs for the following laptops:

  - MSI GF63 Thin 11SC
  - MSI Modern 15 A5M (thanks @tedomi2705)
  - MSI Katana GF66 12UG (thanks @porkmanager)
  - MSI Crosshair 17 B12UGZ (thanks @ios7jbpro)

There are also generic configs that should work with most MSI laptops, but with an incorrect default
config. You can use the EC-to-config feature to get the proper fan curves for your laptop, then
[create a pull request](https://github.com/Sparronator9999/YAMDCC/pulls) to get your laptop's
config added to the project.

Other laptop brands are not officially supported. You can still try and
[make your own config](https://github.com/Sparronator9999/YAMDCC/wiki/How-to-make-a-config-for-YAMDCC#manually-from-scratch),
but chances are you're looking for [NoteBook FanControl](https://github.com/UraniumDonut/nbfc-revive) instead.

### Community-tested laptops

The following laptops have been tested by the community and are confirmed to be working, but don't
have their own public YAMDCC configs. A suggested generic config is provided below:

- MSI Vector GP78 HX 13V (thanks @Twisted6): `MSI-10th-gen-or-newer-dualfan.xml`
- MSI Raider GE66 12UGS (thanks @grimy400): `MSI-10th-gen-or-newer-dualfan.xml`
- MSI Vector 17 HX A14VHG (thanks @injitools): `MSI-10th-gen-or-newer-dualfan-nokeylight.xml`

To test your laptop, go to the [config tutorial](https://github.com/Sparronator9999/YAMDCC/wiki/How-to-make-a-config-for-YAMDCC)
wiki page and follow the instructions to get a config for your laptop.

## Comparison

| Feature                         | MSI Center | YAMDCC      |
|---------------------------------|------------|-------------|
| Installed size                  | ~950 MB²   | ~2.5 MB²    |
| Fan control                     | ✔          | ✔           |
| Temp. threshold control         | ❌          | ✔           |
| Multi-fan profile support       | ❌          | ✔           |
| Charge threshold setting        | Limited³   | ✔           |
| Perf. mode setting              | ✔          | ✔           |
| Win/Fn key swap¹                | ✔          | ✔           |
| Win key disable                 | ✔          | ❌           |
| Keyboard backlight adjustment¹  | ❌          | ✔           |
| Hardware monitoring             | ✔          | Limited⁴    |
| Other MSI Center features       | ✔          | ❌           |
| Open source                     | ❌          | ✔           |

1: Support for this feature depends on the specific MSI laptop model and YAMDCC support.

2: As of v2.0.38, MSI Center takes about 950 MB of storage space when counting the UWP app (749 MB)
and the files installed on first launch to `C:\Program Files (x86)\MSI` (205 MB). YAMDCC's installed
size is based on the Release build of [v1.0 Beta 7](https://github.com/Sparronator9999/YAMDCC/releases/tag/v1.0.0-beta.7),
and includes all unzipped program files and included config XMLs.

3: MSI Center only supports setting the charge threshold to 60%, 80%, or 100%, while YAMDCC can set
this to anything between 0 and 100% (with 0 meaning charge to 100% always).

4: YAMDCC only supports monitoring the CPU/GPU temperatures and fan speeds via EC.

## Roadmap

Below are some changes I would like to make before a 1.0 release of YAMDCC:

- [ ] Fix any remaining bugs before the 1.0 release.
  - Beta releases are currently available for v1.0. Please download them from
    [Releases](https://github.com/Sparronator9999/YAMDCC/releases) and report any problems to
    [the issue tracker](https://github.com/Sparronator9999/YAMDCC/issues)

The following features are currently planned for v1.1:

- [ ] Hotkey support (requested by @grimy400)
  - This will most likely be implemented as a separate background program that listens to keyboard shortcuts,
    since Windows services (or at least those running as Local System) can't "see" keyboard input.
- [ ] Switch fan profiles on performance mode setting adjustment ([#37](https://github.com/Sparronator9999/YAMDCC/issues/37))

Below are some planned features for potential future releases:

- [ ] CLI support
  - Development of a CLI application for YAMDCC has started, but isn't publicly available yet
    - The CLI is missing a *lot* of features, and could do with a rewrite.
- [ ] Support for editing laptop config registers using the GUI/CLI
  - This would allow for creating configs for other laptop brands from the config UI
  - Currently, the only way to do this is to edit the XML directly or use the EC-to-config feature.
- [ ] Plugin system for additional optional features *(needs research)*
- [ ] .NET support
  - Mandatory for Linux support
  - The GUI *should* compile on .NET 8 (and in fact *has* been compiled on .NET 8 before).
  - The Windows service on the other hand... is going to be interesting. Even
    with the `Microsoft.Windows.Compatibility` package installed, I still
    wasn't able to get the service to run without issues.
- [ ] Linux support *(not guaranteed)*
  - Now this would require some figuring out, and may end up being a seperate
    project that's compatible with this project's configs.

## Download

Beta builds are available from [GitHub releases](https://github.com/Sparronator9999/YAMDCC/releases).

Development builds are available through [GitHub Actions](https://github.com/Sparronator9999/YAMDCC/actions).

If you don't have a GitHub account, you can also download the latest development build from
[nightly.link](https://nightly.link/Sparronator9999/YAMDCC/workflows/build/main?preview).

You're probably looking for the `Release` build, unless you're debugging issues with the program.

Alternatively, you can [build the program yourself](#compile).

## Compile

See also the [wiki page](https://github.com/Sparronator9999/YAMDCC/wiki/Building).

### Using Visual Studio

1.  Install Visual Studio 2022 with the `.NET Desktop Development` workload checked.
2.  Download the code repository, or clone it with `git`.
3.  Extract the downloaded code, if needed.
4.  Open `YAMDCC.sln` in Visual Studio.
5.  Click `Build` > `Build Solution` to build everything.
6.  Your output, assuming default build settings, is located in `YAMDCC.GUI\bin\Debug\net48\`.
7.  ???
8.  Profit!

Make sure to only use matching `yamdccsvc.exe` and `YAMDCC.exe` together, otherwise you
may encounter issues (that means `net stop yamdccsvc` first, then compile).

### From the command line

1.  Follow steps 1-3 above to install Visual Studio and download the code.
2.  Open `Developer Command Prompt for VS 2022` and `cd` to your project directory.
3.  Run `msbuild /t:restore` to restore the solution, including NuGet packages.
4.  Run `msbuild YAMDCC.sln /p:platform="Any CPU" /p:configuration="Debug"` to build
    the project, substituting `Debug` with `Release` (or `Any CPU` with `x86` or `x64`)
    if you want a release build instead.
5.  Your output should be located in `YAMDCC.GUI\bin\Debug\net48\`, assuming you built
    with the above unmodified command.
6.  ???
7.  Profit!

## Issues

If your question isn't already answered in the [FAQ](#faq) or [issues megathread](https://github.com/Sparronator9999/YAMDCC/issues/1),
and there isn't already another similar issue in [the issue tracker](https://github.com/Sparronator9999/YAMDCC/issues),
feel free to open an issue. Please make sure to use the correct issue template for your problem.

## Contributing

See the [compile instructions](#compile) to build this project.

If you would like to contribute to the project with bug fixes, new features,
or new configs, feel free to open a pull request. Please include the following:

- **Bug fixes/improvements/new feature:** Describe the changes you made and why they
  are important or useful.
- **New config:** Add a config with your laptop's default fan profile so that
  other people don't have to run the EC-to-config tool.

## FAQ

### What versions of Windows do you support?

Windows 10 and 11 (64-bit).

32-bit Windows 10 should also work, but you really should be using 64-bit Windows in 2024.

Older versions of Windows that support .NET Framework 4.8 may also work, but with no support from me.

### Can you please make a Linux version?

Soon™.

Use one of the [many](https://github.com/dmitry-s93/MControlCenter) [other](https://github.com/gourav1100/isw)
[projects](https://github.com/YoCodingMonster/OpenFreezeCenter) on GitHub instead while you wait.

### How does this program work?

YAMDCC works by accessing your laptop's embedded controller (aka, the EC). Many settings that
can be configured with MSI Center are stored here, including fan curve, performance mode,
and the Win/Fn key swap setting.

### Why do I need administrator privileges to run this program?

1. Because admin privileges are required to install kernel drivers, and...

2. For security reasons, only programs with admin privileges are allowed to communicate with the
YAMDCC service once it's running.

### Why does this program need a kernel driver?

Because communicating with the EC requires low-level hardware access, something only possible from
within the kernel. This program achieves this with [WinRing0](https://github.com/GermanAizek/WinRing0).

### Doesn't WinRing0 have security issues?

[Yes](https://voidsec.com/crucial-mod-utility-lpe-cve-2021-41285/), however YAMDCC mitigates this
by installing the driver such that only programs run with administrator privileges can communicate
with the driver.

Why wasn't this done by the driver itself in the first place, you might ask? Honestly, I don't know
how this slipped through during development, but here we are. Unfortunately the
[updated fork of WinRing0](https://github.com/GermanAizek/WinRing0) that *does* fix this
vulnerability driver-side doesn't have a binary release due to Microsoft's strict driver signing
requirements.

NOTE:
If YAMDCC finds the driver already installed, it may try to use that (potentially vulnerable)
driver instead. If it was installed with, e.g. [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor),
you should be fine, as they implement the same workaround that YAMDCC does.

Please read the [disclaimer](#disclaimers), especially the bold text, if you haven't already.

### Help! My fan profiles aren't being applied!

You may need to disable virtualisation-based security (VBS). See
[this page](https://www.thewindowsclub.com/disable-vbs-windows) for more info,
but generally this boils down to disabling a Windows Defender feature (Memory
integrity), and/or uninstalling virtualisation features (Hyper-V, WSL, etc.)

A warning may be added in the future to YAMDCC if VBS is enabled once more
information is available on what settings break YAMDCC (and related software,
e.g. ThrottleStop)

If the above steps don't fix your problem, search the [issue tracker](https://github.com/Sparronator9999/YAMDCC/issues)
for similar issues, or [open a new issue](https://github.com/Sparronator9999/YAMDCC/issues/new)
if a similar issue doesn't already exist (see the [Issues section](#issues) of the README).

### My laptop isn't supported! What do I do?

[See above](#supported-laptops).

### Can you write a config for my laptop?

If it's an MSI laptop, maybe. All MSI laptops (or at least those I've come across in bug reports)
share the same (or very similar) EC register layout, and are documented to varying degrees in
similar open-source projects. I may be able to assist you if the generic configs aren't working as
expected.

For other laptop brands, you're on your own.

### Help! My laptop stopped booting/is doing weird stuff!

MSI laptop users: reset your EC:

Shut down the laptop if it's on (force shut down if needed), then find the EC reset button
(on the GF63 Thin 11SC, it's a small hole located on the bottom of the laptop next to the charge port)
and press it with the end of a paperclip, SIM ejector, or similarly small tool for at least 5 seconds,
then try rebooting.

You can also try holding the power button for 60 seconds. The power LED will flash, and the EC/BIOS should reset.

If the issue persists, try disconnecting all power sources, including the laptop's CMOS/clock
battery and "main" battery (requires disassembly of laptop). Leave disconnected for a few seconds,
then re-connect everything, re-assemble and attempt a reboot. This will reset your BIOS settings.

Users of other laptop brands will need to look up instructions for their laptop to reset BIOS settings.

### Dark mode?

Due to WinForms limitations, no.

Technical explanation: A few specific WinForms controls used by YAMDCC look really bad
when trying to recolour them to be dark themed. Also, built-in dialog boxes (for C# programmers,
think `MessageBox.Show`) cannot be recoloured from their default white theme. Also, I have little
to no experience with other UI kits (e.g. WPF).

### Why are you still using WinForms in 2024?

Because it's what I know (thanks, my high school programming classes).

I've tried WPF before, but without much success (although I might look into it more once I find a
suitable replacement for some WinForms controls missing in WPF, and get some more WPF experience).

### .NET (Core) 5/6/8/<insert latest .NET version>!

Probably not for Windows (unless it goes EOL, which I doubt will happen for a while).

If Linux support ever comes, it will be using .NET (since .NET Framework isn't supported on Linux).

## License and Copyright

Copyright © 2023-2025 Sparronator9999.

This program is free software: you can redistribute it and/or modify it under
the terms of the GNU General Public License as published by the Free Software
Foundation, either version 3 of the License, or (at your option) any later
version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE. See the [GNU General Public License](LICENSE.md) for more
details.

## Third-party Libraries

This project makes use of the following third-party libraries:

- [Json.NET (Newtonsoft.Json)](https://www.newtonsoft.com/json) to parse obtained release manifests from GitHub.
- [Marked .NET](https://github.com/tomlm/MarkedNet) to parse release changelogs.
- [My fork of Named Pipe Wrapper](https://github.com/Sparronator9999/NamedPipeWrapper) for
  communication between the service and UI program (called `YAMDCC.IPC` in the source files).
- [Task Scheduler Managed Wrapper](https://github.com/dahall/taskscheduler) to schedule automatic update checks.
- [WinRing0](https://github.com/QCute/WinRing0) for low-level hardware access required to
  read/write the EC.
