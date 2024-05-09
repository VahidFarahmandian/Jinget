using Jinget.Core.Security.Hashing.Model;

namespace Jinget.Core.Security.Hashing;

public interface IHashManager
{
    HashModel Hash(string input);

    string Hash(string input, string salt);

    /// <param name="hashModel">result in this model is currently hashed using it's salt value</param>
    /// <param name="destString">string to be hashed using hashModel's salt value</param>
    /// <returns>
    ///     if the hashModel's Result property and the result of the destString hashing process are same then true will be
    ///     returned
    /// </returns>
    bool AreEqual(HashModel hashModel, string destString);
}