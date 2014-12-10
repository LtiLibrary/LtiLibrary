using System;

namespace LtiLibrary.Core.Common
{
    public class LtiException : Exception
    {
        public LtiException() { }
        public LtiException(string message) : base(message) { }
        public LtiException(string message, Exception innerException) : base(message, innerException) { }
    }
}