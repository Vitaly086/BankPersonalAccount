using System.Security.Cryptography;
using System.Text;

namespace BankAccountService.Services;

public class ClientIdEncryptor
{
    public static string EncryptClientId(int clientId, string key)
    {
        using var aesAlg = Aes.Create();
        using var sha = SHA256.Create();

        var hashedKey = sha.ComputeHash(Encoding.UTF8.GetBytes(key));

        aesAlg.Key = hashedKey;
        aesAlg.IV = new byte[16];

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        var clientIdBytes = BitConverter.GetBytes(clientId);

        var encryptedClientIdBytes = encryptor.TransformFinalBlock(clientIdBytes, 0, clientIdBytes.Length);

        return Convert.ToBase64String(encryptedClientIdBytes);
    }

    public static int DecryptClientId(string encryptedClientId, string key)
    {
        using var aesAlg = Aes.Create();
        using var sha = SHA256.Create();

        var hashedKey = sha.ComputeHash(Encoding.UTF8.GetBytes(key));

        aesAlg.Key = hashedKey;
        aesAlg.IV = new byte[16];

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        var encryptedClientIdBytes = Convert.FromBase64String(encryptedClientId);

        var decryptedClientIdBytes =
            decryptor.TransformFinalBlock(encryptedClientIdBytes, 0, encryptedClientIdBytes.Length);

        return BitConverter.ToInt32(decryptedClientIdBytes, 0);
    }
}