using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardController : MonoBehaviour
{
    public delegate void OnMovingDeck();
    public static event OnMovingDeck onMovingDeck;

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
            animatorCardSides.enabled = true;
            animatorCardSides.Play("CardFlip");
            facedDown = false;
        }
        else
        {
            animatorCard.enabled = true;
            animatorCard.Play("CardDisappear");

            StartCoroutine(MoveDeck());
            Destroy(gameObject, 0.5f);
        }
    }
    IEnumerator MoveDeck()
    {
        yield return new WaitForSeconds(0.1f);
        onMovingDeck?.Invoke();
    }
    public void SetCardBackText(int numOfCard)
    {
        cardNumText.text = numOfCard.ToString();
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
        imageCardFace.sprite = cardSpySprite;
        cardFaceText.text = "מרגל";
    }
}
