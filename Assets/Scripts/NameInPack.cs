using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameInPack : MonoBehaviour
{
    public delegate void OnToggleChange(int place,bool toggleStatus);
    public static event OnToggleChange onToggleChange;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Toggle toggle;
    private int place;
    public void SetGameObject(int _place,string str,bool status)
    {
        place = _place;
        nameText.text = str;
        toggle.isOn = status;
    }
    public void SetGameObject(string str, bool status)
    {
        nameText.text = str;
        toggle.isOn = status;
    }

    public void ToggleChange()
    {
        onToggleChange.Invoke(place, toggle.isOn);
    }
}
