using System;

namespace MSIFanControl.IPC
{
    /// <summary>
    /// Represents a list of possible commands that can
    /// be sent to the MSI Fan Control Service.
    /// </summary>
    public enum Command
    {
        /// <summary>
        /// <para>Get the MSI Fan Control Service version.</para>
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
        /// currently loaded MSI Fan Control config.
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
        /// currently loaded MSI Fan Control config.
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
        /// with a specified system fan in the currently loaded MSI Fan
        /// Control config.
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
        /// Reload and apply an MSI Fan Control config.
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
        FullBlast
    }

    /// <summary>
    /// Represents a command to send to the MSI Fan Control Service.
    /// </summary>
    [Serializable]
    public class ServiceCommand
    {
        /// <summary>
        /// The <see cref="Command"/> to send to the service.
        /// </summary>
        public Command Command;

        /// <summary>
        /// The argument(s) to send to the service with the command.
        /// The number of parameters for a service command vary depending on the
        /// specific command sent to the service.
        /// </summary>
        public string Arguments;

        public ServiceCommand(Command command, string args)
        {
            Command = command;
            Arguments = args;
        }
    }
}
