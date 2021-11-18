using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    public string Name => name;
    public string Description => description;
    public Sprite Icon => icon;

    public virtual bool Use(Tazo tazo)
    {
        return false;
    }
}
