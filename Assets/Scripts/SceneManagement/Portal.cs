using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private int sceneToLoad = -1;
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
        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        var destPortal = FindObjectsOfType<Portal>().First(x => x != this);
        player.transform.position = destPortal.SpawnPoint.position;
        Destroy(gameObject);
    }

}
