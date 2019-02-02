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
        if(!FileSaveLoad.Load())
        {
            sd = new SaveData();
            sd.volume = GameSettings.volume;
            sd.graphicsIndex = GameSettings.graphicsIndex;
            sd.resolutionIndex = GameSettings.resolutionIndex;
            sd.showGUI = GameSettings.showGUI;
            sd.showFPS = GameSettings.showFPS;
            sd.fullscreen = GameSettings.fullscreen;
            sd.nick = GameSettings.nick;
        }
    }

    
    void Update()
    {
        if(GameSettings.loaded)
        {
            sd.volume = GameSettings.volume;
            sd.graphicsIndex = GameSettings.graphicsIndex;
            sd.resolutionIndex = GameSettings.resolutionIndex;
            sd.showGUI = GameSettings.showGUI;
            sd.showFPS = GameSettings.showFPS;
            sd.fullscreen = GameSettings.fullscreen;
            sd.nick = GameSettings.nick;
        }
        
        FileSaveLoad.Save();
    }
}

[Serializable ()]
public class SaveData : ISerializable
{
    public float volume;
    public int graphicsIndex;
    public int resolutionIndex;
    public bool showGUI;
    public bool showFPS;
    public bool fullscreen;
    public string nick;

    public SaveData() { }

    public SaveData(SerializationInfo info, StreamingContext ctxt)
    {
		volume = (float)info.GetValue("volume", typeof(float));
        graphicsIndex = (int)info.GetValue("graphicsIndex", typeof(int));
        resolutionIndex = (int)info.GetValue("resolutionIndex", typeof(int));
        showGUI = (bool)info.GetValue("showGUI", typeof(bool));
        showFPS = (bool)info.GetValue("showFPS", typeof(bool));
        fullscreen = (bool)info.GetValue("fullscreen", typeof(bool));
        nick = (string)info.GetValue("nick", typeof(string));
    }
 
    public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
		info.AddValue("volume", volume);
		info.AddValue("graphicsIndex", graphicsIndex);
		info.AddValue("resolutionIndex", resolutionIndex);
		info.AddValue("showGUI", showGUI);
		info.AddValue("showFPS", showFPS);
		info.AddValue("fullscreen", fullscreen);
		info.AddValue("nick", nick);
    }
}

public class FileSaveLoad {

	public static string currentFilePath = Application.persistentDataPath + @"/SaveData.cjc";
  
	public static void Save()
	{
		Save (currentFilePath);
	}

	public static void Save(string filePath)
	{
		SaveData data = SaveLoad.sd;
		Stream stream = File.Open(filePath, FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder(); 
		bformatter.Serialize(stream, data);
		stream.Close();
	}
	
	public static bool Load ()
    {
        if (File.Exists(currentFilePath))
        {
            Load(currentFilePath);
            return true;
        } else
        {
            return false;
        }
	}

	public static void Load (string filePath) 
	{
		SaveData data = new SaveData ();
		Stream stream = File.Open(filePath, FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder(); 
		data = (SaveData)bformatter.Deserialize(stream);
        SaveLoad.sd = data;
		stream.Close();
	}
}

public sealed class VersionDeserializationBinder : SerializationBinder 
{ 
    public override Type BindToType( string assemblyName, string typeName )
    { 
        if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName)) 
        { 
            Type typeToDeserialize = null; 
            assemblyName = Assembly.GetExecutingAssembly().FullName; 
            typeToDeserialize = Type.GetType( String.Format( "{0}, {1}", typeName, assemblyName ) ); 
            return typeToDeserialize; 
        } 
        return null; 
    } 
}
