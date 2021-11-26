using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemBase item;

    public bool Used { get; set; } = false;
    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            initiator.GetComponent<Inventory>().AddItem(item);
            Used = true;

            GetComponentInChildren<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;

            string playerName = initiator.GetComponent<PlayerController>().Name;

            yield return DialogManager.Instance.ShowDialog($"{playerName} found {item.Name}!");
        }
    }

}
