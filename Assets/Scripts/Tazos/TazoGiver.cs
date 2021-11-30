using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TazoGiver : MonoBehaviour, ISavable
{
    [SerializeField] private Tazo tazoToGive;
    [SerializeField] private Dialog dialog;

    private bool used = false;

    public IEnumerator GiveTazo(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        tazoToGive.Init();
        player.GetComponent<TazoParty>().AddTazo(tazoToGive);

        used = true;

        string dialogText = $"{player.Name} received {tazoToGive.Base.Name}!";

        yield return DialogManager.Instance.ShowDialog(dialogText);
    }

    public bool CanBeGiven()
    {
        return tazoToGive != null && !used;
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;
    }
}
