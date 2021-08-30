using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance { get; private set; }

    [SerializeField] private Color highlightedColorText;
    [SerializeField] private Color highlightedColorFrame;
    [SerializeField] private Sprite maleIcon, femaleIcon;
    [SerializeField] private Sprite normal, fire, water, grass, flying, fighting, poison, electric, ground, rock, psychic, ice, bug, ghost, steel, dragon, dark, fairy;

    public Color HighlightedColorText => highlightedColorText;
    public Color HighlightedColorFrame => highlightedColorFrame;
    public Sprite MaleIcon { get => maleIcon; }
    public Sprite FemaleIcon { get => femaleIcon; }
    public Sprite Normal { get => normal; }
    public Sprite Fire { get => fire; }
    public Sprite Water { get => water; }
    public Sprite Grass { get => grass; }
    public Sprite Flying { get => flying; }
    public Sprite Fighting { get => fighting; }
    public Sprite Poison { get => poison; }
    public Sprite Electric { get => electric; }
    public Sprite Ground { get => ground; }
    public Sprite Rock { get => rock; }
    public Sprite Psychic { get => psychic; }
    public Sprite Ice { get => ice; }
    public Sprite Bug { get => bug; }
    public Sprite Ghost { get => ghost; }
    public Sprite Steel { get => steel; }
    public Sprite Dragon { get => dragon; }
    public Sprite Dark { get => dark; }
    public Sprite Fairy { get => fairy; }

    private void Awake()
    {
        Instance = this;
    }
}