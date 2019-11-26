using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistantStorage : MonoBehaviour
{
    string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Save(PersistableObject pO)
    {
        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            pO.Save(new GameDataWriter(writer));
        }
    }
    public void Load(PersistableObject pO)
    {
        using (var reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            pO.Load(new GameDataReader(reader));
        }
    }
}
