using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour
{
    public Image Icon { get; private set; }
    public TextMeshProUGUI Text { get; private set; }

    private void OnEnable()
    {
        if (Icon == null)
            Icon = GetComponentInChildren<Image>();
        if (Text == null)
            Text = GetComponentInChildren<TextMeshProUGUI>();
    }
}