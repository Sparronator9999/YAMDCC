using MessagePack;

namespace YAMDCC.IPC;

/// <summary>
/// Represents a list of possible commands that can
/// be sent to the YAMDCC Service.
/// </summary>
public enum Command
{
    /// <summary>
    /// Fallback value if empty (zero-length) message received by server.
    /// </summary>
    Nothing = 0,
    /// <summary>
    /// Get the YAMDCC Service version, as a Git revision hash.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The result is sent to the caller as a
    /// <see cref="Response.ServiceVer"/> message.
    /// </para>
    /// <para>This command expects no arguments.</para>
    /// </remarks>
    GetServiceVer,
    /// <summary>
    /// Gets the EC firmware version and date for the current laptop.
    /// </summary>
    /// <remarks>
    /// <para>This command expects no arguments.</para>
    /// <para>
    /// The result is sent to the caller as a
    /// <see cref="Response.FirmVer"/> message.
    /// </para>
    /// </remarks>
    GetFirmVer,
    /// <summary>
    /// Read a byte from the EC.
    /// </summary>
    /// <remarks>
    /// <para>The result is sent to the caller as a
    /// <see cref="Response.ReadResult"/> message.
    /// </para>
    /// <para>
    /// This command expects the following argument as
    /// a <see langword="byte"/>:<br/>
    /// • Register: The EC register to read.
    /// </para>
    /// </remarks>
    ReadECByte,
    /// <summary>
    /// Write a byte to the EC.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This command expects the following arguments as
    /// a <see langword="byte"/> array:<br/>
    /// • Register: The EC register to write to.<br/>
    /// • Value: The value to write.
    /// </para>
    /// </remarks>
    WriteECByte,
    /// <summary>
    /// Reloads and applys the YAMDCC config located at
    /// <c>C:\ProgramData\Sparronator9999\YAMDCC\CurrentConfig.xml</c>.
    /// </summary>
    ApplyConf,
    /// <summary>
    /// Enable or disable Full Blast on the system.
    /// </summary>
    /// <remarks>
    /// This command expects the following argument as
    /// a <see langword="bool"/>:<br/>
    /// • Enable: 1 to enable Full Blast, 0 to disable, -1 to toggle.
    /// </remarks>
    SetFullBlast,
    /// <summary>
    /// Get the target speed of a specified system fan in the
    /// currently loaded YAMDCC config.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This command expects the following argument as
    /// an <see langword="int"/>:<br/>
    /// • Fan: The index of the fan to read the target speed from.
    /// </para>
    /// <para>
    /// The result is sent to the caller as a
    /// <see cref="Response.FanSpeed"/> message.
    /// </para>
    /// </remarks>
    GetFanSpeed,
    /// <summary>
    /// Get the RPM of a specified system fan in the
    /// currently loaded YAMDCC config.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This command expects the following argument as
    /// an <see langword="int"/>:<br/>
    /// • Fan: The index of the fan to read the RPM from.
    /// </para>
    /// <para>
    /// The result is sent to the caller as a
    /// <see cref="Response.FanRPM"/> message.
    /// </para>
    /// </remarks>
    GetFanRPM,
    /// <summary>
    /// Get the temperature of the component (CPU, GPU...) associated
    /// with a specified system fan in the currently loaded YAMDCC config.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This command expects the following argument as
    /// an <see langword="int"/>:<br/>
    /// • Fan: The index of the fan to read the associated component's temperature from.
    /// </para>
    /// <para>
    /// The result is sent to the caller as a
    /// <see cref="Response.Temp"/> message.
    /// </para>
    /// </remarks>
    GetTemp,
    /// <summary>
    /// Gets the brightness of the keyboard backlight,
    /// and sends a <see cref="Response.KeyLightBright"/>
    /// response with the result.
    /// </summary>
    /// <remarks>
    /// Returns an error if the read keyboard backlight value is outside the allowed
    /// range specified by the current YAMDCC config, or if keyboard backlight
    /// support straight up doesn't exist in the current YAMDCC config.
    /// </remarks>
    GetKeyLightBright,
    /// <summary>
    /// Sets the keyboard backlight to the specified value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Returns an error if the keyboard backlight value is outside the allowed
    /// range specified by the current YAMDCC config, or if keyboard backlight
    /// support straight up doesn't exist in the current YAMDCC config.
    /// </para>
    /// <para>
    /// This command expects the following argument as
    /// a <see langword="byte"/>:<br/>
    /// • Brightness: A value between the minimum and
    /// maximum brightness value (minus offset).
    /// </para>
    /// </remarks>
    SetKeyLightBright,
    /// <summary>
    /// Sets whether the Windows and Fn keys are swapped compared to their labels.
    /// </summary>
    /// <remarks>
    /// <para>This setting will be reset when reloading the current config.</para>
    /// <para>This command expects no arguments.</para>
    /// </remarks>
    SetWinFnSwap,
    /// <summary>
    /// Sets all fans' profiles to the specified index, or cycles through
    /// all available fan profiles (depending on the value passed to
    /// <see cref="ServiceCommand.Arguments"/>).
    /// </summary>
    /// <remarks>
    /// <para>This setting will be reset when reloading the current config.</para>
    /// <para>Out-of-range values are clamped to the nearest valid value.</para>
    /// <para>
    /// This command expects the following argument as
    /// an <see langword="int"/>:<br/>
    /// • ProfSel: A zero-indexed value indicating the fan profile to switch to,
    /// or -1 to switch to the next fan profile in order.
    /// </para>
    /// </remarks>
    SetFanProf,
    /// <summary>
    /// Sets the computer's performance mode to the specified index, or cycles through
    /// all available performance modes (depending on the value passed to
    /// <see cref="ServiceCommand.Arguments"/>).
    /// </summary>
    /// <remarks>
    /// <para>This setting will be reset when reloading the current config.</para>
    /// <para>Out-of-range values are clamped to the nearest valid value.</para>
    /// <para>
    /// Returns an error if performance modes are not
    /// supported by the current YAMDCC config.
    /// </para>
    /// <para>
    /// This command expects the following argument as
    /// an <see langword="int"/>:<br/>
    /// • ModeSel: A zero-indexed value indicating the performance mode to switch to,
    /// or -1 to switch to the next performance mode in order.
    /// </para>
    /// </remarks>
    SetPerfMode,
}

/// <summary>
/// Represents a command to send to the YAMDCC Service.
/// </summary>
[MessagePackObject]
public class ServiceCommand
{
    /// <summary>
    /// The <see cref="IPC.Command"/> to send to the service.
    /// </summary>
    [Key(0)]
    public Command Command { get; set; } = Command.Nothing;

    /// <summary>
    /// The argument(s) to send to the service with the command.
    /// The number of parameters for a service command vary depending on the
    /// specific command sent to the service.
    /// </summary>
    [Key(1)]
    public object[] Arguments { get; set; }

    public ServiceCommand(Command command, params object[] args)
    {
        Command = command;
        Arguments = args;
    }
}
