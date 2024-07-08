# MSI Fan Control

A fast, lightweight, and highly configurable fan control utility for MSI laptops, written in C#.

**Please read the whole README (or at least the [Supported Laptops](#supported-laptops) and [FAQ](#faq) sections) before downloading.**

## Disclaimers

- While this program is mostly complete, I still consider it to be **alpha-quality software!** You *will* likely encounter bugs or missing features!
- This program requires low-level access to some of your computer hardware in order to control your computer's fans. While no issues should arise from the use of this program, **I (Sparronator9999) and any other contributers shall not be held responsible for any**
**damage to your laptop that result from your use of this program.**
- I (Sparronator9999) am currently transitioning to Linux for [various](https://www.theverge.com/2024/6/3/24170305) [reasons](https://www.howtogeek.com/how-to-disable-microsofts-ads-and-recommendations-in-windows-11/) (and more, outside of the links provided), and so I may no longer be able to test to make sure no one else broke something in the near future.
- This program, repository and its authors are not affiliated with Micro-Star International Co., Ltd. in any way, shape, or form.

## Features

- **Fan control:** MSI Fan Control can set fan curves for your CPU and GPU fans, including fan speeds, temperature thresholds, and Full Blast (a.k.a. Cooler Boost in MSI Center).
- **Charging threshold:** MSI laptops come with the ability to limit the battery charge percentage, which can reduce battery degradation. This utility can set your charge threshold to whatever you want.
- **Lightweight:** MSI Fan Control takes up less than half a megabyte of disk space when installed, and only works when re-applying configs (manually, or when rebooting or waking up from sleep mode).

## Screenshots

![MSI Fan Control's main interface](Media/MSIFC-MainWindow.png)

## Supported Laptops

Please see the `Configs` folder for a list of supported laptops.

If your laptop is not listed, you must make your own config (tutorial Coming Soon™).

For MSI laptops, this is as easy as supplying your laptop's default fan curves to the Defaut profile in a copy of an existing MSI laptop config.

Other laptop brands will require you to figure out which EC registers store the fan curve (or just use NoteBook FanControl if your laptop's supported).

Please also avoid asking me (or other people) in the issue tracker to create a config for you.

## FAQ

### Can you please make a Linux version?

Soon™.

Use one of the [many](https://github.com/dmitry-s93/MControlCenter) [other](https://github.com/gourav1100/isw)
[projects](https://github.com/YoCodingMonster/OpenFreezeCenter) on GitHub instead while you wait.

### What versions of Windows do you support?

This program is tested by me (Sparronator9999) on 64-bit Windows 10 (specifically LTSC 2021).
It should, however, run on any verison of Windows 10, 32- or 64-bit.

Windows 11 should be supported as well, but I have not tested it. Open an issue if you have trouble with Windows 11.

Older versions of Windows may also work, but with no support from me.

### Why do I need administrator privileges to run this program?

The MSI Fan Control service requires administrator privileges to install and
communicate with the WinRing0 driver, which allows for low-level hardware
access. This is restricted to privileged programs for obvious reasons.

### My laptop isn't supported! What do I do?

[See above](#supported-laptops).

### Can you write a config for my laptop?

Again, [see above](#supported-laptops).

### Help! My laptop stopped booting/is doing weird stuff!

On MSI laptops, shut down the laptop (holding the power button to force shut down if needed), then find the
EC reset button (on the GF63 Thin 11SC, it is located on the underside of the laptop near the charge port)
and press it with the end of a paperclip (or similarly small object) for at least 5 seconds.

If all else fails, you can try unplugging the laptop's battery for a few
seconds (including any CMOS/Clock batteries, requires disassembly of laptop),
then plug them back in, re-assemble the laptop and reboot. This will reset your
BIOS configuration, though.

For other laptop brands, you will need to find instructions for your laptop.

### How does this program work?

MSI Fan Control works by writing a fan curve to your laptop's embedded controller (aka, the EC).
This is a feature that, as far as I know, is only supported by MSI laptops.

This is unlike NoteBook FanControl, which monitors the CPU temperature and directly controls
the fan speed using a backgronud service.

### Dark mode?

Due to WinForms limitations, no.

### Doesn't WinRing0 have security issues?

[Yes](https://voidsec.com/crucial-mod-utility-lpe-cve-2021-41285/), however the
[updated fork](https://github.com/GermanAizek/WinRing0) does not provide a binary release
(due to Microsoft's driver signing requirements). I don't have the time or money to go through
the hassle of trying to get it production-signed myself, so here we are.

Please read the [disclaimer](#disclaimers), especially the bold text, if you haven't already.

## Issues

If your question wasn't answered in the FAQ, feel free to open an issue
request. Please state the following in your issue request:

- Laptop model, system specifications (CPU, GPU), and OS version
- A detailed description of the problem
- Reproduction steps
- Relevant screenshots

However, know that I **don't check my GitHub page very often** when I'm not working
on anything, so your issue may remain open for a while before I answer it.

## Roadmap

- [ ] Make sure GitHub didn't break anything after initial upload  
- [ ] Config UI fixes:  
  - [ ] Add button to create and delete fan profiles  
  - [ ] Add option to uninstall service  
- [ ] Config generation for MSI laptops  
  - This would only work because many MSI laptops have almost identical EC register locations
    for all the relevent settings we change  
  - The only thing we need to do is get the default fan curve from the user's laptop, and add
    it to the default fan profile.  
- [ ] Command line support  
- [ ] Support for creating laptop configs using the GUI interface  
  - Currently, the only way to make a new config for MSI Fan Control is to
    edit the XML directly

## Contributing

See the [build instructions](#build) below to build this project.

If you would like to contribute to the project with bug fixes, new features,
or new configs, feel free to open a pull request. Please include the following:

- **Bug Fixes/Improvements:** Describe the changes you made and why they
  are important or useful.
- **New Config:** Include the laptop model the config was made for in the PR
  title. Make sure your laptop doesn't need any modifications (installing 3rd-party
  software (**MSI Center**), or even physical hardware modifications) for the
  config to work.

## Build

### Using Visual Studio

1.  Install Visual Studio 2022 with the `.NET Desktop Development` workload checked.
2.  Download the code repository, or clone it with `git`.
3.  Extract the downloaded code, if needed.
4.  Open `MSIFanControl.sln` in Visual Studio.
5.  Click `Build` > `Build Solution` to build everything.
6.  Your output, assuming default build settings, is located in `MSIFanControl.GUI\bin\Debug\`.
7.  ???
8.  Profit!

Make sure to only use matching `msifcsvc.exe` and `MSIFanControl.exe` together, otherwise you
may encounter issues.

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

- [Named Pipe Wrapper](https://github.com/acdvorak/named-pipe-wrapper), as `MSIFanControl.IPC`,
  for communication between the service and UI program.
- [WinRing0](https://github.com/QCute/WinRing0) for low-level hardware access required to
  read/write the EC.
