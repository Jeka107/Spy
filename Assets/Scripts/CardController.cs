using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardController : MonoBehaviour
{
    public delegate void OnMovingDeck();
    public static event OnMovingDeck onMovingDeck;

    public delegate void OnFlipCard(string explanation);
    public static event OnFlipCard onFlipCard;

    public delegate void OnCheckLastCard();
    public static event OnCheckLastCard onCheckLastCard;

    [SerializeField] private GameObject cardSides;

    [Space]
    [Header("Card Back")]
    [SerializeField] private GameObject cardBackTextGO;
    [SerializeField] private TextMeshProUGUI cardNumText;

    [Space]
    [Header("Card Face")]
    [SerializeField] private Image imageCardFace;
    [SerializeField] private Sprite cardSpySprite;
    [SerializeField] private TextMeshProUGUI cardFaceText;

    private bool facedDown=true;
    private Animator animatorCard,animatorCardSides;
    private bool spy = false;

    private void Awake()
    {
        animatorCard = GetComponent<Animator>();
        animatorCardSides = cardSides.GetComponent<Animator>();
        cardBackTextGO.SetActive(false);
    }

    public void StartFlipCard()
    {
        if (facedDown)
        {
            if(spy)
                onFlipCard?.Invoke(EnumExplanation.Spy);
            else
                onFlipCard?.Invoke(EnumExplanation.TapCardAgain);

            animatorCardSides.enabled = true;
            animatorCardSides.Play("CardFlip");
            facedDown = false;
        }
        else
        {
            cardBackTextGO.SetActive(false);
            onFlipCard?.Invoke(EnumExplanation.TapCard);

            animatorCard.enabled = true;
            animatorCard.Play("CardDisappear");

            StartCoroutine(MoveDeck());
            onCheckLastCard.Invoke();

            Destroy(gameObject, 0.5f);
        }
    }
    IEnumerator MoveDeck()
    {
        yield return new WaitForSeconds(0.1f);
        onMovingDeck?.Invoke();
    }
    public void SetCardBackText(int _numOfPlayer)
    {
        cardNumText.text = _numOfPlayer.ToString();
    }
    public void ActivateCardBackText()
    {
        cardBackTextGO.SetActive(true);
    }
    public void SetCardFace(string word)
    {
        cardFaceText.text = word;
    }
    public void SetSpyCard()
    {
        spy = true;
        imageCardFace.sprite = cardSpySprite;
        cardFaceText.text = "מרגל";
    }
}
