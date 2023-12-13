using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    public delegate int OnGetNumberOfPlayers();
    public static event OnGetNumberOfPlayers onGetNumberOfPlayers;
    public delegate int OnGetNumberOfSpyes();
    public static event OnGetNumberOfSpyes onGetNumberOfSpyes;
    public delegate int OnGetTimer();
    public static event OnGetTimer onGetTimer;
    public delegate string OnGetWord();
    public static event OnGetWord onGetWord;

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject card;
    [SerializeField] private int numOfCards=0;
    [SerializeField] private int numOfSpyes=0;
    [SerializeField] private int timer = 0;
    [SerializeField] private string word;
    [SerializeField] private int distance;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float speedDeck;
    [SerializeField] private List<GameObject> deckOfCard = new List<GameObject>();

    private GameObject currentCard;
    private CardController cardController;
    private Vector3 currentCardPos=Vector3.zero;
    private List<int> listNumbers = new List<int>();

    private void Awake()
    {
        numOfCards = onGetNumberOfPlayers.Invoke();
        numOfSpyes = onGetNumberOfSpyes.Invoke();
        timer = onGetTimer.Invoke();
        word = onGetWord.Invoke();
        CreateListNumbers();
        CreateCards();
        CardController.onMovingDeck += StartMoveDeckOfCards;
    }
    private void OnDestroy()
    {
        CardController.onMovingDeck -= StartMoveDeckOfCards;
    }
    private void CreateListNumbers()
    {
        for(int i=0;i<numOfCards;i++)
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
        for(int i=0;i< numOfSpyes;i++)
        {
            int rand = Random.Range(0, listNumbers.Count);

            deckOfCard[listNumbers[rand]].GetComponent<CardController>().SetSpyCard();
            listNumbers.RemoveAt(rand);
        }
    }
    private void StartMoveDeckOfCards()
    {
        deckOfCard.RemoveAt(deckOfCard.Count - 1);

        if(deckOfCard.Count!=0)
            StartCoroutine(MoveDeckOfCards());
    }
    IEnumerator MoveDeckOfCards()
    {
        yield return new WaitForSeconds(0.1f);

        int step = deckOfCard.Count-1;

        deckOfCard[deckOfCard.Count - 1].GetComponent<CardController>().ActivateCardBackText();

        while (step >= 0)
        {
            deckOfCard[step].transform.localPosition = Vector3.MoveTowards(deckOfCard[step].transform.localPosition,
                new Vector3(deckOfCard[step].transform.localPosition.x +distance, deckOfCard[step].transform.localPosition .y+distance, 0), movementSpeed);
            step--;
            yield return new WaitForSeconds(speedDeck);
        }
    }
}
