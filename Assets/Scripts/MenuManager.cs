using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class MenuManager : MonoBehaviour
{
    // Events Managment
    public delegate void OnSecondScreenOn();
    public static event OnSecondScreenOn onSecondScreenOn;
    public delegate void OnSecondScreenOff();
    public static event OnSecondScreenOff onSecondScreenOff;
    public delegate void OnThirdScreenOn();
    public static event OnThirdScreenOn onThirdScreenOn;
    public delegate void OnThirdScreenOff();
    public static event OnThirdScreenOff onThirdScreenOff;
    
    public delegate SavedData OnLoadSavedDataToUI();
    public static event OnLoadSavedDataToUI onLoadSavedDataToUI;
    public delegate void OnSetGame(SavedData savedData);
    public static event OnSetGame onSetGame;
    public delegate void OnPackChange(List<PackData> pack, string packName);
    public static event OnPackChange onPackChange;
    //

    [Header("Scene Management")]
    [SerializeField] private float waitBeforeLoadScene;
   
    [Space]
    [Header("UI Managment(Menu Screen)")]
    [SerializeField] private GameObject subjScroolView;
    [SerializeField] private GameObject subjScroolViewContent;
    [SerializeField] private GameObject namesScroolView;
    [SerializeField] private GameObject namesScroolViewContent;
    [SerializeField] private TextMeshProUGUI numberOfPlayersText;
    [SerializeField] private TextMeshProUGUI numberOfSpyesText;
    [SerializeField] private TextMeshProUGUI timeSelected;
    [SerializeField] private TextMeshProUGUI subjectText;
    [SerializeField] private GameObject nameOfPrefab;
    [SerializeField] private string[] names;

    [Space]
    [Header("UI Managment(Second Screen)")]
    [SerializeField] private TextMeshProUGUI secondScreenTitle;
    [SerializeField] private GameObject playersScroolView;
    [SerializeField] private GameObject spyesScroolView;
    [SerializeField] private GameObject timeScroolView;

    private GameObject currentNameInPack;
    private string currentPackName;

    private SavedData currentSavedData=new SavedData();

    private void Awake()
    {
        namesScroolView.SetActive(false);

        //first time playing
        SaveDataManager.onFirstTimePack += SetFirstTimeGame;

        TextFileStorage.onSelected += SubjectSelected;
        NewPack.onSelected += SubjectSelected;
        NewPack.onDefaultPack += DefaultPack;
        SnapToSlot.onSnapNumberOfPlayers += SetNumberOfPlayers;
        SnapToSlot.onSnapNumberOfSpyes += SetNumberOfSpyes;
        SnapToSlot.onSnapTime += SetTimeSelected;
        NameInPack.onToggleChange += ToggleChange;
    }
    private void Start()
    {
        currentSavedData = onLoadSavedDataToUI.Invoke();

        if (currentSavedData != null)
        {
            SetSavedDataUI(currentSavedData);
        }
    }
    private void OnDestroy()
    {
        SaveDataManager.onFirstTimePack -= SetFirstTimeGame;

        TextFileStorage.onSelected -= SubjectSelected;
        NewPack.onSelected -= SubjectSelected;
        NewPack.onDefaultPack -= DefaultPack;
        SnapToSlot.onSnapNumberOfPlayers -= SetNumberOfPlayers;
        SnapToSlot.onSnapNumberOfSpyes -= SetNumberOfSpyes;
        SnapToSlot.onSnapTime -= SetTimeSelected;
        NameInPack.onToggleChange -= ToggleChange;
    }

    private void SetFirstTimeGame(SavedData savedData)
    {
        currentSavedData = savedData;
    }

    #region SceneManagement
    public void LoadNextScene()
    {
        onSetGame?.Invoke(currentSavedData);
        StartCoroutine(Load());
    }
    IEnumerator Load()
    {
        yield return new WaitForSeconds(waitBeforeLoadScene);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion
    #region UI Managment
    private void DefaultPack()
    {
        TextFileStorage currentContentChild = subjScroolViewContent.transform.GetChild(0).gameObject.GetComponent<TextFileStorage>();

        currentSavedData.pack = currentContentChild.GetPack();
        currentSavedData.packName = currentContentChild.GetPackName();

        SetSavedDataUI(currentSavedData);
    }
    private void SetSavedDataUI(SavedData _savedData)
    {
        numberOfPlayersText.text = _savedData.players.ToString();
        numberOfSpyesText.text = _savedData.spyes.ToString();
        timeSelected.text = _savedData.time.ToString();
        subjectText.text = _savedData.packName; 
    }
    public void NumberOfPlayersOn()
    {
        secondScreenTitle.text = EnumTitle.Players.ToString();
        playersScroolView.SetActive(true);
        spyesScroolView.SetActive(false);
        timeScroolView.SetActive(false);
        onSecondScreenOn?.Invoke();
    }
    public void NumberOfSpyesOn()
    {
        secondScreenTitle.text = EnumTitle.Spyes.ToString();
        spyesScroolView.SetActive(true);
        playersScroolView.SetActive(false);
        timeScroolView.SetActive(false);
        onSecondScreenOn?.Invoke();
    }
    public void TimeSelectOn()
    {
        secondScreenTitle.text = EnumTitle.Time.ToString();
        timeScroolView.SetActive(true);
        spyesScroolView.SetActive(false);
        playersScroolView.SetActive(false);
        onSecondScreenOn?.Invoke();
    }
    public void SubjectsOn()
    {
        secondScreenTitle.text = EnumTitle.Subjects.ToString();
        onThirdScreenOn?.Invoke();
    }
    private void SetNumberOfPlayers(int num)
    {
        currentSavedData.players = num;
    }
    private void SetNumberOfSpyes(int num)
    {
        currentSavedData.spyes = num;
    }
    private void SetTimeSelected(int num)
    {
        currentSavedData.time = num;
    }
    public void SubjectSelected(List<PackData> pack,string packName)
    {
        NameInPack nameInPack;
        currentSavedData.pack = pack;
        currentPackName = packName;

        for (int i = 0; i < pack.Count; i++)
        {
            currentNameInPack = Instantiate(nameOfPrefab, transform.position, Quaternion.identity, namesScroolViewContent.transform);
            nameInPack = currentNameInPack.GetComponent<NameInPack>();

            nameInPack.SetGameObject(i,pack[i].name, pack[i].status);
        }

        namesScroolView.SetActive(true);
        subjScroolView.SetActive(false);
    }
    public void SubjectSelectionOn()
    {
        StartCoroutine(ClearThirdScreen(0f));
    }
    public void ConfirmedNumberSelection()
    {
        SetSavedDataUI(currentSavedData);
        playersScroolView.SetActive(false);
        spyesScroolView.SetActive(false);
        timeScroolView.SetActive(false);

        onSecondScreenOff?.Invoke();
    }
    public void ConfirmedSubjectSelection()
    {
        currentSavedData.packName = currentPackName;
        onPackChange?.Invoke(currentSavedData.pack, currentSavedData.packName);
        SetSavedDataUI(currentSavedData);

        ThirdsScreenOff();
    }
    public void ThirdsScreenOff()
    {
        onThirdScreenOff?.Invoke();
        StartCoroutine(ClearThirdScreen(0.5f));
    }
    IEnumerator ClearThirdScreen(float waitForSeconds)
    {
        yield return new WaitForSeconds(waitForSeconds);

        while (namesScroolViewContent.transform.childCount > 0)
        {
            DestroyImmediate(namesScroolViewContent.transform.GetChild(0).gameObject);
        }
        namesScroolView.SetActive(false);
        subjScroolView.SetActive(true);
    } 
    public void ScreensOff()
    {
        onSecondScreenOff?.Invoke();
        ThirdsScreenOff();
    }
    private void ToggleChange(int namePlace,bool status)
    {
        currentSavedData.pack[namePlace].status = status;
    }
    public void AddCustomWord(TMP_InputField inputField)
    {
        NameInPack nameInPack;
        currentSavedData.pack.Add(new PackData(inputField.text, true));
        onPackChange?.Invoke(currentSavedData.pack, currentPackName);

        currentNameInPack = Instantiate(nameOfPrefab, transform.position, Quaternion.identity, namesScroolViewContent.transform);
        nameInPack = currentNameInPack.GetComponent<NameInPack>();

        nameInPack.SetGameObject(inputField.text, true);
    }
    #endregion
}
