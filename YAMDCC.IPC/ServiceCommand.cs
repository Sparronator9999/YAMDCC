using System;

namespace YAMDCC.IPC
{
    /// <summary>
    /// Represents a list of possible commands that can
    /// be sent to the YAMDCC Service.
    /// </summary>
    public enum Command
    {
        /// <summary>
        /// <para>Get the YAMDCC Service version.</para>
        /// <para>
        /// The result is sent to the caller as a
        /// <see cref="Response.Version"/> message.
        /// </para>
        /// </summary>
        /// <remarks>This command expects no arguments.</remarks>
        GetVersion,
        /// <summary>
        /// <para>Read a byte from the EC.</para>
        /// <para>The result is sent to the caller as a
        /// <see cref="Response.ReadResult"/> message.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// This command expects the following arguments as
        /// a space-seperated string:<br/>
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
        /// a space-seperated string:<br/>
        /// • Register: The EC register to write to.<br/>
        /// • Value: The value to write.
        /// </para>
        /// </remarks>
        WriteECByte,
        /// <summary>
        /// Get the target speed of a specified system fan in the
        /// currently loaded YAMDCC config.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This command expects the following arguments as
        /// a space-seperated string:<br/>
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
        /// This command expects the following arguments as
        /// a space-seperated string:<br/>
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
        /// This command expects the following arguments as
        /// a space-seperated string:<br/>
        /// • Fan: The index of the fan to read the associated component's temperature from.
        /// </para>
        /// <para>
        /// The result is sent to the caller as a
        /// <see cref="Response.Temp"/> message.
        /// </para>
        /// </remarks>
        GetTemp,
        /// <summary>
        /// Reload and apply a YAMDCC config.
        /// </summary>
        ApplyConfig,
        /// <summary>
        /// Enable or disable Full Blast on the system.
        /// </summary>
        /// <remarks>
        /// This command expects the following arguments as
        /// a space-seperated string:<br/>
        /// • Enable: 1 to enable Full Blast, 0 to disable.
        /// </remarks>
        FullBlast,
        /// <summary>
        /// Gets the brightness of the keyboard backlight,
        /// and sends a <see cref="Response.KeyLightBright"/>
        /// response with the result.
        /// </summary>
        GetKeyLightBright,
        /// <summary>
        /// Sets the keyboard backlight to the specified value.
        /// </summary>
        /// <remarks>
        /// This command expects the following arguments as
        /// a space-seperated string:<br/>
        /// • Brightness: A value between the minimum and
        /// maximum brightness value (minus offset).
        /// </remarks>
        SetKeyLightBright,
        /// <summary>
        /// Attempts to write the default fan curves
        /// of each fan from the EC to the config.
        /// </summary>
        /// <remarks>
        /// <para>This command expects no arguments.</para>
        /// <para>
        /// If the loaded config is a template config, it
        /// will be made into a full (non-template) config.
        /// </para>
        /// <para>
        /// If there is no config loaded, this command will issue a
        /// <see cref="Response.Error"/> to the requesting client.
        /// </para>
        /// </remarks>
        FanCurveECToConf,
    }

    /// <summary>
    /// Represents a command to send to the YAMDCC Service.
    /// </summary>
    [Serializable]
    public class ServiceCommand
    {
        /// <summary>
        /// The <see cref="IPC.Command"/> to send to the service.
        /// </summary>
        public Command Command { get; set; }

        /// <summary>
        /// The argument(s) to send to the service with the command.
        /// The number of parameters for a service command vary depending on the
        /// specific command sent to the service.
        /// </summary>
        public string Arguments { get; set; }

        public ServiceCommand(Command command, string args)
        {
            Command = command;
            Arguments = args;
        }
    }
}
