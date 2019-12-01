using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : PersistableObject
{
    public ShapeFactory shapeFacto;
    public PersistantStorage storage;
    public KeyCode createObj = KeyCode.C, newGame = KeyCode.N, save = KeyCode.E, load = KeyCode.R, destroy = KeyCode.Alpha1;
    List<Shape> shapes = new List<Shape>();
    string savePath;
    const int saveVersion = 1;
    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }
    float creationProgress, destructionProgress;

    private void Update()
    {
        if (Input.GetKey(createObj))
        {
            Debug.Log("Spawned");
            CreateShape();
        }
        else if (Input.GetKeyDown(newGame))
        {
            Debug.Log("New Game");
            BeginNewGame();
        }
        else if (Input.GetKeyDown(save))
        {
            Debug.Log("Saved");
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(load))
        {
            Debug.Log("Loaded");
            BeginNewGame();
            storage.Load(this);
        }
        else if (Input.GetKey(destroy))
        {
            Debug.Log("Destroy");
            DestroyShape();
        }

        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }
    }

    public void CreateShape()
    {
        Shape instance = shapeFacto.GetRandom();
        Transform t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 10;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(1, 3);
        instance.SetColor(Random.ColorHSV(hueMin: 0f, hueMax: 1f, saturationMin: 0.5f, saturationMax: 1f, valueMin: 0.25f, valueMax: 1f, alphaMin: 1f, alphaMax: 1f));
        shapes.Add(instance);
    }

    void BeginNewGame()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            shapeFacto.Reclaim(shapes[i]);
        }
        shapes.Clear();
    }

    void DestroyShape()
    {
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            shapeFacto.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);

        writer.Write(shapes.Count);

        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;

        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }

        int count = version <= 0 ? -version : reader.ReadInt();

        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFacto.Get(shapeId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }
}
