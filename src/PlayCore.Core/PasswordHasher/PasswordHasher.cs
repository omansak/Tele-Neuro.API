﻿using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace PlayCore.Core.PasswordHasher
{
    public class PasswordHasher
    {
        /// <summary>
        /// Size of salt.
        /// </summary>
        private const int SaltSize = 16;

        /// <summary>
        /// Size of hash.
        /// </summary>
        private const int HashSize = 20;
        /// <summary>
        /// Hash format. (Must be includes {0},{1})
        /// </summary>
        private const string HashFormat = "$OMANSAK${0}#{1}$OMANSAK$";

        /// <summary>
        /// Creates a hash from a password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="iterations">Number of iterations.</param>
        /// <returns>The hash.</returns>
        public string Hash(string password, int iterations)
        {
            // Create salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(HashSize);

            // Combine salt and hash
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);

            // Format hash with extra information
            return string.Format(HashFormat, iterations, base64Hash);
        }

        /// <summary>
        /// Creates a hash from a password with 10000 iterations
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>The hash.</returns>
        public string Hash(string password)
        {
            return Hash(password, 10000);
        }

        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hashedPassword">The hash.</param>
        /// <returns>Could be verified?</returns>
        public bool Verify(string password, string hashedPassword)
        {
            // Check hash
            var parsedHash = ParseHashedString(hashedPassword);
            if (parsedHash == null)
                return false;

            // Extract iteration and Base64 string
            var iterations = parsedHash.Value.Iterations;
            var base64Hash = parsedHash.Value.Base64Hash;

            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash);

            // Get salt
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Get result
            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Parse hashed string if possible
        /// </summary>
        /// <param name="hashedString"></param>
        /// <returns></returns>
        private (int Iterations, string Base64Hash)? ParseHashedString(string hashedString)
        {
            var collections = Regex.Match(hashedString, string.Format(Regex.Escape(HashFormat).Replace(@"\{", "{"), @"(\d*)", "(.*)"));
            if (collections.Success)
            {
                return (int.Parse(collections.Groups[1].Value), collections.Groups[2].Value);
            }

            return null;
        }

    }
}
