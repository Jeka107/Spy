using System;
using System.Collections.Generic;

[Serializable]
public class SavedData
{
    public int players;
    public int spyes;
    public int time;
    public string packName;
    public List<PackData> pack;

    public SavedData()
    {
        players = 3;
        spyes = 1;
        time = 1;
        packName = "";
        pack = new List<PackData>();
    }

    public SavedData(int _players,int _spyes,int _time,string _packName, List<PackData> _pack)
    {
        players = _players;
        spyes = _spyes;
        time = _time;
        packName = _packName;
        pack = _pack;
    }
}
