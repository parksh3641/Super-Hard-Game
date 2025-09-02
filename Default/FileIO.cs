using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using System.IO;

public static class FileIO
{
    // Ű�� ����ϱ� ���� ��ȣ ����
    private static readonly string PASSWORD = "3os1s334e3kcc7c4yf4554e632983h";
    // ����Ű ����
    private static readonly string KEY = PASSWORD.Substring(0, 128 / 8);

    /// <summary>
    /// ������ ����, ��ȣȭ ����(��ȣȭ �ڵ�)
    /// </summary>
    /// <param name="fileName">���ϸ�</param>
    /// <param name="data">������</param>
    /// <param name="withEncryption">��ȣȭ ����</param>
    public static void SaveData(string fileName, string data, bool withEncryption = false)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new System.ArgumentException("Parameter cannot be null", "fileName");
        }
        if (string.IsNullOrEmpty(data))
        {
            throw new System.ArgumentException("Parameter cannot be null", "data");
        }

        if (withEncryption)
        {
            SaveData(fileName, Encrypt(data));
        }
        else
        {
            SaveData(fileName, data);
        }
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    /// <param name="fileName">���ϸ�</param>
    /// <param name="data">������</param>
    private static void SaveData(string fileName, string data)
    {
        string path = pathForDocumensFile(fileName);
        FileStream file = File.Open(path, FileMode.Create, FileAccess.Write);

        StreamWriter sw = new StreamWriter(file);
        sw.WriteLine(data);
        sw.Close();
        file.Close();
    }

    /// <summary>
    /// ������ �ε�, ��ȣȭ ����
    /// </summary>
    /// <param name="fileName">���ϸ�</param>
    /// <param name="withDecryption">��ȣȭ ����</param>
    /// <returns></returns>
    public static string LoadData(string fileName, bool withDecryption = false)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new System.ArgumentException("Parameter cannot be null", "fileName");
        }
        if (withDecryption)
        {
            string load = LoadData(fileName);
            if (!string.IsNullOrEmpty(load))
            {
                load = Decrypt(load);
            }
            return load;
        }
        else
        {
            return LoadData(fileName);
        }
    }

    /// <summary>
    /// ������ �ε�
    /// </summary>
    /// <param name="fileName">���ϸ�</param>
    /// <returns></returns>
    private static string LoadData(string fileName)
    {
        string data = string.Empty;
        string path = pathForDocumensFile(fileName);

        Debug.Log("���� ��� : " + path);

        if (File.Exists(path))
        {
            FileStream file = File.Open(path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);

            data = sr.ReadLine();

            sr.Close();
            file.Close();
        }
        return data;
    }

    public static string LoadDataWithPath(string filePath)
    {
        string data = string.Empty;
        if (File.Exists(filePath))
        {
            FileStream file = File.Open(filePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);

            data = sr.ReadLine();

            sr.Close();
            file.Close();
        }
        return data;
    }

    /// <summary>
    /// ������ ���� ���� �Լ�
    /// </summary>
    /// <param name="fileName">������ ������ �Լ� �̸�</param>
    /// <returns>0�� ������ ����. 1�� ���� ����</returns>
    public static bool DeleteData(string fileName)
    {
        string path = pathForDocumensFile(fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// ���� ��ġ ã�� �Լ� �ܺ� ȣ����� �ʴ´�.
    /// </summary>
    /// <param name="FileName">ã������</param>
    /// <returns>�ش� ���� ���</returns>
    private static string pathForDocumensFile(string FileName)
    {
        string path = null;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = Application.persistentDataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, FileName);
            case RuntimePlatform.IPhonePlayer:
                path = Application.persistentDataPath;
                //path = Application.persistentDataPath.Substring(0, Application.dataPath.Length - 5); ;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(Path.Combine(path, "Documents"), FileName);
            default:
                path = Application.dataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, FileName);
        }
    }

    /// <summary>
    /// ��ȣȭ �ڵ�
    /// </summary>
    /// <param name="data">�Է¹��� ������</param>
    /// <returns>��ȣȭ�� ������</returns>
    private static string Encrypt(string data)
    {
        byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(data);

        RijndaelManaged myRijndael = new RijndaelManaged();
        myRijndael.Mode = CipherMode.CBC;
        myRijndael.Padding = PaddingMode.PKCS7;
        myRijndael.KeySize = 128;

        MemoryStream memoryStream = new MemoryStream();

        ICryptoTransform encryptor = myRijndael.CreateEncryptor(System.Text.Encoding.UTF8.GetBytes(KEY), System.Text.Encoding.UTF8.GetBytes(KEY));

        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
        cryptoStream.FlushFinalBlock();

        byte[] encryptBytes = memoryStream.ToArray();

        string encryptString = System.Convert.ToBase64String(encryptBytes);

        cryptoStream.Close();
        memoryStream.Close();

        return encryptString;
    }



    /// <summary>
    /// ��ȣȭ �ڵ�
    /// </summary>
    /// <param name="data">�Է¹��� ��ȣȭ�� ������</param>
    /// <returns>��ȣȭ�� ������</returns>
    private static string Decrypt(string data)
    {
        byte[] encryptBytes = System.Convert.FromBase64String(data);

        RijndaelManaged myRijndael = new RijndaelManaged();
        myRijndael.Mode = CipherMode.CBC;
        myRijndael.Padding = PaddingMode.PKCS7;
        myRijndael.KeySize = 128;

        MemoryStream memoryStream = new MemoryStream(encryptBytes);

        ICryptoTransform decryptor = myRijndael.CreateDecryptor(System.Text.Encoding.UTF8.GetBytes(KEY), System.Text.Encoding.UTF8.GetBytes(KEY));

        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        byte[] plainBytes = new byte[encryptBytes.Length];

        int plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

        string plainString = System.Text.Encoding.UTF8.GetString(plainBytes, 0, plainCount);

        cryptoStream.Close();
        memoryStream.Close();

        return plainString;
    }


}