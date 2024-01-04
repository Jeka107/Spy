using System.Collections.Generic;
using UnityEngine;

public class DefaultPack : MonoBehaviour
{
    public delegate void OnSelected(List<PackData> pack, string textfileName, GameObject content);
    public static event OnSelected onSelected;

    public delegate List<PackData> OnAwake(TextAsset textFile,string textfileName);
    public static event OnAwake onAwake;

    public delegate List<T> OnLoadPack<T>(string textfileName);
    public static event OnLoadPack<PackData> onLoadPack;

    [SerializeField] private string textfileName;
    [SerializeField] private TextAsset textFile;
    [SerializeField] private List<PackData> pack = new List<PackData>();

    [Space]
    [SerializeField] private Transform namesViewPort;
    [SerializeField] private GameObject namesContent;
    [SerializeField] private GameObject nameInPackPrefab;

    private GameObject myContent;
    private void Start()
    {
       pack = onAwake?.Invoke(textFile, textfileName);

       if(pack==null) 
         pack = onLoadPack?.Invoke(textfileName);
        CreateNames();
    }
    private void CreateNames()
    {
        GameObject currentNameInPack;
        NameInPack nameInPack;

        myContent = Instantiate(namesContent,Vector3.zero,
            Quaternion.identity);
        myContent.transform.SetParent(namesViewPort, false);

        for (int i = 0; i < pack.Count; i++)
        {
            currentNameInPack = Instantiate(nameInPackPrefab, Vector3.zero,
                Quaternion.identity, transform);
            currentNameInPack.transform.SetParent(myContent.transform, false);

            nameInPack = currentNameInPack.GetComponent<NameInPack>();
            nameInPack.SetGameObject(i, pack[i].name, pack[i].status);
            nameInPack.ToggleOn();

            nameInPack.DisableDelete();
        }
        myContent.SetActive(false);
    }
    public void Selected()
    {
        myContent.SetActive(true);
        onSelected?.Invoke(pack, textfileName, myContent);
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
