namespace LtiLibrary.Core.Common
{
    /// <summary>
    /// A basic result with a boolean success and string message.
    /// This class can be implicitly cast to a bool.
    /// </summary>
    public class BasicResult
    {   
        public bool IsValid  { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// A basic result.
        /// </summary>
        /// <param name="isValid">True if the result is valid.</param>
        /// <param name="message">Message.</param>
        public BasicResult(bool isValid = true, string message = "")
        {
            IsValid = isValid;
            Message = message;
        }

        public static implicit operator bool(BasicResult basicResult)
        {
            return basicResult != null && basicResult.IsValid;
        }
    }
}
