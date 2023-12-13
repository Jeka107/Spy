using System;

[Serializable]
public class PackData:PacksData
{
    public string name;
    public bool status;

    public PackData()
    {
        name = "";
        status = true;
    }

    public PackData(string _name, bool _status)
    {
        name = _name;
        status = _status;
    }
}
