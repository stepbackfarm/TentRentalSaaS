namespace TentRentalSaaS.Api.Helpers
{
    /// <summary>
    /// Security utilities for cryptographic operations
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Performs constant-time string comparison to prevent timing attacks.
        /// This is critical for comparing security tokens, passwords, or any other
        /// sensitive strings where timing differences could leak information.
        /// </summary>
        /// <param name="a">First string to compare</param>
        /// <param name="b">Second string to compare</param>
        /// <returns>True if strings are equal, false otherwise</returns>
        public static bool SecureStringEquals(string? a, string? b)
        {
            if (a == null || b == null)
                return a == b;

            if (a.Length != b.Length)
                return false;

            var result = 0;
            for (var i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }
    }
}
