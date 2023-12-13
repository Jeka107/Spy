using System;

[Serializable]
public class CustomPacksData:PacksData
{
    public string name;

    public CustomPacksData()
    {
        name = "";
    }

    public CustomPacksData(string _name)
    {
        name = _name;
    }
}
