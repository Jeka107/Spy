using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject secondScreen;
    [SerializeField] private GameObject thirdScreen;
    [SerializeField] private GameObject howToPlayScreen;
    [SerializeField] private GameObject blur;
    [SerializeField] private List<Vector2> wayPoints;
    [SerializeField] private float speed;
    [SerializeField] private List<Color> colors;
    [SerializeField] private float blurSpeed;

    private Image blurImage;
    private bool move = false;
    private GameObject currentScreen;
    private Vector3 currentWayPoint;

    private void Awake()
    {
        blurImage = blur.GetComponent<Image>();
        MenuManager.onSecondScreen += SecondScreenAction;
        MenuManager.onThirdScreen += ThirdScreenAction;
        MenuManager.onHowToPlayScreen += howToPlayScreenAction;

        MenuManager.onBlurOn += BlurEffectOn;
        MenuManager.onBlurOff += BlurEffectOff;
    }
    private void OnDestroy()
    {
        MenuManager.onSecondScreen -= SecondScreenAction;
        MenuManager.onThirdScreen -= ThirdScreenAction;
        MenuManager.onHowToPlayScreen -= howToPlayScreenAction;

        MenuManager.onBlurOn -= BlurEffectOn;
        MenuManager.onBlurOff -= BlurEffectOff;
    }
    private void SecondScreenAction(bool status)
    { 
        if(status)
        {
            currentScreen = secondScreen;
            currentWayPoint = wayPoints[1];

            StartCoroutine(BlurEffectOn(blurImage));
        }
        else
        {
            currentWayPoint = wayPoints[0];

            StartCoroutine(BlurEffectOff(blurImage));
        }
        move = true;
        StartCoroutine(MoveScreens());
    }
    private void ThirdScreenAction(bool status)
    {
        if (status)
        {
            currentScreen = thirdScreen;
            currentWayPoint = wayPoints[1];

            StartCoroutine(BlurEffectOn(blurImage));
        }
        else
        {
            currentWayPoint = wayPoints[0];

            StartCoroutine(BlurEffectOff(blurImage));
        }
        move = true;
        StartCoroutine(MoveScreens());
    }
    private void howToPlayScreenAction(bool status)
    {
        if (status)
        {
            currentScreen = howToPlayScreen;
            currentWayPoint = wayPoints[1];

            StartCoroutine(BlurEffectOn(blurImage));
        }
        else
        {
            currentWayPoint = wayPoints[0];

            StartCoroutine(BlurEffectOff(blurImage));
        }
        move = true;
        StartCoroutine(MoveScreens());
    }
    IEnumerator MoveScreens()
    {
        while (move)
        {
            currentScreen.transform.localPosition = Vector2.MoveTowards(currentScreen.transform.localPosition,
                currentWayPoint, speed * Time.deltaTime);
            if (currentScreen.transform.localPosition == currentWayPoint)
                move = false;
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator BlurEffectOn(Image blurImage)
    {
        Color color = new Color(0, 0, 0, 0);
        blurImage.raycastTarget = true;

        while (color.a<= colors[1].a)
        {
            color.a += 0.05f;
            blurImage.color = color;

            yield return new WaitForSeconds(blurSpeed);
        }
    }
    IEnumerator BlurEffectOff(Image blurImage)
    {
        Color color = new Color(0, 0, 0, 0.8f);
        blurImage.raycastTarget = false;

        while (color.a >= colors[0].a)
        {
            color.a -= 0.05f;
            blurImage.color = color;

            yield return new WaitForSeconds(blurSpeed);
        }
    }
}
