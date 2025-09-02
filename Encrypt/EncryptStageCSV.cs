using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR  // ✅ 빌드할 때 이 코드가 포함되지 않도록 설정
using UnityEditor;

public class EncryptAllCSV
{
    private static readonly string key = "MySecretKey12345"; // 16, 24, 32 바이트 키

    [MenuItem("Tools/Encrypt All Stage CSV")]
    public static void EncryptAllStages()
    {
        string folderPath = "Assets/StreamingAssets/Stage/";

        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("Stage 폴더가 존재하지 않습니다: " + folderPath);
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "Stage*Data.csv");
        foreach (string file in files)
        {
            EncryptCSVFile(file);
        }

        Debug.Log("모든 스테이지 CSV 파일이 암호화되었습니다.");
    }

    private static void EncryptCSVFile(string filePath)
    {
        string encryptedFilePath = filePath.Replace(".csv", ".bin");

        try
        {
            string csvData = File.ReadAllText(filePath);
            byte[] encryptedData = Encrypt(csvData);
            File.WriteAllBytes(encryptedFilePath, encryptedData);

            Debug.Log($"암호화 완료: {Path.GetFileName(filePath)} -> {Path.GetFileName(encryptedFilePath)}");
        }
        catch (Exception e)
        {
            Debug.LogError($"파일 암호화 중 오류 발생: {filePath}\n{e.Message}");
        }
    }

    private static byte[] Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = new byte[16]; // 기본 IV 설정

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                return ms.ToArray();
            }
        }
    }
}
#endif