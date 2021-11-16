using UnityEngine;

public class GrassPatchHandler : MonoBehaviour, IPlayerTriggerable
{
    private GameObject overlay;

    private void Awake()
    {
        overlay = transform.Find("Overlay").gameObject;
    }

    // Start is called before the first frame update
    private void Start()
    {
        overlay.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            overlay.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Collider[] hitColliders =
            Physics.OverlapSphere(
                new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), 0.15f);
        bool deactivate = true;
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].name == "Player")
            {
                deactivate = false;
                i = hitColliders.Length;
            }
        }
        if (deactivate)
        {
            overlay.SetActive(false);
        }
    }

    public void OnPlayerTrigger(PlayerController player)
    {
        Debug.Log("Wild Area");
        if (Random.Range(1, 101) <= 10)
        {
            player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle();
        }
    }
}