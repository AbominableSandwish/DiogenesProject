using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

class FileSystem : MonoBehaviour
{
    public static void WriteFile(string folder, string fileName, string data)
    {
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, fileName + ".json");
        File.WriteAllText(path, data);

        Debug.Log($"🌍 World.json saved: {path}");
    }

    public static string ReadFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"❌ File not found : {path}");
            return "";
        }

        return File.ReadAllText(path);
    }

    public static List<string> GetFiles(string root, string pattern)
    {
        var list = new List<string>();
        if (!Directory.Exists(root)) return list;

        foreach (var path in Directory.GetFiles(root, pattern, SearchOption.TopDirectoryOnly))
            list.Add(Path.GetFileName(path));

        return list;
    }

    public void OpenPersistentDataPath()
    {
        Application.OpenURL(Application.persistentDataPath);
    }
}

