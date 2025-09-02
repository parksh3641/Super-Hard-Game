using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR  // ✅ 빌드할 때 이 코드가 포함되지 않도록 설정
using UnityEditor;

public class EncryptLocalization
{
    private static readonly string key = "MySecretKey12345"; // 16, 24, 32 바이트 키

    [UnityEditor.MenuItem("Tools/Encrypt Localization")]
    public static void EncryptLocalizationText()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Localization.txt");
        string encryptedFilePath = Path.Combine(Application.streamingAssetsPath, "Localization.bin");

        if (!File.Exists(filePath))
        {
            Debug.LogError("Localization.txt 파일이 존재하지 않습니다.");
            return;
        }

        string fileContents = File.ReadAllText(filePath);
        byte[] encryptedData = Encrypt(fileContents);

        File.WriteAllBytes(encryptedFilePath, encryptedData);
        Debug.Log("Localization 파일이 암호화되었습니다.");
    }

    private static byte[] Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = new byte[16]; // IV는 0으로 설정

            using (MemoryStream ms = new MemoryStream()) // ✅ MemoryStream을 명확히 선언
            {
                using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return ms.ToArray(); // ✅ using 블록이 끝나기 전에 바이트 배열로 변환 후 반환
            }
        }
    }
}
#endif