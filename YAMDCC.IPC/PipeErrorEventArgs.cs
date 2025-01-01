using System;

namespace YAMDCC.IPC
{
    /// <summary>
    /// Provides data for the
    /// <see cref="NamedPipeClient{TRead, TWrite}.Error"/> and
    /// <see cref="NamedPipeServer{TRead, Write}.Error"/> events.
    /// </summary>
    public class PipeErrorEventArgs<TRead, TWrite> : EventArgs
        where TRead : class
        where TWrite : class
    {
        public NamedPipeConnection<TRead, TWrite> Connection { get; }

        /// <summary>
        /// The <see cref="System.Exception"/> that caused the error.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="PipeErrorEventArgs"/> class.
        /// </summary>
        /// <param name="connection">
        /// <para>The connection that caused the error.</para>
        /// <para>
        /// The only time this should be <c>null</c> is if the error was caused
        /// by <see cref="NamedPipeServer{TRead, TWrite}.WaitForConnection"/>.
        /// </para>
        /// </param>
        /// <param name="exception">
        /// The exception that caused the error.
        /// </param>
        internal PipeErrorEventArgs(
            NamedPipeConnection<TRead, TWrite> connection,
            Exception exception)
        {
            Connection = connection;
            Exception = exception;
        }
    }
}
