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

    private TextMeshProUGUI explanationText;
    private bool timerStatus = false;
    private void Awake()
    {
        CardController.onFlipCard += SetExplanationText;
        CardsManager.onDeckEmpty += DeckEmpty;

        timerGameobject.SetActive(false);

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
                Debug.Log("Time has run out!");
                timerStatus = false;
            }
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
        timerStatus = true;
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
