using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DestionationIdentifier
{ A, B, C, D, E }

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private int sceneToLoad = -1;
    [SerializeField] private DestionationIdentifier destinationPortal;
    [SerializeField] private Transform spawnPoint;

    private PlayerController player;

    public Transform SpawnPoint => spawnPoint;

    public void OnPlayerTrigger(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
                DontDestroyOnLoad(gameObject);
        GameController.Instance.PauseGame(true);

        yield return Fader.Instance.FadeIn(.5f);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.transform.position = destPortal.SpawnPoint.position;

        yield return new WaitForSeconds(.5f);
        yield return Fader.Instance.FadeOut(.5f);

        GameController.Instance.PauseGame(false);
        Destroy(gameObject);
    }
}