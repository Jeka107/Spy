using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class AddPackManager : MonoBehaviour
{
    [SerializeField] private GameObject newPack;
    [SerializeField] private GameObject packsContent;

    private void Awake()
    {
        SaveDataManager.onCreateSavedCustomPacks += AddPack;
        NewPack.onDeleteThisPack += DeletePack;
    }
    private void OnDestroy()
    {
        SaveDataManager.onCreateSavedCustomPacks -= AddPack;
        NewPack.onDeleteThisPack -= DeletePack;
    }

    private GameObject currentNewPack;
    public void AddPack(TMP_InputField inputField)
    {
        if (inputField.text != "")
        {
            currentNewPack = Instantiate(newPack, transform.position, Quaternion.identity, packsContent.transform);
            currentNewPack.GetComponent<NewPack>().SetPackName(inputField.text);
            inputField.text = "";
        }
    }
    private void AddPack(string packName)
    {
        currentNewPack = Instantiate(newPack, transform.position, Quaternion.identity, packsContent.transform);
        currentNewPack.GetComponent<NewPack>().SetPackName(packName);
    }
    private void DeletePack(string packName)
    {
        System.IO.File.Delete(Application.persistentDataPath + "/" + "Saved_Data_JSON" + "/" + packName + ".Json");
    }
}
