using UnityEngine;
using UnityEngine.UI;

public class SpyesSnapToSlot : MonoBehaviour
{
    public delegate SavedData OnNumberSnapedOnStartGame();
    public static event OnNumberSnapedOnStartGame onNumberSnapedOnStartGame;

    public delegate void OnSnapNumberOfSpyes(int numberSnaped);
    public static event OnSnapNumberOfSpyes onSnapNumberOfSpyes;


    [SerializeField] private ScroolView scroolView;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private RectTransform sampleListSlot;
    [SerializeField] private HorizontalOrVerticalLayoutGroup HLG;
    [SerializeField] private float snapForce;
    [SerializeField] private MenuManager menuManager;

    [Space]
    [Header("Spy Slots")]
    [SerializeField] private GameObject slot1;
    [SerializeField] private GameObject slot2;
    [SerializeField] private GameObject slot3;
    [SerializeField] private GameObject slot4;

    private SavedData savedData;
    private int currentSlot = 0;
    public bool isSnapped = false;
    private float snapSpeed;

    private void Awake()
    {
        savedData = onNumberSnapedOnStartGame?.Invoke();
        SetMaxNumbersOfSpyes(savedData.players);
        NumberSnapedOnStartGame();
    }

    private void Update()
    {
        currentSlot = Mathf.RoundToInt(0 - (contentPanel.localPosition.x / (sampleListSlot.rect.width + HLG.spacing)));

        if (scrollRect.velocity.magnitude < 200 && !isSnapped)
        {
            scrollRect.velocity = Vector2.zero;
            snapSpeed += snapForce * Time.deltaTime;

            contentPanel.localPosition = new Vector3(
                Mathf.MoveTowards(contentPanel.localPosition.x, 0 - (currentSlot * (sampleListSlot.rect.width + HLG.spacing)), snapSpeed),
                contentPanel.localPosition.y,
                contentPanel.localPosition.z);

            if (contentPanel.localPosition.x == 0 - (currentSlot * (sampleListSlot.rect.width + HLG.spacing)))
            {
                
                NumberSnaped();
            }
        }
        if (scrollRect.velocity.magnitude > 200)
        {
            isSnapped = false;
            menuManager.isSnaped = false;
            snapSpeed = 0;
        }
    }
    private void NumberSnapedOnStartGame()
    {
        currentSlot = savedData.spyes - 1;
      
        contentPanel.localPosition = new Vector3(
                0 - (currentSlot * (sampleListSlot.rect.width + HLG.spacing)), 0, 0);
    }
    private void NumberSnaped()
    {
        currentSlot += 1;
        onSnapNumberOfSpyes?.Invoke(currentSlot);
        isSnapped = true;
        menuManager.isSnaped = true;
    }

    public void SetMaxNumbersOfSpyes(int numberPlayers)
    {
        if (numberPlayers < 5)
        {
            slot1.SetActive(true);
            slot2.SetActive(false);
            slot3.SetActive(false);
            slot4.SetActive(false);
        }
        else if (numberPlayers < 10)
        {
            slot1.SetActive(true);
            slot2.SetActive(true);
            slot3.SetActive(false);
            slot4.SetActive(false);
        }
        else if (numberPlayers < 15)
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
    public void SetCurrentSlot(int slot)
    {
        currentSlot = slot-1;

        int pos;
        pos = Mathf.RoundToInt(0 - currentSlot * (sampleListSlot.rect.width + HLG.spacing));
        contentPanel.localPosition = new Vector3(pos, contentPanel.localPosition.y,
            contentPanel.localPosition.z);
    }
}
