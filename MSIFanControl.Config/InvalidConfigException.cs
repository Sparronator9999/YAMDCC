// This file is part of MSI Fan Control.
// Copyright © Sparronator9999 2023-2024.
//
// MSI Fan Control is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// MSI Fan Control is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// MSI Fan Control. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Runtime.Serialization;

namespace MSIFanControl.Config
{
    /// <summary>
    /// The exception thrown when an invalid <see cref="FanControlConfig"/> is loaded.
    /// </summary>
    [Serializable]
    public sealed class InvalidConfigException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigException" /> class.
        /// </summary>
        public InvalidConfigException()
            : base("The fan control config was not in an expected format.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigException"/>
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidConfigException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigException"/> class
        /// with a specified error message and a reference to the inner exception that
        /// is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null
        /// reference (<see langword="Nothing"/> in Visual Basic) if no inner
        /// exception is specified.
        /// </param>
        public InvalidConfigException(string message, Exception innerException)
            : base(message, innerException) { }

        private InvalidConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
