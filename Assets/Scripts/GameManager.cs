using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public delegate int OnGetTimer();
    public static event OnGetTimer onGetTimer;
    public delegate AdmobAdsScript OnGetAdmobAds();
    public static event OnGetAdmobAds onGetAdmobAds;

    [SerializeField] private GameObject titleImage;
    [SerializeField] private GameObject explanation;
    [SerializeField] private float timer = 1;
    [SerializeField] private GameObject timerGameobject;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI playerNum;
    [SerializeField] private CardsManager cardsManager;
    [SerializeField] private GameObject Spyes;
    [SerializeField] private TextMeshProUGUI numsOfSpyes;
    [SerializeField] private AdmobAdsScript admobAds;

    private TextMeshProUGUI explanationText;
    private bool timerStatus = false;
    private List<int> spyesPlaces = new List<int>();
    private void Awake()
    {
        CardController.onFlipCard += SetExplanationText;
        CardsManager.onDeckEmpty += DeckEmpty;
        admobAds = onGetAdmobAds?.Invoke();

        timerGameobject.SetActive(false);
        Spyes.SetActive(false);
        titleImage.SetActive(false);

        if (FindObjectOfType<DataManager>() != null)
        {
            timer = onGetTimer.Invoke();
        }
        timer *= 60f;

        explanationText = explanation.GetComponent<TextMeshProUGUI>();
        SetExplanationText(EnumExplanation.TapCard);
    }
    private void OnDestroy()
    {
        CardController.onFlipCard -= SetExplanationText;
        CardsManager.onDeckEmpty -= DeckEmpty;
    }


    private void Update()
    {
        if(timerStatus)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                DisplayTime(timer);
            }
            else
            {
                timerStatus = false;
                timerGameobject.SetActive(false);
                DisplaySpyes();
            }
        }
    }
    private void DisplaySpyes()
    {
        Spyes.SetActive(true);
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        for (int i = 0; i < spyesPlaces.Count; i++)
        {
            numsOfSpyes.text += spyesPlaces[i].ToString();
            if (i != spyesPlaces.Count - 1)
                numsOfSpyes.text += ",";
        }
    }
    public void BackToHome()
    {
        SceneManager.LoadScene(0);
    }
    private void SetExplanationText(string explanation)
    {
        explanationText.text = explanation;
    }
    
    private void DeckEmpty()
    {
        explanationText.text = EnumExplanation.TimerOn;
        timerGameobject.SetActive(true);
        titleImage.SetActive(true);
        spyesPlaces = cardsManager.spyesPlaces;
        timerStatus = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        StartCoroutine(StartAds());
    }
    IEnumerator StartAds()
    {
        yield return new WaitForSeconds(1f);
        admobAds?.ShowInterstitialAd();
        admobAds?.LoadAd();
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
