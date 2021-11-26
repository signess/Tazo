using UnityEngine;

[CreateAssetMenu(fileName = "Tazocatcher", menuName = "Items/Tazocatcher Item")]
public class TazocatcherItem : ItemBase
{
    [SerializeField] private Sprite sprite;

    [SerializeField] private float catchRateModifier = 1;
    public Sprite Sprite => sprite;
    public float CatchRateModifier => catchRateModifier;

    public override bool CanUseOutsideBattle => false;

    public override bool Use(Tazo tazo)
    {
        return true;
    }
}