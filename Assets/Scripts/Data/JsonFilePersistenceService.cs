using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonFilePersistenceService : IColorPersistenceService
{
    [System.Serializable]
    private class ColorEntry
    {
        public string id;
        public Color color;
    }

    [System.Serializable]
    private class ColorEntries
    {
        public List<ColorEntry> entries = new();
    }

    // Private variables
    private readonly string _filePath;

    public JsonFilePersistenceService(string filePath)
    {
        _filePath = filePath;
    }

    public Dictionary<string, Color> Load()
    {
        var dict = new Dictionary<string, Color>();
        if (!File.Exists(_filePath)) return dict;

        string json = File.ReadAllText(_filePath);
        var data = JsonUtility.FromJson<ColorEntries>(json);
        if (data == null) return dict;

        foreach (var entry in data.entries)
        {
            dict[entry.id] = entry.color;
        }

        return dict;
    }

    public void Save(Dictionary<string, Color> colors)
    {
        var data = new ColorEntries();
        foreach (var colorItem in colors)
            data.entries.Add(new ColorEntry { id = colorItem.Key, color = colorItem.Value });

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_filePath, json);
    }
}
