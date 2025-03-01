using MessagePack;

namespace YAMDCC.IPC;

/// <summary>
/// Represents a list of possible responses to a <see cref="ServiceCommand"/>.
/// </summary>
public enum Response
{
    /// <summary>
    /// Fallback value if empty (zero-length) message received by client.
    /// </summary>
    Nothing,
    /// <summary>
    /// Sent when any command that doesn't return data finishes successfully.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This response's <see cref="ServiceResponse.Value"/> field includes
    /// the following data as an <see langword="int"/>:<br/>
    /// • Command: The command that sent this response.
    /// </para>
    /// </remarks>
    Success,
    /// <summary>
    /// Sent when any command encounters an error.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This response's <see cref="ServiceResponse.Value"/> field includes
    /// the following data as an <see langword="int"/>:<br/>
    /// • Command: The command that sent this response.
    /// </para>
    /// </remarks>
    Error,
    /// <summary>
    /// The result of a <see cref="Command.GetServiceVer"/> command.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This response's <see cref="ServiceResponse.Value"/> field includes
    /// the following data as a <see langword="string"/>:<br/>
    /// • Revison: The current YAMDCC service version, as a Git revision/hash.
    /// </para>
    /// </remarks>
    ServiceVer,
    /// <summary>
    /// The result of a <see cref="Command.GetFirmVer"/> command.
    /// </summary>
    /// <remarks>
    /// This response's <see cref="ServiceResponse.Value"/> field
    /// includes an <see cref="EcInfo"/> instance containing the
    /// firmware version and date.
    /// </remarks>
    FirmVer,
    /// <summary>
    /// The result of a <see cref="Command.ReadECByte"/> command.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This response's <see cref="ServiceResponse.Value"/> field includes
    /// the following data as an <see langword="int"/> array:<br/>
    /// • Register: The EC register that was read from.<br/>
    /// • Value: The value that was stored in the EC register.
    /// </para>
    /// </remarks>
    ReadResult,
    /// <summary>
    /// The result of a <see cref="Command.GetFanSpeed"/> command.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This response's <see cref="ServiceResponse.Value"/> field includes
    /// the following data as an <see langword="int"/> array:<br/>
    /// • Fan: The zero-indexed fan in the loaded config that the fan speed was read from.<br/>
    /// • Temp: The speed of the fan, as a percentage.
    /// </para>
    /// </remarks>
    FanSpeed,
    /// <summary>
    /// The result of a <see cref="Command.GetFanRPM"/> command.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This response's <see cref="ServiceResponse.Value"/> field includes
    /// the following data as an <see langword="int"/> array:<br/>
    /// • Fan: The zero-indexed fan in the loaded config that the fan RPM was read from.<br/>
    /// • Temp: The fan RPM.
    /// </para>
    /// </remarks>
    FanRPM,
    /// <summary>
    /// The result of a <see cref="Command.GetTemp"/> command.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This response's <see cref="ServiceResponse.Value"/> field includes
    /// the following data as an <see langword="int"/> array:<br/>
    /// • Fan: The zero-indexed fan in the loaded config that the component temperature was read from.<br/>
    /// • Temp: The temperature of the component being cooled by the fan.
    /// </para>
    /// </remarks>
    Temp,
    /// <summary>
    /// The result of a <see cref="Command.GetKeyLightBright"/> command.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This response's <see cref="ServiceResponse.Value"/> field includes
    /// the following data as a <see langword="byte"/><br/>
    /// • Brightness: The keyboard backlight brightness.
    /// </para>
    /// </remarks>
    KeyLightBright,
    /// <summary>
    /// Sent to all connected clients when the YAMDCC service
    /// reloads the current YAMDCC config (CurrentConfig.xml).
    /// </summary>
    /// <remarks>
    /// <para>
    /// This response's <see cref="ServiceResponse.Value"/> field includes
    /// the following data as an <see langword="int"/><br/>
    /// • ClientId: The ID of the client that triggered the config reload.
    /// </para>
    /// </remarks>
    ConfLoaded,
}

/// <summary>
/// Represents a response to a <see cref="ServiceCommand"/>.
/// </summary>
[MessagePackObject]
public class ServiceResponse
{
    /// <summary>
    /// The <see cref="IPC.Response"/> to send to the service.
    /// </summary>
    [Key(0)]
    public Response Response { get; set; } = Response.Nothing;

    /// <summary>
    /// The value associated with the <see cref="IPC.Response"/>.
    /// </summary>
    [Key(1)]
    public object[] Value { get; set; }

    /// <summary>
    /// Initialises a new instance of the <see cref="ServiceResponse"/>
    /// struct with the specified message and return value.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="value"></param>
    public ServiceResponse(Response response, params object[] value)
    {
        Response = response;
        Value = value;
    }
}
