using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper?.Items;
    }

    public static string ToJson<T>(List<T> list)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = list;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(List<T> list, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = list;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }
}
public class SaveDataManager : MonoBehaviour
{
    public delegate void OnFirstTimePack(SavedData savedData);
    public static event OnFirstTimePack onFirstTimePack;
    public delegate void OnCreateCustomPacks(string name);
    public static event OnCreateCustomPacks onCreateCustomPacks;

    private string saveFolderName = "Saved_Data_JSON";
    private string savedDataFileName = "SaveData.Json";
    private string CustomPackDataName = "CustomPacksData";

    public static SavedData savedData = new SavedData();
    public static List<CustomPacksData> customPacks = new List<CustomPacksData>();

    private string savedDatafilePath;
    private string customPacksDatafilePath;
    private bool firstTime=true;
    private void Awake()
    {
        savedDatafilePath = Application.persistentDataPath + "/" + saveFolderName + "/" + savedDataFileName;
        customPacksDatafilePath= Application.persistentDataPath + "/" + saveFolderName + "/" + CustomPackDataName + ".Json";

        if (!File.Exists(savedDatafilePath))
        {
            DefaultPack.onAwake += CreateJsonPack;
        }
        else
        {
            savedData = LoadData();
        }
        if (!File.Exists(customPacksDatafilePath))
        {
            SaveListJson(customPacks, CustomPackDataName);
        }
        else
        {
            customPacks = LoadListJson<CustomPacksData>("CustomPacksData");
            CreateSavedCustomPacks();
        }
        DefaultPack.onLoadPack += LoadListJson<PackData>;

        SnapToSlot.onNumberSnapedOnStartGame += GetSavedData;
        SpyesSnapToSlot.onNumberSnapedOnStartGame += GetSavedData;
        MenuManager.onLoadSavedDataToUI += GetSavedData;
        MenuManager.onPackChange += SaveListJson;
        MenuManager.onSetGame += SaveData;

        NewPack.onLoadPack += LoadListJson<PackData>;
        NewPack.onAddNewPack += AddCustomPack;
        NewPack.onSavePack += SaveListJson;
        NewPack.onDeleteThisPack += RemoveSavedCustomPack;
    }
    private void OnDestroy()
    {
        DefaultPack.onAwake -= CreateJsonPack;
        DefaultPack.onLoadPack -= LoadListJson<PackData>;

        SnapToSlot.onNumberSnapedOnStartGame -= GetSavedData;
        SpyesSnapToSlot.onNumberSnapedOnStartGame -= GetSavedData;
        MenuManager.onLoadSavedDataToUI -= GetSavedData;
        MenuManager.onPackChange -= SaveListJson;
        MenuManager.onSetGame -= SaveData;

        NewPack.onLoadPack -= LoadListJson<PackData>;
        NewPack.onAddNewPack -= AddCustomPack;
        NewPack.onSavePack -= SaveListJson;
        NewPack.onDeleteThisPack -= RemoveSavedCustomPack;
    }

    #region Save Data Manager
    public void SaveData(SavedData currentSavedData)
    {
        if (!File.Exists(savedDatafilePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savedDatafilePath));
        }
        
        string dataInJSON = JsonUtility.ToJson(currentSavedData, true);
        FileStream fs = new FileStream(savedDatafilePath, FileMode.Create);

        StreamWriter sw = new StreamWriter(fs);

        sw.Write(dataInJSON);

        sw.Close();
        fs.Close();
    }
    public SavedData LoadData()
    { 
        string dataToLoad = "";

        if (File.Exists(savedDatafilePath))
        {
            FileStream fs = new FileStream(savedDatafilePath, FileMode.Open);

            StreamReader sr = new StreamReader(fs);

            dataToLoad = sr.ReadToEnd();

            if (dataToLoad != null)
            {
                SavedData loadedData = JsonUtility.FromJson<SavedData>(dataToLoad);

                sr.Close();
                fs.Close();

                return loadedData;
            }
            else
                return null;
        }
        return null;
    }
    private SavedData GetSavedData()
    {
        return savedData;
    }
    #endregion

    #region Save List
    public void SaveListJson<T>(List<T> currentPack, string packName)
    {
        string filePath = Application.persistentDataPath + "/" + saveFolderName + "/" + packName + ".Json";

        if (!File.Exists(filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        string dataInJSON = JsonHelper.ToJson(currentPack, true);
        FileStream fs = new FileStream(filePath, FileMode.Create);

        StreamWriter sw = new StreamWriter(fs);

        sw.Write(dataInJSON);

        sw.Close();
        fs.Close();
    }

    public List<T> LoadListJson<T>(string packName) where T : PacksData
    {
        string filePath = Application.persistentDataPath + "/" + saveFolderName + "/" + packName + ".Json";

        string dataToLoad = "";

        if (File.Exists(filePath))
        {

            FileStream fs = new FileStream(filePath, FileMode.Open);

            StreamReader sr = new StreamReader(fs);

            dataToLoad = sr.ReadToEnd();

            if (dataToLoad != null)
            {
                List<T> loadedData = JsonHelper.FromJson<T>(dataToLoad);

                sr.Close();
                fs.Close();

                return loadedData;
            }
            else
                return null;
        }
        return null;
    }
    #endregion

    #region Pack Data Manager
    private List<PackData> CreateJsonPack(TextAsset textFile, string textfileName)
    {
        List<PackData> pack = new List<PackData>();
        string[] names = textFile.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        foreach (string name in names)
        {
            pack.Add(new PackData(name, true));
        }
        SaveListJson(pack, textfileName);

        if(firstTime)//first time playing
        {
            savedData.packName = textfileName;
            savedData.pack = pack;
            SaveData(savedData);
            onFirstTimePack?.Invoke(savedData);
            firstTime = false;
        }

        return pack;
    }
    
    #endregion

    #region Custom Pack Data Manager
    private void CreateSavedCustomPacks()
    {
        foreach(CustomPacksData customPackData in customPacks)
        {
            onCreateCustomPacks?.Invoke(customPackData.name);
        }
    }
    private void RemoveSavedCustomPack(string packName)
    {
        for (int i=0;i<customPacks.Count;i++)
        {
            if(packName==customPacks[i].name)
            {
                customPacks.RemoveAt(i);
                SaveListJson(customPacks, CustomPackDataName);
            }
        }
    }
    private void AddCustomPack(string customPackName)
    {
        customPacks.Add(new CustomPacksData(customPackName));
        SaveListJson(customPacks,CustomPackDataName);
    }
    #endregion
}
