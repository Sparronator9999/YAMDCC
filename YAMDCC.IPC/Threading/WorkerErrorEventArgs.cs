using System;

namespace YAMDCC.IPC.Threading
{
    /// <summary>
    /// Provides data for the
    /// <see cref="NamedPipeClient{TRead, TWrite}.Error"/> and
    /// <see cref="NamedPipeServer{TRead, Write}.Error"/> events.
    /// </summary>
    public class WorkerErrorEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="System.Exception"/> that caused the error.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="PipeErrorEventArgs"/> class.
        /// </summary>
        /// <param name="exception">
        /// The exception that caused the error.
        /// </param>
        internal WorkerErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
