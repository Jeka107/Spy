using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject secondScreen;
    [SerializeField] private GameObject thirdScreen;
    [SerializeField] private GameObject blur;

    private Animator secondScreenAnimator;
    private Animator thirdScreenAnimator;
    private Animator blurAnimator;

    private void Awake()
    {
        secondScreenAnimator = secondScreen.GetComponent<Animator>();
        thirdScreenAnimator=thirdScreen.GetComponent<Animator>();
        blurAnimator = blur.GetComponent<Animator>();

        MenuManager.onSecondScreenOn += SecondScreenOn;
        MenuManager.onSecondScreenOff += SecondScreenOff;
        MenuManager.onThirdScreenOn += ThirdScreenOn;
        MenuManager.onThirdScreenOff += ThirdScreenOff;
    }
    private void OnDestroy()
    {
        MenuManager.onSecondScreenOn -= SecondScreenOn;
        MenuManager.onSecondScreenOff -= SecondScreenOff;
        MenuManager.onThirdScreenOn -= ThirdScreenOn;
        MenuManager.onThirdScreenOff -= ThirdScreenOff;
    }
    private void SecondScreenOn()
    {
        secondScreenAnimator.SetBool("Screen", true);
        blurAnimator.SetBool("Screen", true);
    }
    private void SecondScreenOff()
    {
        secondScreenAnimator.SetBool("Screen", false);
        blurAnimator.SetBool("Screen", false);
    }
    private void ThirdScreenOn()
    {
        thirdScreenAnimator.SetBool("Screen", true);
        blurAnimator.SetBool("Screen", true);
    }
    private void ThirdScreenOff()
    {
        thirdScreenAnimator.SetBool("Screen", false);
        blurAnimator.SetBool("Screen", false);
    }
}
