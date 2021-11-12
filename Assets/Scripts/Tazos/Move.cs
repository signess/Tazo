public class Move
{
    public MoveBase Base { get; set; }
    public int EP { get; set; }

    public Move(MoveBase mBase)
    {
        Base = mBase;
        EP = mBase.EP;
    }

    public Move(MoveSaveData saveData)
    {
        Base = MoveDB.GetMoveByName(saveData.Name);
        EP = saveData.EP;
    }

    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            Name = Base.Name,
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
