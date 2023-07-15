using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Jinget.Core.Utilities
{
    public static class StringUtility
    {
        /// <summary>
        /// Create a random string with given <see cref="length"/> using given <see cref="characterSet"/>
        /// </summary>
        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("Jinget Says: length must not be negative", nameof(length));
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("Jinget Says: length is too big", nameof(length));
            if (characterSet == null)
                throw new ArgumentNullException(nameof(characterSet));
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("Jinget Says: characterSet must not be empty", nameof(characterSet));

            var bytes = new byte[length * 8];
            var result = new char[length];
            using (var cryptoProvider = RandomNumberGenerator.Create())
            {
                cryptoProvider.GetBytes(bytes);
            }
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }
    }
}
