using System;
using System.IO;
using UnityEngine;

public static class IoHelper
{
    private static readonly string SaveDirectory = Application.persistentDataPath + "/SaveData";

    public static int GetSavedFilesCount()
    {
        Directory.CreateDirectory(SaveDirectory);
        return Directory.GetFiles(SaveDirectory).Length;
    }
    
    public static void WriteBytesToFile(byte[] bytes, string fileName)
    {
        var filePath = Path.Combine(SaveDirectory, fileName);
        var file = File.Open(filePath, FileMode.Create);
        var binary = new BinaryWriter(file);
        binary.Write(bytes);
        file.Close();
    }
    
    public static async void ReadAllBytes(string fileName, Action<byte[]> successCallback)
    {
        byte[] result;
        var filePath = Path.Combine(SaveDirectory, fileName);
        using (var stream = File.Open(filePath, FileMode.Open))
        {
            result = new byte[stream.Length];
            await stream.ReadAsync(result, 0, (int)stream.Length);
        }
        successCallback.Invoke(result);
    }
    
    public static byte[] ReadAllBytes(string fileName)
    {
        var filePath = Path.Combine(SaveDirectory, fileName);
        if (!File.Exists(filePath))
            return new byte[]{};
        byte[] result;
        using (var stream = File.Open(filePath, FileMode.Open))
        {
            result = new byte[stream.Length];
            stream.Read(result, 0, (int)stream.Length);
        }

        return result;
    }
}