using System;
using System.Security.Cryptography;
using Jinget.Core.Exceptions;
using Jinget.Core.Security.Hashing.Model;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Jinget.Core.Security.Hashing;

/// <summary>
/// Hash string using Pbkdf2 algorithm. This class cannot be inherited.
/// Implements the <see cref="Jinget.Core.Security.Hashing.IHashManager" />
/// more about this algorithm: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-2.1
/// </summary>
/// <seealso cref="Jinget.Core.Security.Hashing.IHashManager" />
public sealed class HashManager : IHashManager
{
    /// <summary>
    /// Ares the equal.
    /// </summary>
    /// <param name="hashModel">result in this model is currently hashed using it's salt value</param>
    /// <param name="destString">string to be hashed using hashModel's salt value</param>
    /// <returns>if the hashModel's Result property and the result of the destString hashing process are same then true will be
    /// returned</returns>
    /// <exception cref="JingetException">Jinget Says: {nameof(hashModel.Salt)} can not be null or empty - 1000</exception>
    /// <exception cref="JingetException">Jinget Says: source hashed value can not be null or empty - 1000</exception>
    /// <exception cref="JingetException">Jinget Says: {nameof(destString)} can not be null or empty - 1000</exception>
    /// <exception cref="JingetException">Jinget Says: given {nameof(hashModel.Salt)} is not a valid base64 string - 1000</exception>
    public bool AreEqual(HashModel hashModel, string destString)
    {
        if (string.IsNullOrEmpty(hashModel.Salt))
            throw new JingetException($"Jinget Says: {nameof(hashModel.Salt)} can not be null or empty", 1000);
        if (string.IsNullOrEmpty(hashModel.HashedValue))
            throw new JingetException("Jinget Says: source hashed value can not be null or empty", 1000);
        if (string.IsNullOrEmpty(destString))
            throw new JingetException($"Jinget Says: {nameof(destString)} can not be null or empty", 1000);

        byte[] originalSalt;
        try
        {
            originalSalt = Convert.FromBase64String(hashModel.Salt);
        }
        catch (FormatException ex)
        {
            throw new JingetException($"Jinget Says: given {nameof(hashModel.Salt)} is not a valid base64 string", ex, 1000);
        }

        var destHashResult = Hash(originalSalt, destString);

        return hashModel.HashedValue.Equals(Convert.ToBase64String(destHashResult));
    }

    /// <summary>
    /// Hashes the specified input.
    /// </summary>
    public HashModel Hash(string input)
    {
        var salt = GenerateSalt();

        var hashed = Hash(salt, input);

        return new HashModel
        {
            Salt = Convert.ToBase64String(salt),
            HashedValue = Convert.ToBase64String(hashed)
        };
    }

    /// <summary>
    /// Hashes the specified input.
    /// </summary>
    public string Hash(string input, string salt)
    {
        byte[] decryptedSalt = Convert.FromBase64String(salt);
        byte[] unhashedInput = Hash(decryptedSalt, input);
        string hashedInput = Convert.ToBase64String(unhashedInput);
        return hashedInput;
    }

    /// <summary>
    /// Hashes the specified input with givent salt.
    /// </summary>
    private byte[] Hash(byte[] salt, string input) => KeyDerivation.Pbkdf2(input, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8);

    /// <summary>
    /// Generates random salt value
    /// </summary>
    private byte[] GenerateSalt()
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        return salt;
    }
}