# YAMDCC - Yet Another MSI (Dragon) Center Clone

*(formerly known as MSI Fan Control)*

A fast, lightweight alternative to MSI Center for MSI laptops, written in C#.

**Please read the whole README (or at least the [Supported Laptops](#supported-laptops) and [FAQ](#faq) sections) before downloading.**

## Disclaimers

- While this program is mostly complete, I still consider it to be **alpha-quality software!**
- This program is held together by a lot of metaphorical duct-tape. You *will* experience bugs
  and/or crashes when using it (especially if feeding it any config other than the one provided)!
- This program requires low-level access to some of your computer hardware to apply settings.
  While no issues should arise from the use of this program, **I (Sparronator9999) and any other**
  **contributers shall not be held responsible for any damage to your laptop that result from**
  **your use of this program.**
- Linux is not yet supported. I will update it for Linux once I figure out how to run services/background processes on Linux.
- This program, repository and its authors are not affiliated with Micro-Star International Co., Ltd. in any way, shape, or form.

## Features

- **Fan control:** Change the fan curves for your CPU and GPU fans, including fan speeds, temperature
  thresholds, and Full Blast (a.k.a. Cooler Boost in MSI Center).
- **Performance mode:** MSI laptops have their own performance mode setting (not to be confused
  with Windows' built-in power plans). You can change it here.
- **Charging threshold:** MSI laptops come with the ability to limit the battery charge percentage,
  which can reduce battery degradation. This utility can set your charge threshold to whatever
  you want.
- **Lightweight:** YAMDCC takes up less than a megabyte of disk space when installed, and
  only works when re-applying configs (manually, or when rebooting or waking up from sleep mode).
- **Configurable:** Almost all settings (including those not accessible through the config GUI) can
  be changed with the power of XML.

## Screenshots

![A screenshot of YAMDCC's fan control interface](Media/YAMDCC-FanControlTab.png)

![A screenshot of YAMDCC's extra options interface](Media/YAMDCC-ExtrasTab.png)

## Supported Laptops

Currently, only the MSI GF63 Thin 11SC is supported, with more MSI laptop support Coming Soon™.

In the meantime, you must make your own config for your laptop (tutorial Coming Soon™).

This should be as easy as supplying your laptop's default fan curves to the Defaut profile in a copy of
an existing MSI laptop config, however some MSI laptops have a few extra setting located at different EC
registers. Try looking up similar fan control utilities (most are written for Linux).

Other laptop brands are not officially supported. You can still try and make your own config, but chances
are you're looking for [NoteBook FanControl](https://github.com/UraniumDonut/nbfc-revive) instead.

**Please avoid asking me (or other people) in the issue tracker to create a config for you.**
**Unless we have your specific laptop model (which we probably don't), we will not be able to**
**help you outside of the general instructions.**

## Comparison

| Feature                   | MSI Center | YAMDCC   |
|---------------------------|------------|----------|
| Installed size            | ~950 MB¹   | ~1.1 MB  |
| Fan control               | ✔          | ✔        |
| Temp. threshold control   | ❌          | ✔        |
| Multi-fan profile support | ❌          | ✔        |
| Charge threshold setting  | Limited²   | ✔        |
| Perf. mode setting        | ✔          | ✔        |
| Win/Fn key swap           | ✔          | ✔        |
| Win key disable           | ✔          | ❌        |
| Hardware monitoring       | ✔          | Limited³ |
| Other MSI Center features | ✔          | ❌        |
| Open source               | ❌          | ✔        |

1: As of v2.0.38, MSI Center takes about 950 MB of storage space when counting the UWP app (749 MB) and the files installed on first launch to `C:\Program Files (x86)\MSI` (205 MB).

2: MSI Center only supports setting the charge threshold to 60%, 80%, or 100%, while YAMDCC can set this to anything between 0 and 100% (with 0 meaning charge to 100% always).

3: YAMDCC only supports monitoring the CPU/GPU temperatures and fan speeds via EC.

## Roadmap

- [ ] Config UI fixes:
  - [ ] Actually implement the "revert to last saved config" functionality
  - [ ] Implement missing tooltips
- [ ] Code bug fixes *(started)*
  - [ ] Fix the config system (it likes to crash if even a slightly wrong config is loaded)
  - [ ] just (re)write more robust code in general lol
- [x] Add more extra options for MSI laptops (from MSI Center):
  - [x] Performance mode selection
  - [x] Fn/Win key swap
- [ ] Config generation for MSI laptops
  - This would only work because many MSI laptops have almost identical EC register locations
    for all the relevent settings we change
  - The only thing we need to do is get the default fan curve from the user's laptop, and add
    it to the default fan profile.
- [ ] Command line support
  - The beginning of a CLI for YAMDCC exists, just not publicly yet
- [ ] .NET support
  - Mandatory for Linux support
  - The GUI *should* compile on .NET 8, but hasn't been tested yet
- [ ] Support for editing laptop config registers using the GUI interface
  - This would allow for creating configs for other laptop brands from the config UI
  - Currently, the only way to do this is to edit the XML directly

## Download

Development builds are availabe through [GitHub Actions](https://github.com/Sparronator9999/MSIFanControl/actions).

Alternatively, if you don't have a GitHub account, you can download the latest build from [nightly.link](https://nightly.link/Sparronator9999/MSIFanControl/workflows/build/main?preview).

(You probably want the `Release` build, unless you're debugging issues with the program)

Alternatively, you can [build the program yourself](#build).

## Build

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

### From command line

1.  Follow steps 1-3 above to install Visual Studio and download the code.
2.  Open `Developer Command Prompt for VS 2022` and `cd` to your project directory.
3.  Run `msbuild /t:restore` to restore the solution, including NuGet packages.
4.  Run `msbuild YAMDCC.sln /p:platform="Any CPU" /p:configuration="Debug"` to build
    the project, substituting `Debug` with `Release` (or `Any CPU` with `x86` or `x64`) as 
5.  Your output should be located in `YAMDCC.GUI\bin\Debug\net48\`, assuming you built
    with the above unmodified command.
6.  ???
7.  Profit!

## Issues

If your question isn't already answered in the [FAQ](#faq) or [issues megathread](https://github.com/Sparronator9999/YAMDCC/issues/1),
feel free to open an issue. Please make sure to use the correct issue template for your problem.

<!--
- Laptop model, system specifications (CPU, GPU), and OS version
- A detailed description of the problem
- Steps to reproduce the issue
- Relevant screenshots when needed
-->

However, know that I **may not check my GitHub page very often** when I'm not working
on anything, so your issue may remain open for a while before I answer it.

## Contributing

See the [build instructions](#build) to build this project.

If you would like to contribute to the project with bug fixes, new features,
or new configs, feel free to open a pull request. Please include the following:

- **Bug Fixes/Improvements:** Describe the changes you made and why they
  are important or useful.
- ~~**New Config:**~~ Not currently accepting new configs.

## FAQ

### Can you please make a Linux version?

Soon™.

Use one of the [many](https://github.com/dmitry-s93/MControlCenter) [other](https://github.com/gourav1100/isw)
[projects](https://github.com/YoCodingMonster/OpenFreezeCenter) on GitHub instead while you wait.

### What versions of Windows do you support?

This program is tested by me (Sparronator9999) on 64-bit Windows 10 (specifically LTSC 2021).
It should, however, run on any verison of Windows 10, 32- or 64-bit.

Windows 11 *should* be supported as well, but I have not tested it. Open an issue if you have trouble with Windows 11.

Older versions of Windows may also work, but with no support from me.

### Why do I need administrator privileges to run this program?

The YAMDCC service requires administrator privileges to install and communicate with
the WinRing0 driver, which allows for low-level hardware access (required for EC access).
This is restricted to privileged programs for obvious reasons.

### My laptop isn't supported! What do I do?

[See above](#supported-laptops).

### Can you write a config for my laptop?

Again, [see above](#supported-laptops).

### Help! My laptop stopped booting/is doing weird stuff!

Reset your EC (MSI laptops only):

Shut down the laptop if it's on (force shut down if needed), then find the EC reset button
(on the GF63 Thin 11SC, it's a small hole located on the bottom of the laptop next to the charge port)
and press it with the end of a paperclip (or similarly small object, e.g. SIM eject tool)
for at least 5 seconds. Try rebooting.

If the issue persists, try unplugging all power sources, including the laptop battery and
CMOS/clock battery (requires disassembly of laptop), for a few seconds. Re-connect everything,
then re-assemble and attempt another reboot. This will reset your BIOS settings.

Users of other laptop brands will need to look up instructions for their laptop.

### How does this program work?

YAMDCC works by accessing your laptop's embedded controller (aka, the EC). Many settings that
can be configured with MSI Center are stored here, including fan curve, performance mode,
and the Win/Fn key swap setting.

### Dark mode?

Due to WinForms limitations, no.

Technical explanation: A few specific WinForms controls used by YAMDCC look really bad
when trying to recolour them to be dark themed. Also, built-in dialog boxes (for C# programmers,
think `MessageBox.Show`) cannot be recoloured from their default white theme. Also, I have little
to no experience with other UI kits (e.g. WPF).

### .NET (Core) 5/6/8/<insert latest .NET version>!

Probably not for Windows (unless it goes EOL, which I doubt will happen for a while).

If Linux support ever comes, it will be using .NET (since .NET Framework isn't supported on Linux).

### Doesn't WinRing0 have security issues?

[Yes](https://voidsec.com/crucial-mod-utility-lpe-cve-2021-41285/), however YAMDCC installs the
driver such that only programs with administrator privileges can access the driver functions
(something that should have been done in the first place by the driver itself), largely
mitigating this vulnerability.

However, if YAMDCC finds the driver already installed, it will use that (potentially vulnerable)
version instead. If it was installed with, e.g. [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor),
you should be fine, as they implement the same fix.

The [updated fork of WinRing0](https://github.com/GermanAizek/WinRing0) updates the driver itself
(`WinRing0.sys`) to apply this fix, however binary releases of the driver aren't provided due to
Microsoft's driver signing requirements, and I'm too smooth-brained to write my own EC access
kernel driver (but apparently not an entire fan control utility from scratch, including the
WinRing0 interface code...), and I'd have to get it signed anyways.

Please read the [disclaimer](#disclaimers), especially the bold text, if you haven't already.

## License and Copyright

Copyright © 2023-2024 Sparronator9999.

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

- [Named Pipe Wrapper](https://github.com/acdvorak/named-pipe-wrapper), as `YAMDCC.IPC`,
  for communication between the service and UI program.
- [WinRing0](https://github.com/QCute/WinRing0) for low-level hardware access required to
  read/write the EC.
