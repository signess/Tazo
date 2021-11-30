using UnityEngine;

public class WildArea : MonoBehaviour, IPlayerTriggerable
{
    public bool IsRepeatable => true;

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
