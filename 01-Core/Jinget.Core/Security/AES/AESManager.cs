using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Jinget.Core.Exceptions;

namespace Jinget.Core.Security.AES;

/// <summary>
/// Encrypt & Decrypt string using System.Security.Cryptography.RijndaelManaged algorithm
/// </summary>
public static class AESManager
{
    /// <summary>
    /// Encrypts the specified plain text.
    /// </summary>
    /// <exception cref="Jinget.Core.Exceptions.JingetException">Jinget Says: {nameof(plainText)} cannot be null or empty - 1000</exception>
    /// <exception cref="Jinget.Core.Exceptions.JingetException">Jinget Says: {nameof(key)} cannot be null or empty - 1000</exception>
    /// <exception cref="Jinget.Core.Exceptions.JingetException">Jinget Says: {nameof(iv)} cannot be null or empty - 1000</exception>
    public static string Encrypt(string plainText, string key, string iv)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var ivBytes = Encoding.UTF8.GetBytes(iv);

        if (plainText == null || plainText.Length <= 0)
            throw new JingetException($"Jinget Says: {nameof(plainText)} cannot be null or empty", 1000);
        if (keyBytes == null || keyBytes.Length <= 0 || key.Length < 32)
            throw new JingetException($"Jinget Says: {nameof(key)} cannot be null or empty and should have at least 32 chars", 1000);
        if (ivBytes == null || ivBytes.Length <= 0 || iv.Length != 16)
            throw new JingetException($"Jinget Says: {nameof(iv)} cannot be null or empty and should have 16 chars", 1000);

        byte[] encrypted;

        using (var rijAlg = Aes.Create())
        {
            rijAlg.Key = keyBytes;
            rijAlg.IV = ivBytes;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for encryption. 
            using MemoryStream msEncrypt = new();
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                //Write all data to the stream.
                swEncrypt.Write(plainText);
            }
            encrypted = msEncrypt.ToArray();
        }
        // Return the encrypted bytes from the memory stream. 
        return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    /// Decrypts the specified input.
    /// </summary>
    /// <exception cref="Jinget.Core.Exceptions.JingetException">Jinget Says: {nameof(input)} is not a valid base64 string - 1000</exception>
    /// <exception cref="Jinget.Core.Exceptions.JingetException">Jinget Says: {nameof(input)} cannot be null or empty, or has invalid value - 1000</exception>
    /// <exception cref="Jinget.Core.Exceptions.JingetException">Jinget Says: {nameof(key)} cannot be null or empty - 1000</exception>
    /// <exception cref="Jinget.Core.Exceptions.JingetException">Jinget Says: {nameof(iv)} cannot be null or empty - 1000</exception>
    /// <exception cref="Jinget.Core.Exceptions.JingetException">Jinget Says: {nameof(key)}/{nameof(iv)} is not a valid key/iv - 1000</exception>
    public static string Decrypt(string input, string key, string iv)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var ivBytes = Encoding.UTF8.GetBytes(iv);

        byte[] cipherText;
        try
        {
            cipherText = Convert.FromBase64String(input);
        }
        catch (FormatException ex)
        {
            throw new JingetException($"Jinget Says: {nameof(input)} is not a valid base64 string", ex, 1000);
        }

        // Check arguments.  
        if (cipherText == null || cipherText.Length <= 0)
            throw new JingetException($"Jinget Says: {nameof(input)} cannot be null or empty, or has invalid value", 1000);
        if (key == null || key.Length <= 0)
            throw new JingetException($"Jinget Says: {nameof(key)} cannot be null or empty", 1000);
        if (iv == null || iv.Length <= 0)
            throw new JingetException($"Jinget Says: {nameof(iv)} cannot be null or empty", 1000);

        string plaintext;

        using (var rijAlg = Aes.Create())
        {
            try
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = keyBytes;
                rijAlg.IV = ivBytes;

                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                using var msDecrypt = new MemoryStream(cipherText);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                plaintext = srDecrypt.ReadToEnd();
            }
            catch (CryptographicException ex)
            {
                throw new JingetException($"Jinget Says: {nameof(key)}/{nameof(iv)} is not a valid key/iv", ex, 1000);
            }
        }

        return plaintext;
    }
}