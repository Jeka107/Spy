using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SnapToSlot : MonoBehaviour
{
    public delegate SavedData OnNumberSnapedOnStartGame();
    public static event OnNumberSnapedOnStartGame onNumberSnapedOnStartGame;

    public delegate void OnSnapNumberOfPlayers(int numberSnaped);
    public static event OnSnapNumberOfPlayers onSnapNumberOfPlayers;

    public delegate void OnSnapNumberOfSpyes(int numberSnaped);
    public static event OnSnapNumberOfSpyes onSnapNumberOfSpyes;

    public delegate void OnSnapTime(int numberSnaped);
    public static event OnSnapTime onSnapTime;

    [SerializeField] private ScroolView scroolView;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private RectTransform sampleListSlot;
    [SerializeField] private HorizontalOrVerticalLayoutGroup HLG;
    [SerializeField] private float snapForce;

    [Space]
    [Header("Spy Slots")]
    [SerializeField] private GameObject slot1;
    [SerializeField] private GameObject slot2;
    [SerializeField] private GameObject slot3;
    [SerializeField] private GameObject slot4;

    private SavedData savedData;
    private int currentSlot = 0;
    private bool isSnapped=false;
    private float snapSpeed;

    private void Awake()
    {
        savedData = onNumberSnapedOnStartGame?.Invoke();
        NumberSnapedOnStartGame();
    }

    private void Update()
    {
        currentSlot=Mathf.RoundToInt(0-(contentPanel.localPosition.x/(sampleListSlot.rect.width+HLG.spacing)));

        if (scrollRect.velocity.magnitude < 200&&!isSnapped)
        {
            scrollRect.velocity = Vector2.zero;
            snapSpeed += snapForce * Time.deltaTime;

            contentPanel.localPosition = new Vector3(
                Mathf.MoveTowards(contentPanel.localPosition.x, 0 - (currentSlot * (sampleListSlot.rect.width + HLG.spacing)), snapSpeed),
                contentPanel.localPosition.y,
                contentPanel.localPosition.z);

            if (contentPanel.localPosition.x == 0 - (currentSlot * (sampleListSlot.rect.width + HLG.spacing)))
            {
                isSnapped = true;
                NumberSnaped();
            }
        }
        if(scrollRect.velocity.magnitude>200)
        {
            isSnapped = false;
            snapSpeed = 0;
        }
    }
    private void NumberSnapedOnStartGame()
    {
        switch (scroolView)
        {
            case ScroolView.NumberOfPlayers:
                currentSlot = savedData.players-3;
                SetMaxNumbersOfSpyes(currentSlot);
                break;
            case ScroolView.NumberOfSpyes:
                currentSlot = savedData.spyes-1;
                break;
            case ScroolView.Time:
                currentSlot = savedData.time-1;
                break;
        }
        contentPanel.localPosition = new Vector3(
                0 - (currentSlot * (sampleListSlot.rect.width + HLG.spacing)), 0, 0);
    }
    private void NumberSnaped()
    {
        switch(scroolView)
        {
            case ScroolView.NumberOfPlayers:
                currentSlot += 3;
                onSnapNumberOfPlayers?.Invoke(currentSlot);
                SetMaxNumbersOfSpyes(currentSlot);
                break;
            case ScroolView.NumberOfSpyes:
                currentSlot += 1;
                onSnapNumberOfSpyes?.Invoke(currentSlot);
                break;
            case ScroolView.Time:
                currentSlot += 1;
                onSnapTime?.Invoke(currentSlot);
                break;
        }
    }

    private void SetMaxNumbersOfSpyes(int numberPlayers)
    {
        if(numberPlayers<5)
        {
            slot1.SetActive(true);
            slot2.SetActive(false);
            slot3.SetActive(false);
            slot4.SetActive(false);
        }
        else if(numberPlayers<10)
        {
            slot1.SetActive(true);
            slot2.SetActive(true);
            slot3.SetActive(false);
            slot4.SetActive(false);
        }
        else if(numberPlayers<15)
        {
            slot1.SetActive(true);
            slot2.SetActive(true);
            slot3.SetActive(true);
            slot4.SetActive(false);
        }
        else
        {
            slot1.SetActive(true);
            slot2.SetActive(true);
            slot3.SetActive(true);
            slot4.SetActive(true);
        }
    }
}
