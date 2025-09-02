using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class DecryptStageCSV
{
    private static readonly string key = "MySecretKey12345"; // AES 암호화 키 (16, 24, 32 바이트)

    public static string DecryptFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"파일이 존재하지 않습니다: {filePath}");
            return null;
        }

        try
        {
            byte[] encryptedData = File.ReadAllBytes(filePath);
            return Decrypt(encryptedData);
        }
        catch (Exception e)
        {
            Debug.LogError($"파일 복호화 중 오류 발생: {e.Message}");
            return null;
        }
    }

    private static string Decrypt(byte[] cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = new byte[16]; // IV는 0으로 설정

            using (MemoryStream ms = new MemoryStream(cipherText))
            using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
