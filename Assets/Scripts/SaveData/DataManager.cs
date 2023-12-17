using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public delegate void OnNamesEmpty(bool status);
    public static event OnNamesEmpty onNamesEmpty;

    [SerializeField] private int numberOfPlayers;
    [SerializeField] private int numberOfSpyes;
    [SerializeField] private int timer;
    [SerializeField] private List<PackData> pack;

    private static bool created = false;
    private List<string> names=new List<string>();
    private string randomWord;
    private void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        MenuManager.onSetGame += SetGame;

        //Get Data from menu. 
        CardsManager.onGetNumberOfPlayers += GetNumberOfPlayers;
        CardsManager.onGetNumberOfSpyes += GetNumberOfSpyes;
        CardsManager.onGetTimer += GetTimer;
        CardsManager.onGetWord += GetRandomWord;
    }
    private void OnDestroy()
    {
        MenuManager.onSetGame -= SetGame;

        CardsManager.onGetNumberOfPlayers -= GetNumberOfPlayers;
        CardsManager.onGetNumberOfSpyes -= GetNumberOfSpyes;
        CardsManager.onGetTimer -= GetTimer;
        CardsManager.onGetWord -= GetRandomWord;
    }
    private void SetGame(SavedData savedData)
    {
        names.Clear();

        numberOfPlayers = savedData.players;
        numberOfSpyes = savedData.spyes;
        timer = savedData.time;
        pack = savedData.pack;

        for (int i = 0; i < pack.Count; i++)
        {
            if (pack[i].status == true)
                names.Add(pack[i].name);
        }

        if (names.Count > 0)
        {
            int rand = Random.Range(0, names.Count - 1);
            randomWord = names[rand];
        }
        if (names.Count == 0)
            onNamesEmpty?.Invoke(false);
        else
            onNamesEmpty?.Invoke(true);

    }
    private int GetNumberOfPlayers()
    {
        return numberOfPlayers;
    }
    private int GetNumberOfSpyes()
    {
        return numberOfSpyes;
    }
    private int GetTimer()
    {
        return timer;
    }
    private string GetRandomWord()
    {
        return randomWord;
    }
}
