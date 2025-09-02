using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class DecryptStageCSV
{
    private static readonly string key = "MySecretKey12345"; // AES ��ȣȭ Ű (16, 24, 32 ����Ʈ)

    public static string DecryptFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"������ �������� �ʽ��ϴ�: {filePath}");
            return null;
        }

        try
        {
            byte[] encryptedData = File.ReadAllBytes(filePath);
            return Decrypt(encryptedData);
        }
        catch (Exception e)
        {
            Debug.LogError($"���� ��ȣȭ �� ���� �߻�: {e.Message}");
            return null;
        }
    }

    private static string Decrypt(byte[] cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = new byte[16]; // IV�� 0���� ����

            using (MemoryStream ms = new MemoryStream(cipherText))
            using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
