using System;

namespace YAMDCC.IPC
{
    /// <summary>
    /// Represents a list of possible responses to a <see cref="ServiceCommand"/>.
    /// </summary>
    public enum Response
    {
        /// <summary>
        /// Sent when any command that doesn't return data finishes successfully.
        /// </summary>
        Success,
        /// <summary>
        /// Sent when any command encounters an error.
        /// </summary>
        Fail,
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
        ReadResult
    }

    /// <summary>
    /// Represents a response to a <see cref="ServiceCommand"/>.
    /// </summary>
    [Serializable]
    public class ServiceResponse
    {
        /// <summary>
        /// The <see cref="IPC.Response"/> to send to the service.
        /// </summary>
        public Response Response { get; set; }

        /// <summary>
        /// The value associated with the <see cref="IPC.Response"/>.
        /// </summary>
        public string Value { get; set; }

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
