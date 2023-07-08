using System.Security.Cryptography;
using System.Text;

namespace WebApi.Services;
public static class Encryption
{
    public static byte[] CreateSHA256Hash(string inputString, byte[] salt)
    {
        using HashAlgorithm algorithm = SHA256.Create();
        return GetHash(inputString, salt, algorithm);
    }

    public static byte[] GetHash(string inputString, byte[] salt, HashAlgorithm algorithm)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
        byte[] saltedInputBytes = inputBytes.Concat(salt).ToArray();
        return algorithm.ComputeHash(saltedInputBytes);
    }
    public static byte[] GenerateSalt()
    {
        var random = RandomNumberGenerator.Create();
        int max_length = 32;
        byte[] salt = new byte[max_length];

        random.GetNonZeroBytes(salt);
        return salt;
    }
}