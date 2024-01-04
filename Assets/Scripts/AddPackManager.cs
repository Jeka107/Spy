using UnityEngine;
using TMPro;

public class AddPackManager : MonoBehaviour
{
    [SerializeField] private GameObject newPack;
    [SerializeField] private GameObject packsContent;

    [Space]
    [SerializeField] private Transform namesViewPort;
    [SerializeField] private GameObject namesContent;
    [SerializeField] private GameObject nameInPackPrefab;

    private void Awake()
    {
        SaveDataManager.onCreateCustomPacks += AddPack;
        NewPack.onDeleteThisPack += DeletePack;
    }

    private void OnDestroy()
    {
        SaveDataManager.onCreateCustomPacks -= AddPack;
        NewPack.onDeleteThisPack -= DeletePack;
    }

    private GameObject currentNewPack;
    public void AddNewPack(TMP_InputField inputField)
    {
        if (inputField.text != "")
        {
            currentNewPack = Instantiate(newPack, transform.position, Quaternion.identity, packsContent.transform);
            currentNewPack.GetComponent<NewPack>().SetNewPackName(inputField.text);
            currentNewPack.GetComponent<NewPack>().SetPack(namesViewPort, namesContent, nameInPackPrefab);
            inputField.text = "";
        }
    }
    private void AddPack(string packName)
    {
        currentNewPack = Instantiate(newPack, transform.position, Quaternion.identity, packsContent.transform);
        currentNewPack.GetComponent<NewPack>().SetPackName(packName);
        currentNewPack.GetComponent<NewPack>().SetPack(namesViewPort, namesContent, nameInPackPrefab);
    }
    private void DeletePack(string packName)
    {
        System.IO.File.Delete(Application.persistentDataPath + "/" + "Saved_Data_JSON" + "/" + packName + ".Json");
    }
}
