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

    [SerializeField] private GameObject newPackText;
    [SerializeField] private GameObject deletePackButton;
    [SerializeField] private TextMeshProUGUI packName;
    [SerializeField] private GameObject confirmDelete;
    [SerializeField] private float holdTime;
    [SerializeField] private Animator animator;

    private List<PackData> pack = new List<PackData>();
    private bool hold = false;
    private void Start()
    {
        deletePackButton.SetActive(false);
        confirmDelete.SetActive(false);
        pack = onLoadPack?.Invoke(packName.text);  
    }
    public void HoldStart()
    {
        StartCoroutine(Hold());
    }
    IEnumerator Hold()
    {
        yield return new WaitForSeconds(holdTime);
        animator.SetBool("Size", true);
        newPackText.SetActive(false);
        deletePackButton.SetActive(true);
        hold = true;
    }
    public void CancelDelete()
    {
        confirmDelete.SetActive(false);
        animator.SetBool("Size", false);
        newPackText.SetActive(true);
        deletePackButton.SetActive(false);
        hold = false;
    }
    public void ConfirmDeleteLabelOn()
    {
        confirmDelete.SetActive(true);
    }
    public void ConfirmDeleteLabelOff()
    {
        CancelDelete();
    }
    public void Selected()
    {
        if(!hold)
            onSelected?.Invoke(pack, packName.text);
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
