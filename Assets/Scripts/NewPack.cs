using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewPack : MonoBehaviour
{
    public delegate List<PackData> OnLoadPack(string textfileName);
    public static event OnLoadPack onLoadPack;

    public delegate void OnAddNewPack(string packName);
    public static event OnAddNewPack onAddNewPack;
    public delegate void OnSavePack(List<PackData> pack, string packName);
    public static event OnSavePack onSavePack;
    public delegate void OnDeleteThisPack(string packName);
    public static event OnDeleteThisPack onDeleteThisPack;
    public delegate void OnDefaultPack();
    public static event OnDefaultPack onDefaultPack;
    public delegate void OnSelected(List<PackData> pack, string textfileName,GameObject content);
    public static event OnSelected onSelected;
    public delegate void OnSelectedCustomPack(GameObject gameObject);
    public static event OnSelectedCustomPack onSelectedCustomPack;

    [SerializeField] private GameObject newPackText;
    [SerializeField] private TextMeshProUGUI packName;

    private List<PackData> pack = new List<PackData>();
    private Transform namesViewPort;
    private GameObject namesContent;
    private GameObject nameInPackPrefab;

    private GameObject myContent;
    private void Start()
    {
        pack = onLoadPack?.Invoke(packName.text);
        CreateNames();
    }
    public void SetPack(Transform viewPort, GameObject content, GameObject _nameInPackPrefab)
    {
        namesViewPort = viewPort;
        namesContent = content;
        nameInPackPrefab = _nameInPackPrefab;
    }
    private void CreateNames()
    {
        GameObject currentNameInPack;
        NameInPack nameInPack;


        myContent = Instantiate(namesContent, Vector3.zero,
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
        }
        myContent.SetActive(false);
    }

    public void Selected()
    {
        myContent.SetActive(true);
        onSelected?.Invoke(pack, packName.text, myContent);
        onSelectedCustomPack?.Invoke(this.gameObject);
    }
    public void SetNewPackName(string _packName)
    {
        packName.text = _packName;
        onAddNewPack?.Invoke(_packName);
        onSavePack?.Invoke(pack, _packName);
    }
    public void SetPackName(string _packName)
    {
        packName.text = _packName;
    }
    public void DeleteThisPack()
    {
        onDefaultPack?.Invoke();
        onDeleteThisPack?.Invoke(packName.text);
        Destroy(this.gameObject);
    }
}
