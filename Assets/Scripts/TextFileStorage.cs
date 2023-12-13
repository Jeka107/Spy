using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFileStorage : MonoBehaviour
{
    public delegate void OnSelected(List<PackData> pack,string textfileName);
    public static event OnSelected onSelected;

    public delegate List<PackData> OnAwake(TextAsset textFile,string textfileName);
    public static event OnAwake onAwake;

    public delegate List<T> OnLoadPack<T>(string textfileName);
    public static event OnLoadPack<PackData> onLoadPack;

    [SerializeField] private string textfileName;
    [SerializeField] private TextAsset textFile;
    [SerializeField] private List<PackData> pack = new List<PackData>();

    private void Start()
    {
       pack = onAwake?.Invoke(textFile, textfileName);

       if(pack==null) 
         pack = onLoadPack?.Invoke(textfileName);
    }
    public void Selected()
    {
        onSelected?.Invoke(pack, textfileName);
    }
    public List<PackData> GetPack()
    {
        return pack;
    }
    public string GetPackName()
    {
        return textfileName;
    }
}
