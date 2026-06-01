using System;
using System.Security.Cryptography;
using System.Text;

public static class CryptoHelper
{
    private const int IvSize = 16;

    private static readonly byte[] Key = BuildKey("KimKwangHwan'sMiniGameProjectMetrovaniaGame");

    private static byte[] BuildKey(string passphrase)
    {
        using var sha = SHA256.Create();
        return sha.ComputeHash(Encoding.UTF8.GetBytes(passphrase));
    }

    public static byte[] Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.GenerateIV();
        byte[] iv = aes.IV;

        using var encryptor = aes.CreateEncryptor(aes.Key, iv);
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        byte[] result = new byte[iv.Length + cipherBytes.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, iv.Length, cipherBytes.Length);
        return result;
    }

    public static string Decrypt(byte[] cipherData)
    {
        if (cipherData == null || cipherData.Length <= IvSize)
            throw new ArgumentException("암호 데이터가 IV 크기보다 작거나 비어 있습니다.");

        byte[] iv = new byte[IvSize];
        Buffer.BlockCopy(cipherData, 0, iv, 0, IvSize);

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, iv);
        byte[] plainBytes = decryptor.TransformFinalBlock(
            cipherData, IvSize, cipherData.Length - IvSize);
        return Encoding.UTF8.GetString(plainBytes);
    }
}