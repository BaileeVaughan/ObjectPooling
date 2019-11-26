using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : PersistableObject
{
    public PersistableObject prefab;
    public PersistantStorage storage;
    public KeyCode createObj = KeyCode.C, newGame = KeyCode.N, save = KeyCode.E, load = KeyCode.R;
    List<PersistableObject> objects = new List<PersistableObject>();
    string savePath;

    private void Update()
    {
        if (Input.GetKey(createObj))
        {
            Debug.Log("Spawned");
            CreateObject();
        }
        else if (Input.GetKeyDown(newGame))
        {
            Debug.Log("New Game");
            BeginNewGame();
        }
        else if (Input.GetKeyDown(save))
        {
            Debug.Log("Saved");
            storage.Save(this);
        }
        else if (Input.GetKeyDown(load))
        {
            Debug.Log("Loaded");
            BeginNewGame();
            storage.Load(this);
        }
    }

    public void CreateObject()
    {
        PersistableObject o = Instantiate(prefab);
        Transform t = o.transform;
        t.localPosition = Random.insideUnitSphere * 10;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(1, 3);
        objects.Add(o);
    }

    void BeginNewGame()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(objects.Count);
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].Save(writer);
        }
    }
    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            PersistableObject o = Instantiate(prefab);
            o.Load(reader);
            objects.Add(o);
        }
    }
}
