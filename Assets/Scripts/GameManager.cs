using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public delegate int OnGetTimer();
    public static event OnGetTimer onGetTimer;

    [SerializeField] private GameObject explanation;
    [SerializeField] private float timer = 1;
    [SerializeField] private GameObject timerGameobject;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI playerNum;
    [SerializeField] private CardsManager cardsManager;
    [SerializeField] private GameObject Spyes;
    [SerializeField] private TextMeshProUGUI numsOfSpyes;

    private TextMeshProUGUI explanationText;
    private bool timerStatus = false;
    private List<int> spyesPlaces = new List<int>();
    private void Awake()
    {
        CardController.onFlipCard += SetExplanationText;
        CardsManager.onDeckEmpty += DeckEmpty;

        timerGameobject.SetActive(false);
        Spyes.SetActive(false);

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
        SceneManager.LoadScene("Menu");
    }
    private void SetExplanationText(string explanation)
    {
        explanationText.text = explanation;
    }
    
    private void DeckEmpty()
    {
        explanationText.text = EnumExplanation.TimerOn;
        timerGameobject.SetActive(true);
        spyesPlaces = cardsManager.spyesPlaces;
        timerStatus = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
