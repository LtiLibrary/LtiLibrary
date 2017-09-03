using System;

namespace LtiLibrary.NetCore.Common
{
    /// <summary>
    /// Represents errors that occur during LTI execution.
    /// </summary>
    public class LtiException : Exception
    {
        /// <summary>
        /// Initialize a new instance of the LtiException class.
        /// </summary>
        public LtiException() { }

        /// <summary>
        /// Initialize a new instance of the LtiException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LtiException(string message) : base(message) { }

        /// <summary>
        /// Initialize a new instance of the LtiException class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public LtiException(string message, Exception innerException) : base(message, innerException) { }
    }
}