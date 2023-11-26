using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    public delegate void OnMovingDeck();
    public static event OnMovingDeck onMovingDeck;

    [SerializeField] private GameObject cardSides,cardBackText;
    [SerializeField] private TextMeshProUGUI cardNumText;
    [SerializeField] private float smooth;

    private bool facedDown=true;
    private Animator animatorCard,animatorCardSides;

    private void Awake()
    {
        animatorCard = GetComponent<Animator>();
        animatorCardSides = cardSides.GetComponent<Animator>();
        cardBackText.SetActive(false);
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
    public void SetCardLayoutText(int numOfCard)
    {
        cardNumText.text = numOfCard.ToString();
    }
    public void ActivateCardBackText()
    {
        cardBackText.SetActive(true);
    }
}
