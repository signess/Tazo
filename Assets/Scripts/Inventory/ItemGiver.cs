using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int count = 1;
    [SerializeField] private Dialog dialog;

    private bool used = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        player.GetComponent<Inventory>().AddItem(item, count);

        used = true;

        string dialogText = $"{player.Name} received {item.Name}!";
        if(count > 1)
            dialogText = $"{player.Name} received {count} {item.Name}!";

        yield return DialogManager.Instance.ShowDialog(dialogText);
    }

    public bool CanBeGiven()
    {
        return item != null && count > 0 && !used;
    }
}
