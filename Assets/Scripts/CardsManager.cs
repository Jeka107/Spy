using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    public delegate int OnGetNumberOfPlayers();
    public static event OnGetNumberOfPlayers onGetNumberOfPlayers;
    public delegate int OnGetNumberOfSpyes();
    public static event OnGetNumberOfSpyes onGetNumberOfSpyes;
    public delegate string OnGetWord();
    public static event OnGetWord onGetWord;
    public delegate void OnDeckEmpty();
    public static event OnDeckEmpty onDeckEmpty;

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject card;
    [SerializeField] private int numOfCards;
    [SerializeField] private int numOfSpyes;
    [SerializeField] private string word;
    [SerializeField] private int distance;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float speedDeck;
    [SerializeField] private List<GameObject> deckOfCard = new List<GameObject>();
    [SerializeField] public List<int> spyesPlaces = new List<int>();

    private GameObject currentCard;
    private CardController cardController;
    private Vector3 currentCardPos=Vector3.zero;
    [SerializeField] private List<int> listNumbers = new List<int>();

    private void Awake()
    {
        if (FindObjectOfType<DataManager>() != null)
        {
            numOfCards = onGetNumberOfPlayers.Invoke();
            numOfSpyes = onGetNumberOfSpyes.Invoke();
            word = onGetWord?.Invoke();
        }
        CreateListNumbers();
        CreateCards();

        CardController.onMovingDeck += StartMoveDeckOfCards;
        CardController.onCheckLastCard += CheckCardsEmpty;
    }
    private void OnDestroy()
    {
        CardController.onMovingDeck -= StartMoveDeckOfCards;
        CardController.onCheckLastCard -= CheckCardsEmpty;
    }
    private void CreateListNumbers()
    {
        for(int i= 0; i< numOfCards; i++)
        {
            listNumbers.Add(i);
        }
    }
    private void CreateCards()
    {
        for (int i = numOfCards-1; i >= 0; i--)
        {
            currentCardPos = new Vector3(-i * distance, -i* distance, 0);
            currentCard = Instantiate(card, currentCardPos, Quaternion.identity, transform);
            cardController = currentCard.GetComponent<CardController>();
            deckOfCard.Add(currentCard);

            if (i==0)
            {
                cardController.ActivateCardBackText();
            }

            cardController.SetCardBackText(i+1);
            cardController.SetCardFace(word);
        }
        transform.SetParent(canvas.transform, false);
        RandmSpyes();
    }
    private void RandmSpyes()
    {
        CardController currentCardController;

        for (int i=0;i< numOfSpyes;i++)
        {
            int rand = Random.Range(0, listNumbers.Count);

            currentCardController = deckOfCard[listNumbers[rand]].GetComponent<CardController>();
            currentCardController.SetSpyCard();
            spyesPlaces.Add(currentCardController.numOfPlayer);//to show who the spyes.
            listNumbers.RemoveAt(rand);
        }
    }
    private void StartMoveDeckOfCards()
    {
        numOfCards--;
        if (numOfCards != 0)
            StartCoroutine(MoveDeckOfCards());
    }
    IEnumerator MoveDeckOfCards()
    {
        yield return new WaitForSeconds(0.1f);

        int step = numOfCards - 1;

        deckOfCard[step].GetComponent<CardController>().ActivateCardBackText();

        while (step >= 0)
        {
            deckOfCard[step].transform.localPosition = Vector3.MoveTowards(deckOfCard[step].transform.localPosition,
                new Vector3(deckOfCard[step].transform.localPosition.x +distance, deckOfCard[step].transform.localPosition .y+distance, 0), movementSpeed);
            step--;
            yield return new WaitForSeconds(speedDeck);
        }
    }

    private void CheckCardsEmpty()
    {
        if (numOfCards == 1)
        {
            onDeckEmpty?.Invoke();
            spyesPlaces.Sort();
        }
    }
}
