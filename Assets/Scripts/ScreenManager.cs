using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject secondScreen;
    [SerializeField] private GameObject thirdScreen;
    [SerializeField] private GameObject howToPlayScreen;
    [SerializeField] private GameObject blur;

    private Animator secondScreenAnimator;
    private Animator thirdScreenAnimator;
    private Animator howToPlayScreenAnimator;
    private Animator blurAnimator;

    private void Awake()
    {
        secondScreenAnimator = secondScreen.GetComponent<Animator>();
        thirdScreenAnimator = thirdScreen.GetComponent<Animator>();
        howToPlayScreenAnimator = howToPlayScreen.GetComponent<Animator>();
        blurAnimator = blur.GetComponent<Animator>();

        MenuManager.onSecondScreen += SecondScreenAction;
        MenuManager.onThirdScreen += ThirdScreenAction;
        MenuManager.onHowToPlayScreen += howToPlayScreenAction;
    }
    private void OnDestroy()
    {
        MenuManager.onSecondScreen -= SecondScreenAction;
        MenuManager.onThirdScreen -= ThirdScreenAction;
        MenuManager.onHowToPlayScreen -= howToPlayScreenAction;
    }
    private void SecondScreenAction(bool status)
    {
        secondScreenAnimator.SetBool("Screen", status);
        blurAnimator.SetBool("Screen", status);
    }

    private void ThirdScreenAction(bool status)
    {
        thirdScreenAnimator.SetBool("Screen", status);
        blurAnimator.SetBool("Screen", status);
    }

    private void howToPlayScreenAction(bool status)
    {
        howToPlayScreenAnimator.SetBool("Screen", status);
        blurAnimator.SetBool("Screen", status);
    }
}
