using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorwayHandler : MonoBehaviour
{
    [SerializeField] private GameObject doorObject;
    [SerializeField] private LocationPortal portal;

    private Vector3 originalRotation;

    private PlayerController player;
    private void Start()
    {
        if (doorObject != null)
        {
            originalRotation = doorObject.transform.eulerAngles;
        }
    }

    public void OnPlayerTrigger(Character player)
    {
        this.player = player.GetComponent<PlayerController>();
        StartCoroutine(Teleport());
    }

    public IEnumerator Teleport()
    {
        GameController.Instance.PauseGame(true);
        yield return doorObject.transform.DOLocalRotate(new Vector3(doorObject.transform.eulerAngles.x, 0, doorObject.transform.eulerAngles.z), .5f).WaitForCompletion();
        StartCoroutine(portal.Teleport(player, ResetTransform));
        
    }

    public void ResetTransform()
    {
        doorObject.transform.eulerAngles = originalRotation;
    }
}
