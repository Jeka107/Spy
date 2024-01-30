using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // Events Managment
    public delegate void OnSecondScreen(bool status);
    public static event OnSecondScreen onSecondScreen;
    public delegate void OnThirdScreen(bool status);
    public static event OnThirdScreen onThirdScreen;
    public delegate void OnHowToPlayScreen(bool status);
    public static event OnHowToPlayScreen onHowToPlayScreen;

    public delegate SavedData OnLoadSavedDataToUI();
    public static event OnLoadSavedDataToUI onLoadSavedDataToUI;
    public delegate void OnSetGame(SavedData savedData);
    public static event OnSetGame onSetGame;
    public delegate void OnPackChange(List<PackData> pack, string packName);
    public static event OnPackChange onPackChange;

    public delegate IEnumerator OnBlurOn(Image image);
    public static event OnBlurOn onBlurOn;
    public delegate IEnumerator OnBlurOff(Image image);
    public static event OnBlurOff onBlurOff;
    //
    [Header("Scene Management")]
    [SerializeField] private float waitBeforeLoadScene;
   
    [Space]
    [Header("UI Managment(Menu Screen)")]
    [SerializeField] private GameObject subjScroolView;
    [SerializeField] private GameObject subjScroolViewContent;
    [SerializeField] private GameObject namesScroolView;
    
    [SerializeField] private TextMeshProUGUI numberOfPlayersText;
    [SerializeField] private TextMeshProUGUI numberOfSpyesText;
    [SerializeField] private TextMeshProUGUI timeSelected;
    [SerializeField] private TextMeshProUGUI subjectText;
    [SerializeField] private GameObject nameOfPrefab;
    [SerializeField] private GameObject packEmptyError;
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

    [Space]
    [Header("SnapTools")]
    [SerializeField] private GameObject snapToSlotPlayerGO;
    [SerializeField] private GameObject snapToSlotTimerGo;
    [SerializeField] private SpyesSnapToSlot spyesSnapToSlot;
    private SnapToSlot snapToSlotPlayer;
    private SnapToSlot snapToSlotTimer;

    private GameObject currentGameObject;
    private GameObject currentNameInPack;
    private GameObject namesScroolViewContent;
    private List<PackData> currentPackDatas = new List<PackData>();
    private string currentPackName;
    private ScrollRect scrollRectNamesScroolView;
    private int currentNameInPackPos;
    
    private SavedData currentSavedData=new SavedData();
    private int players;
    private int spyes;
    private int time;

    private string numberbelongs;
    private Image blurImage;
    public bool isSnaped = false;

    private void Awake()
    {
        namesScroolView.SetActive(false);
        scrollRectNamesScroolView = namesScroolView.GetComponent<ScrollRect>();
        blurImage = blur.GetComponent<Image>();
        snapToSlotPlayer = snapToSlotPlayerGO.GetComponent<SnapToSlot>();
        snapToSlotTimer = snapToSlotTimerGo.GetComponent<SnapToSlot>();

        //first time playing
        SaveDataManager.onFirstTimePack += SetFirstTimeGame;

        DefaultPack.onSelected += SubjectSelected;
        NewPack.onSelected += SubjectSelected;
        NewPack.onSelectedCustomPack += CustomSubjectSelected;
        NewPack.onDefaultPack += StartPack;
        SnapToSlot.onSnapNumberOfPlayers += SetNumberOfPlayers;
        SpyesSnapToSlot.onSnapNumberOfSpyes += SetNumberOfSpyes;
        SnapToSlot.onSnapTime += SetTimeSelected;
        NameInPack.onToggleChange += ToggleChange;
        NameInPack.onDelete += ConfirmDeleteNameLabelOn;
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

        DefaultPack.onSelected -= SubjectSelected;
        NewPack.onSelected -= SubjectSelected;
        NewPack.onSelectedCustomPack -= CustomSubjectSelected;
        NewPack.onDefaultPack -= StartPack;
        SnapToSlot.onSnapNumberOfPlayers -= SetNumberOfPlayers;
        SpyesSnapToSlot.onSnapNumberOfSpyes -= SetNumberOfSpyes;
        SnapToSlot.onSnapTime -= SetTimeSelected;
        NameInPack.onToggleChange -= ToggleChange;
        NameInPack.onDelete -= ConfirmDeleteNameLabelOn;
    }

    private void SetFirstTimeGame(SavedData savedData)
    {
        currentSavedData = savedData;
    }
    #region SceneManagement
    public void LoadNextScene()
    {
        StartCoroutine(Load());
    }
    IEnumerator Load()
    {
        yield return new WaitForSeconds(waitBeforeLoadScene);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion
    #region UI Managment
    private void StartPack()
    {
        DefaultPack currentContentChild = subjScroolViewContent.transform.GetChild(0).gameObject.GetComponent<DefaultPack>();

        currentSavedData.pack = currentContentChild.GetPack();
        currentSavedData.packName = currentContentChild.GetPackName();

        SetSavedDataUI(currentSavedData);
        StartCoroutine(ClearThirdScreen(0));
    }
    private void SetSavedDataUI(SavedData _savedData)
    {
        numberOfPlayersText.text = _savedData.players.ToString();
        _savedData.spyes = CheckSpyDependsPlayer(_savedData.players,_savedData.spyes);
        numberOfSpyesText.text = _savedData.spyes.ToString();
        timeSelected.text = _savedData.time.ToString();
        subjectText.text = _savedData.packName;

        onSetGame?.Invoke(_savedData);
    }
    private int CheckSpyDependsPlayer(int players,int spyes)
    {
        if (players < 5)
        {
            spyes = 1;
        }
        else if (players < 10)
        {
            if (spyes > 2)
                spyes = 2;
        }
        else if (players < 15)
        {
            if (spyes > 3)
                spyes = 3;
        }
        return spyes;
    }
    #region Button Manage
    public void NumberOfPlayersOn()
    {
        secondScreenTitle.text = EnumTitle.Players.ToString();
        numberbelongs= EnumTitle.Players.ToString();
        playersScroolView.SetActive(true);
        onSecondScreen?.Invoke(true);
    }
    public void NumberOfSpyesOn()
    {
        secondScreenTitle.text = EnumTitle.Spyes.ToString();
        numberbelongs = EnumTitle.Spyes.ToString();
        spyesScroolView.SetActive(true);
        onSecondScreen?.Invoke(true);
    }
    public void TimeSelectOn()
    {
        secondScreenTitle.text = EnumTitle.Time.ToString();
        numberbelongs = EnumTitle.Time.ToString();
        timeScroolView.SetActive(true);
        onSecondScreen?.Invoke(true);
    }
    public void SubjectSelectionOn()
    {
        deletePackIcon.SetActive(false);
        StartCoroutine(ClearThirdScreen(0f));  
    }
    public void HowToPlayScreenOn()
    {
        onHowToPlayScreen?.Invoke(true);
    }
    public void SubjectsOn()
    {
        secondScreenTitle.text = EnumTitle.Subjects.ToString();
        onThirdScreen?.Invoke(true);
    }
    public void SubjectSelected(List<PackData> packDatas, string packName, GameObject content)
    {
        currentPackDatas = packDatas;
        thirdScreenTitle.text=currentPackName = packName;
        namesScroolViewContent = content;
        scrollRectNamesScroolView.content = content.GetComponent<RectTransform>();//atach content.

        namesScroolView.SetActive(true);
        subjScroolView.SetActive(false);
    }
    public void ConfirmedNumberSelection()
    {
        if (snapToSlotPlayer.isSnapped|| snapToSlotTimer.isSnapped || spyesSnapToSlot.isSnapped)
        {
            switch(numberbelongs)
            {
                case EnumTitle.Players:
                    currentSavedData.players = players;
                    break;
                case EnumTitle.Spyes:
                    currentSavedData.spyes = spyes;
                    break;
                case EnumTitle.Time:
                    currentSavedData.time = time;
                    break;
            }

            SetSavedDataUI(currentSavedData);
            onSecondScreen?.Invoke(false);
            StartCoroutine(ClearSecondScreen());
        }
    }
    IEnumerator ClearSecondScreen()
    {
        yield return new WaitForSeconds(0.2f);

        playersScroolView.SetActive(false);
        spyesScroolView.SetActive(false);
        timeScroolView.SetActive(false);
    }
    public void ConfirmedSubjectSelection()
    {
        bool randCanHappen = CheckIfRandomCanHappen();

        if (randCanHappen)
        {
            currentSavedData.pack = currentPackDatas;
            currentSavedData.packName = currentPackName;
            onPackChange?.Invoke(currentSavedData.pack, currentSavedData.packName);
            SetSavedDataUI(currentSavedData);

            ThirdsScreenOff();
        }
        else
        {
            packEmptyError.SetActive(true);
            StartCoroutine(ErrorLabelOff());
        }
    }
    private bool CheckIfRandomCanHappen()
    {
        foreach(PackData pack in currentPackDatas)
        {
            if (pack.status == true)
                return true;
        }
        return false;
    }
    IEnumerator ErrorLabelOff()
    {
        yield return new WaitForSeconds(2f);
        packEmptyError.SetActive(false);
        
    }
    public void ThirdsScreenOff()
    {
        onThirdScreen?.Invoke(false);
        StartCoroutine(ClearThirdScreen(0.2f));
    }
    public void ScreensOff()
    {
        snapToSlotPlayer.SetCurrentSlot(currentSavedData.players);
        snapToSlotTimer.SetCurrentSlot(currentSavedData.time);
        spyesSnapToSlot.SetCurrentSlot(currentSavedData.spyes);


        onSecondScreen?.Invoke(false);
        StartCoroutine(ClearSecondScreen());
    }
    #endregion

    #region Custom Subject
    private void CustomSubjectSelected(GameObject gameObjectPack)
    {
        currentGameObject = gameObjectPack;
        deletePackIcon.SetActive(true);
    }
    public void CustomSubjectConfirmDelete()
    {
        confirmDeletePackLabel.SetActive(true);
        StartCoroutine(onBlurOn?.Invoke(blurImage));
    }
    public void CustomSubjectCancelDelete()
    {
        confirmDeletePackLabel.SetActive(false);
        StartCoroutine(onBlurOff?.Invoke(blurImage));
    }
    public void CustomSubjectDelete()
    {
        currentGameObject.GetComponent<NewPack>().DeleteThisPack();
        StartCoroutine(onBlurOff?.Invoke(blurImage));

        namesScroolView.SetActive(false);
        subjScroolView.SetActive(true);
        deletePackIcon.SetActive(false);
        confirmDeletePackLabel.SetActive(false);
    }
    #endregion

    #region Custom Name
    private void ToggleChange(int namePlace, bool status)
    {
        currentPackDatas[namePlace].status = status;
        onPackChange?.Invoke(currentPackDatas, currentPackName);
    }
    public void AddCustomWord(TMP_InputField inputField)
    {
        NameInPack nameInPack;

        if (inputField.text != "")
        {
            currentPackDatas.Add(new PackData(inputField.text, true));
            onPackChange?.Invoke(currentPackDatas, currentPackName);

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
        StartCoroutine(onBlurOn?.Invoke(blurImage));
    }
    public void ConfirmDeleteNameLabelOff()
    {
        confirmDeleteNameLabel.SetActive(false);
        StartCoroutine(onBlurOff?.Invoke(blurImage));
    }
    public void ConfirmDeleteName()
    {
        currentPackDatas.RemoveAt(currentNameInPackPos);
        Destroy(currentNameInPack);
        ConfirmDeleteNameLabelOff();

        onPackChange?.Invoke(currentPackDatas, currentPackName);
    }
    #endregion

    private void SetNumberOfPlayers(int num)
    {
        players = num;
    }
    private void SetNumberOfSpyes(int num)
    {
        spyes = num;
    }
    private void SetTimeSelected(int num)
    {
        time = num;
    }

    IEnumerator ClearThirdScreen(float waitForSeconds)
    {
        yield return new WaitForSeconds(waitForSeconds);

        namesScroolViewContent?.SetActive(false);
        namesScroolView?.SetActive(false);
        subjScroolView?.SetActive(true);
        deletePackIcon?.SetActive(false);
        thirdScreenTitle.text = "נושאים";
    } 
    

    #endregion
}
