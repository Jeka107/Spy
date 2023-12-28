using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameInPack : MonoBehaviour
{
    public delegate void OnToggleChange(int place,bool toggleStatus);
    public static event OnToggleChange onToggleChange;
    public delegate void OnDelete(int pos,GameObject gameObject);
    public static event OnDelete onDelete;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Toggle toggle;
    [SerializeField] private GameObject delete;

    private bool toggleStatus = false;
    private int pos;
    public void SetGameObject(int _pos,string str,bool status)
    {
        pos = _pos;
        nameText.text = str;
        toggle.isOn = status;
    }
    public void SetGameObject(string str, bool status)
    {
        nameText.text = str;
        toggle.isOn = status;
    }

    public void ToggleOn()
    {
        toggleStatus = true;
    }
    public void ToggleChange()
    {
        if(toggleStatus)
            onToggleChange?.Invoke(pos,toggle.isOn);
    }
    public void DisableDelete()
    {
        delete.SetActive(false);
    }
    public void OnDeleteThis()
    {
        onDelete?.Invoke(pos,this.gameObject);
    }
}
