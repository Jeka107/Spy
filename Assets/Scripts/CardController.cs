using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] private Vector3 cardRotate;
    [SerializeField] private GameObject cardLayout;
    [SerializeField] private GameObject cardBack;
    [SerializeField] private float smooth;

    private bool cardBackIsActive=false;

    public void StartFlipCard()
    {
        if (!cardBackIsActive)
            StartCoroutine(FlipCard());
        else
        {
            //Activate animation.Card dispears from screen.
            Debug.Log("Card dispears from screen");
            Destroy(gameObject);
        }
    }
    IEnumerator FlipCard()
    {
        int i = 0;

        while(i!=180)
        {
            i++;

            //rotate cards for smooth flip.
            cardLayout.transform.Rotate(cardRotate);
            cardBack.transform.Rotate(-cardRotate);

            if (i == 90)
                FlipStatus();

            yield return new WaitForSeconds(smooth);
        }
    }
    private void FlipStatus()
    {
        cardBack.SetActive(true);
        cardBackIsActive = true;
    }

}
