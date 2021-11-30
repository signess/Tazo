using UnityEngine;

public class Move
{
    public MoveBase Base { get; set; }
    public int EP { get; set; }

    public Move(MoveBase mBase)
    {
        Base = mBase;
        EP = mBase.EP;
    }

    public void IncreaseEP(int amount)
    {
        EP = Mathf.Clamp(EP + amount, 0, Base.EP);
    }

    public Move(MoveSaveData saveData)
    {
        Base = MoveDB.GetObjectByName(saveData.Name);
        EP = saveData.EP;
    }

    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            Name = Base.name,
            EP = EP
        };
        return saveData;
    }
}

[System.Serializable]
public class MoveSaveData
{
    public string Name;
    public int EP;
}
