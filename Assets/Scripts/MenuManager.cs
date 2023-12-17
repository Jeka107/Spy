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

    [Space]
    [Header("UI Managment(Third Screen)")]
    [SerializeField] private TextMeshProUGUI thirdScreenTitle;
    [SerializeField] private GameObject blur;
    [SerializeField] private GameObject deletePackIcon;
    [SerializeField] private GameObject confirmDeletePackLabel;
    [SerializeField] private GameObject confirmDeleteNameLabel;


    private bool gameStatus;
    private GameObject currentGameObject;
    private GameObject currentNameInPack;
    private List<PackData> currentPack = new List<PackData>();
    private string currentPackName;
    private int currentNameInPackPos;

    private SavedData currentSavedData=new SavedData();

    private Animator blurAnimator;

    private void Awake()
    {
        namesScroolView.SetActive(false);
        blurAnimator = blur.GetComponent<Animator>();

        //first time playing
        SaveDataManager.onFirstTimePack += SetFirstTimeGame;

        TextFileStorage.onSelected += SubjectSelected;
        NewPack.onSelected += SubjectSelected;
        NewPack.onSelectedCustomPack += CustomSubjectSelected;
        NewPack.onDefaultPack += DefaultPack;
        SnapToSlot.onSnapNumberOfPlayers += SetNumberOfPlayers;
        SnapToSlot.onSnapNumberOfSpyes += SetNumberOfSpyes;
        SnapToSlot.onSnapTime += SetTimeSelected;
        NameInPack.onToggleChange += ToggleChange;
        NameInPack.onDelete += ConfirmDeleteNameLabelOn;

        DataManager.onNamesEmpty += SetGameStatus;
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
        NewPack.onSelectedCustomPack -= CustomSubjectSelected;
        NewPack.onDefaultPack -= DefaultPack;
        SnapToSlot.onSnapNumberOfPlayers -= SetNumberOfPlayers;
        SnapToSlot.onSnapNumberOfSpyes -= SetNumberOfSpyes;
        SnapToSlot.onSnapTime -= SetTimeSelected;
        NameInPack.onToggleChange -= ToggleChange;
        NameInPack.onDelete -= ConfirmDeleteNameLabelOn;

        DataManager.onNamesEmpty -= SetGameStatus;
    }

    private void SetFirstTimeGame(SavedData savedData)
    {
        currentSavedData = savedData;
    }
    private void SetGameStatus(bool status)
    {
        gameStatus = status;
    }

    #region SceneManagement
    public void LoadNextScene()
    {
        if(gameStatus)
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
        StartCoroutine(ClearThirdScreen(0));
    }
    private void SetSavedDataUI(SavedData _savedData)
    {
        numberOfPlayersText.text = _savedData.players.ToString();
        numberOfSpyesText.text = _savedData.spyes.ToString();
        timeSelected.text = _savedData.time.ToString();
        subjectText.text = _savedData.packName;

        onSetGame?.Invoke(currentSavedData);
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
    

    #region Custom Subject
    private void CustomSubjectSelected(GameObject pack)
    {
        currentGameObject = pack;
        deletePackIcon.SetActive(true);
    }
    public void CustomSubjectConfirmDelete()
    {
        confirmDeletePackLabel.SetActive(true);
        blurAnimator.SetBool("Screen", true);
    }
    public void CustomSubjectCancelDelete()
    {
        confirmDeletePackLabel.SetActive(false);
        blurAnimator.SetBool("Screen", false);
    }
    public void CustomSubjectDelete()
    {
        currentGameObject.GetComponent<NewPack>().DeleteThisPack();
        blurAnimator.SetBool("Screen", false);

        namesScroolView.SetActive(false);
        subjScroolView.SetActive(true);
        deletePackIcon.SetActive(false);
        confirmDeletePackLabel.SetActive(false);
    }
    #endregion

    #region Custom Name
    private void ToggleChange(int namePlace, bool status)
    {
        currentPack[namePlace].status = status;
        onPackChange?.Invoke(currentPack, currentPackName);
    }
    public void AddCustomWord(TMP_InputField inputField)
    {
        NameInPack nameInPack;

        if (inputField.text != "")
        {
            currentPack.Add(new PackData(inputField.text, true));
            onPackChange?.Invoke(currentPack, currentPackName);

            currentNameInPack = Instantiate(nameOfPrefab, transform.position, Quaternion.identity, namesScroolViewContent.transform);
            nameInPack = currentNameInPack.GetComponent<NameInPack>();

            nameInPack.SetGameObject(inputField.text, true);
            inputField.text = "";
        }
    }
    private void ConfirmDeleteNameLabelOn(int _currentNameInPackPos, GameObject _currentNameInPack)
    {
        currentNameInPack = _currentNameInPack;
        currentNameInPackPos = _currentNameInPackPos;

        confirmDeleteNameLabel.SetActive(true);
        blurAnimator.SetBool("Screen", true);
    }
    public void ConfirmDeleteNameLabelOff()
    {
        confirmDeleteNameLabel.SetActive(false);
        blurAnimator.SetBool("Screen", false);
    }
    public void ConfirmDeleteName()
    {
        currentPack.RemoveAt(currentNameInPackPos);
        Destroy(currentNameInPack);
        ConfirmDeleteNameLabelOff();

        onPackChange?.Invoke(currentPack, currentPackName);
    }
    #endregion

    public void SubjectSelected(List<PackData> pack, string packName,PackType packType)
    {
        NameInPack nameInPack;

        currentPack = pack;
        currentPackName = packName;
        thirdScreenTitle.text = currentPackName;

        for (int i = 0; i < pack.Count; i++)
        {
            currentNameInPack = Instantiate(nameOfPrefab, transform.position, Quaternion.identity, namesScroolViewContent.transform);
            nameInPack = currentNameInPack.GetComponent<NameInPack>();

            nameInPack.SetGameObject(i, pack[i].name, pack[i].status);
            nameInPack.ToggleOn();

            if (packType == PackType.Default)
                nameInPack.DisableDelete();
        }

        namesScroolView.SetActive(true);
        subjScroolView.SetActive(false);
    }
    public void SubjectSelectionOn()
    {
        deletePackIcon.SetActive(false);
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
        currentSavedData.pack = currentPack;
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
        thirdScreenTitle.text = "נושאים";
    } 
    public void ScreensOff()
    {
        onSecondScreenOff?.Invoke();
        ThirdsScreenOff();
    }
    
    #endregion
}
