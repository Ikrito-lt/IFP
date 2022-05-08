using System;

namespace IFP.Utils
{
    /// <summary>
    /// Class that is used to hash strings using SHA256
    /// </summary>
    internal static class StrHash
    {
        public static string HashString(string text, int lenght, string salt = "")
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // Uses SHA256 to create the hash
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                // Convert the string to a byte array first, to be processed
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash.Substring(0, lenght - 1);
            }
        }
    }
}
