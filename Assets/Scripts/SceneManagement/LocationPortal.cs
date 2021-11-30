using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private DestionationIdentifier destinationPortal;
    [SerializeField] private Transform spawnPoint;

    private PlayerController player;

    public Transform SpawnPoint => spawnPoint;
    public DestionationIdentifier DestinationPortal => destinationPortal;

    public bool IsRepeatable => false;

    public void OnPlayerTrigger(PlayerController player)
    {
        this.player = player;
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Teleport());
    }

    private IEnumerator Teleport()
    {
        GameController.Instance.PauseGame(true);

        yield return Fader.Instance.FadeIn(.5f);

        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.transform.position = destPortal.SpawnPoint.position;

        yield return new WaitForSeconds(.5f);
        yield return Fader.Instance.FadeOut(.5f);

        GameController.Instance.PauseGame(false);
    }

    public IEnumerator Teleport(PlayerController player, System.Action action = null)
    {
        if (player != null)
            this.player = player;

        yield return Fader.Instance.FadeIn(.5f);

        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.transform.position = destPortal.SpawnPoint.position;

        yield return new WaitForSeconds(.5f);
        yield return Fader.Instance.FadeOut(.5f);

        GameController.Instance.PauseGame(false);

        action?.Invoke();
    }
}
