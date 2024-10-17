using MessagePack;

namespace YAMDCC.IPC
{
    /// <summary>
    /// Represents a list of possible responses to a <see cref="ServiceCommand"/>.
    /// </summary>
    public enum Response
    {
        /// <summary>
        /// Fallback value if empty (zero-length) message received by client.
        /// </summary>
        Nothing = 0,
        /// <summary>
        /// Sent when any command that doesn't return data finishes successfully.
        /// </summary>
        Success,
        /// <summary>
        /// Sent when any command encounters an error.
        /// </summary>
        Error,
        /// <summary>
        /// The result of a <see cref="Command.GetVersion"/> command.
        /// </summary>
        Version,
        /// <summary>
        /// The result of a <see cref="Command.GetTemp"/> command.
        /// </summary>
        Temp,
        /// <summary>
        /// The result of a <see cref="Command.GetFanSpeed"/> command.
        /// </summary>
        FanSpeed,
        /// <summary>
        /// The result of a <see cref="Command.GetFanRPM"/> command.
        /// </summary>
        FanRPM,
        /// <summary>
        /// The result of a <see cref="Command.ReadECByte"/> command.
        /// </summary>
        ReadResult,
        /// <summary>
        /// The result of a <see cref="Command.GetKeyLightBright"/> command.
        /// </summary>
        KeyLightBright,
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
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Initialises a new instance of the <see cref="ServiceResponse"/>
        /// struct with the specified message and return value.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="value"></param>
        public ServiceResponse(Response response, string value)
        {
            Response = response;
            Value = value;
        }
    }
}
