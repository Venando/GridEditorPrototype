using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public static class FieldStock
{   
    public static void SaveField(IReadOnlyList<PieceInfo> list, int atIndex = -1)
    {
        if (atIndex == -1)
            atIndex = GetCount();
        var savingJson = JsonConvert.SerializeObject(list);
        var savingBytes = Encoding.UTF8.GetBytes(savingJson);
        IoHelper.WriteBytesToFile(savingBytes, atIndex.ToString());
    }

    public static List<PieceInfo> LoadAt(int index)
    {
        if (index >= GetCount())
            throw new IndexOutOfRangeException();
        var loadedBytes = IoHelper.ReadAllBytes(index.ToString());
        var loadedJson = Encoding.UTF8.GetString(loadedBytes);
        return JsonConvert.DeserializeObject<List<PieceInfo>>(loadedJson);
    }

    public static int GetCount()
    {
        return IoHelper.GetSavedFilesCount();
    }
}
