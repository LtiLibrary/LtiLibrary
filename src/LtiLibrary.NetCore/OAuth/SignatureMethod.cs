namespace LtiLibrary.NetCore.OAuth
{
    /// <summary>
    /// LTI signature methods for computing hashes.
    /// </summary>
    public enum SignatureMethod
    {
        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA1"/> hash for the input data.
        /// This is the default for LTI 1.1.
        /// </summary>
        HmacSha1,

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA256"/> hash for the input data.
        /// </summary>
        HmacSha256,

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA384"/> hash for the input data.
        /// </summary>
        HmacSha384,

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA512"/> hash for the input data.
        /// </summary>
        HmacSha512,
    }
}
