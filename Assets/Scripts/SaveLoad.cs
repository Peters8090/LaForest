using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Runtime.Serialization;
using System.Reflection;

public class SaveLoad : MonoBehaviour
{
    public static SaveData sd;

    void Awake()
    {
        if (!FileSaveLoad.Load())
        {
            sd = new SaveData();
        }
    }

    void Start()
    {
        InvokeRepeating("Save", 1.0f, 0.25f);
    }

    void Save()
    {
        sd.volume = GameSettings.volume;
        sd.graphicsIndex = GameSettings.graphicsIndex;
        sd.resolutionIndex = GameSettings.resolutionIndex;
        sd.showGUI = GameSettings.showGUI;
        sd.showFPS = GameSettings.showFPS;
        sd.fullscreen = GameSettings.fullscreen;
        sd.nick = GameSettings.nick;

        if (UsefulReferences.initialized)
        {
            sd.health = UsefulReferences.playerHealth.health;
            sd.playerPosX = UsefulReferences.player.transform.position.x;
            sd.playerPosY = UsefulReferences.player.transform.position.y;
            sd.playerPosZ = UsefulReferences.player.transform.position.z;
            sd.playerRotX = UsefulReferences.mainCamera.transform.localEulerAngles.x;
            sd.playerRotY = UsefulReferences.player.transform.localEulerAngles.y;
        }

        FileSaveLoad.Save();
    }
}

[Serializable()]
public class SaveData : ISerializable
{
    //default values
    public float volume = 1f;
    public int graphicsIndex = 3;
    public int resolutionIndex = 0;
    public bool showGUI = true;
    public bool showFPS = false;
    public bool fullscreen = true;
    public string nick = "";
    public float health = 100f;
    public float playerPosX = UsefulReferences.playerResources.transform.position.x;
    public float playerPosY = UsefulReferences.playerResources.transform.position.y;
    public float playerPosZ = UsefulReferences.playerResources.transform.position.z;
    public float playerRotX = UsefulReferences.playerResources.transform.localEulerAngles.x;
    public float playerRotY = UsefulReferences.playerResources.transform.localEulerAngles.y;
    
    public SaveData() { }

    //assigning variables data from info (loading data)
    public SaveData(SerializationInfo info, StreamingContext ctxt)
    {
        volume = (float)info.GetValue("volume", typeof(float));
        graphicsIndex = (int)info.GetValue("graphicsIndex", typeof(int));
        resolutionIndex = (int)info.GetValue("resolutionIndex", typeof(int));
        showGUI = (bool)info.GetValue("showGUI", typeof(bool));
        showFPS = (bool)info.GetValue("showFPS", typeof(bool));
        fullscreen = (bool)info.GetValue("fullscreen", typeof(bool));
        nick = (string)info.GetValue("nick", typeof(string));
        health = (float)info.GetValue("health", typeof(float));
        playerPosX = (float)info.GetValue("playerPosX", typeof(float));
        playerPosY = (float)info.GetValue("playerPosY", typeof(float));
        playerPosZ = (float)info.GetValue("playerPosZ", typeof(float));
        playerRotX = (float)info.GetValue("playerRotX", typeof(float));
        playerRotY = (float)info.GetValue("playerRotY", typeof(float));
    }

    //adding values to info (saving data)
    public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
        info.AddValue("volume", volume);
        info.AddValue("graphicsIndex", graphicsIndex);
        info.AddValue("resolutionIndex", resolutionIndex);
        info.AddValue("showGUI", showGUI);
        info.AddValue("showFPS", showFPS);
        info.AddValue("fullscreen", fullscreen);
        info.AddValue("nick", nick);
        info.AddValue("health", health);
        info.AddValue("playerPosX", playerPosX);
        info.AddValue("playerPosY", playerPosY);
        info.AddValue("playerPosZ", playerPosZ);
        info.AddValue("playerRotX", playerRotX);
        info.AddValue("playerRotY", playerRotY);
    }
}

public class FileSaveLoad
{

    public static string currentFilePath = Application.persistentDataPath + @"/saveData.dat";

    public static void Save()
    {
        Save(currentFilePath);
    }

    public static void Save(string filePath)
    {
        Stream stream = File.Open(filePath, FileMode.Create);
        SaveData data = SaveLoad.sd;
        BinaryFormatter bformatter = new BinaryFormatter();
        bformatter.Binder = new VersionDeserializationBinder();
        bformatter.Serialize(stream, data);
        stream.Close();
    }

    public static bool Load()
    {
        if (File.Exists(currentFilePath))
        {
            Load(currentFilePath);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void Load(string filePath)
    {
        Stream stream = File.Open(filePath, FileMode.Open);
        SaveData data = new SaveData();
        BinaryFormatter bformatter = new BinaryFormatter();
        bformatter.Binder = new VersionDeserializationBinder();
        data = (SaveData)bformatter.Deserialize(stream);
        SaveLoad.sd = data;
        stream.Close();
    }
}

public sealed class VersionDeserializationBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
        {
            Type typeToDeserialize = null;
            assemblyName = Assembly.GetExecutingAssembly().FullName;
            typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
            return typeToDeserialize;
        }
        return null;
    }
}
