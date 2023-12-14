using System.Collections;
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
    public delegate void OnSelected(List<PackData> pack, string textfileName);
    public static event OnSelected onSelected;
    public delegate void OnSelectedCustomPack(GameObject gameObject);
    public static event OnSelectedCustomPack onSelectedCustomPack;

    [SerializeField] private GameObject newPackText;
    [SerializeField] private TextMeshProUGUI packName;

    private List<PackData> pack = new List<PackData>();
    private void Start()
    {
        pack = onLoadPack?.Invoke(packName.text);  
    }

    public void Selected()
    {
        onSelected?.Invoke(pack, packName.text);
        onSelectedCustomPack?.Invoke(this.gameObject);
    }
    public void SetPackName(string _packName)
    {
        packName.text = _packName;
        onAddNewPack?.Invoke(_packName);
        onSavePack?.Invoke(pack, _packName);
    }
    public void DeleteThisPack()
    {
        onDefaultPack?.Invoke();
        onDeleteThisPack?.Invoke(packName.text);
        Destroy(this.gameObject);
    }
}
