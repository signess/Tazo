using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI countText;
    private Image frame;

    private RectTransform rectTransform;

    public Image Frame => frame;

    public float Height => rectTransform.rect.height;

    public void SetData(ItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = itemSlot.Item.Name;
        countText.text = $"x {itemSlot.Count}";

        frame = GetComponent<Image>();
    }
}
