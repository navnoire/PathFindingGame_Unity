using System.Collections;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class SafePlayerPrefs
{
    private string key;
    private List<string> properties = new List<string>();

    public SafePlayerPrefs(string key, params string[] properties)
    {
        this.key = key;
        foreach (string property in properties)
            this.properties.Add(property);
    }

    // Вычисляем контрольную сумму
    private string GenerateChecksum()
    {
        string hash = "";
        foreach (string property in properties)
        {
            hash += property + ":";
            if (PlayerPrefs.HasKey(property))
                hash += PlayerPrefs.GetString(property);
        }
        return Md5Sum(hash + key);
    }

    // Сохраняем контрольную сумму
    public void Save()
    {
        string checksum = GenerateChecksum();
        //TODO Заменить "CHECKSUM"  на что-то более удобоваримое
        PlayerPrefs.SetString("CHECKSUM", checksum);
        PlayerPrefs.Save();
    }

    // Проверяем, изменялись ли данные
    public bool HasBeenEdited()
    {
        if (!PlayerPrefs.HasKey("CHECKSUM"))
            return true;

        string checksumSaved = PlayerPrefs.GetString("CHECKSUM");
        string checksumReal = GenerateChecksum();

        //Debug.Log("Saved checksum = " + checksumSaved + ", realCheckSum = " + checksumReal + " this checksums are equal? " + checksumSaved.Equals(checksumReal));

        return !checksumSaved.Equals(checksumReal);
    }

    // Хеш создаем
    private string Md5Sum(string source)
    {
        MD5 md5Hash = MD5.Create();
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sb.Append(data[i].ToString("x2"));
        }

        return sb.ToString();
    }
}
