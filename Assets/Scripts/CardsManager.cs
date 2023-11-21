using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject card;
    [SerializeField] private int numOfCards;
    [SerializeField] private int distance;

    private GameObject currentCard;
    private Vector3 currentCardPos=Vector3.zero;
    private void Awake()
    {
        //currentCardPos = transform.position;
        CreateCards();
    }
    private void CreateCards()
    {
        for (int i = numOfCards; i > 0; i--)
        {
            currentCardPos = new Vector3(-i * distance, -i* distance, 0);
            currentCard = Instantiate(card, currentCardPos, Quaternion.identity, transform);
        }
        transform.SetParent(canvas.transform, false);
    }
    private void MoveDeckOfCards()
    {

    }
}
