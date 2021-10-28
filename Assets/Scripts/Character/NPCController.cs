using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialog dialog;

    public void Interact()
    {
        DialogManager.Instance.ShowDialog(dialog).GetAwaiter();
    }
}
