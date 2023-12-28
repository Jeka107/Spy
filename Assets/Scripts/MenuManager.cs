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
    [SerializeField] private GameObject namesScroolViewContent;
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

    private GameObject currentGameObject;
    private GameObject currentNameInPack;
    private List<PackData> currentPacks = new List<PackData>();
    private string currentPackName;
    private int currentNameInPackPos;

    private SavedData currentSavedData=new SavedData();

    private Animator blurAnimator;
    private Image blurImage;

    private void Awake()
    {
        namesScroolView.SetActive(false);
        blurAnimator = blur.GetComponent<Animator>();
        blurImage = blur.GetComponent<Image>();

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
    #region Button Manage
    public void NumberOfPlayersOn()
    {
        secondScreenTitle.text = EnumTitle.Players.ToString();
        playersScroolView.SetActive(true);
        spyesScroolView.SetActive(false);
        timeScroolView.SetActive(false);
        onSecondScreen?.Invoke(true);
    }
    public void NumberOfSpyesOn()
    {
        secondScreenTitle.text = EnumTitle.Spyes.ToString();
        spyesScroolView.SetActive(true);
        playersScroolView.SetActive(false);
        timeScroolView.SetActive(false);
        onSecondScreen?.Invoke(true);
    }
    public void TimeSelectOn()
    {
        secondScreenTitle.text = EnumTitle.Time.ToString();
        timeScroolView.SetActive(true);
        spyesScroolView.SetActive(false);
        playersScroolView.SetActive(false);
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
    private void HowToPlayScreenOff()
    {
        onHowToPlayScreen?.Invoke(false);
    }
    public void SubjectsOn()
    {
        secondScreenTitle.text = EnumTitle.Subjects.ToString();
        onThirdScreen?.Invoke(true);
    }
    public void SubjectSelected(List<PackData> packs, string packName, PackType packType)
    {
        NameInPack nameInPack;

        currentPacks = packs;
        currentPackName = packName;
        thirdScreenTitle.text = currentPackName;

        for (int i = 0; i < packs.Count; i++)
        {
            currentNameInPack = Instantiate(nameOfPrefab, transform.position, Quaternion.identity, namesScroolViewContent.transform);
            nameInPack = currentNameInPack.GetComponent<NameInPack>();

            nameInPack.SetGameObject(i, packs[i].name, packs[i].status);
            nameInPack.ToggleOn();

            if (packType == PackType.Default)
                nameInPack.DisableDelete();
        }

        namesScroolView.SetActive(true);
        subjScroolView.SetActive(false);
    }

    public void ConfirmedNumberSelection()
    {
        SetSavedDataUI(currentSavedData);
        playersScroolView.SetActive(false);
        spyesScroolView.SetActive(false);
        timeScroolView.SetActive(false);

        onSecondScreen?.Invoke(false);
    }
    public void ConfirmedSubjectSelection()
    {
        bool randCanHappen = CheckIfRandomCanHappen();

        if (randCanHappen)
        {
            currentSavedData.pack = currentPacks;
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
        foreach(PackData pack in currentPacks)
        {
            if (pack.status == true)
                return true;
        }
        return false;
    }
    IEnumerator ErrorLabelOff()
    {
        yield return new WaitForSeconds(1.6f);
        packEmptyError.SetActive(false);
    }
    public void ThirdsScreenOff()
    {
        onThirdScreen?.Invoke(false);
        StartCoroutine(ClearThirdScreen(0.5f));
    }
    public void ScreensOff()
    {
        onSecondScreen?.Invoke(false);
        ThirdsScreenOff();
        HowToPlayScreenOff();
    }
    #endregion

    #region Custom Subject
    private void CustomSubjectSelected(GameObject pack)
    {
        currentGameObject = pack;
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
        currentPacks[namePlace].status = status;
        onPackChange?.Invoke(currentPacks, currentPackName);
    }
    public void AddCustomWord(TMP_InputField inputField)
    {
        NameInPack nameInPack;

        if (inputField.text != "")
        {
            currentPacks.Add(new PackData(inputField.text, true));
            onPackChange?.Invoke(currentPacks, currentPackName);

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
        currentPacks.RemoveAt(currentNameInPackPos);
        Destroy(currentNameInPack);
        ConfirmDeleteNameLabelOff();

        onPackChange?.Invoke(currentPacks, currentPackName);
    }
    #endregion

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

    IEnumerator ClearThirdScreen(float waitForSeconds)
    {
        yield return new WaitForSeconds(waitForSeconds);

        while (namesScroolViewContent.transform.childCount > 0)
        {
            DestroyImmediate(namesScroolViewContent.transform.GetChild(0).gameObject);
        }
        namesScroolView.SetActive(false);
        subjScroolView.SetActive(true);
        deletePackIcon.SetActive(false);
        thirdScreenTitle.text = "נושאים";
    } 
    

    #endregion
}
