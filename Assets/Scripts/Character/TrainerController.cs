using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class TrainerController : MonoBehaviour, IInteractable
{
    [SerializeField] private string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Dialog dialog;
    [SerializeField] private Dialog lostDialog;
    [SerializeField] private GameObject exclamation;
    [SerializeField] private GameObject fov;

    private Character character;

    //State
    private bool battleLost = false;

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }

    private void Update()
    {
        character.HandleUpdate();
    }

    public async Task TriggerTrainerBattle(PlayerController player)
    {
        // Show Exclamation
        exclamation.SetActive(true);
        await Task.Delay(500);
        exclamation.SetActive(false);

        // Walk towards the player
        var diff = player.transform.position - transform.position;
        var diff2 = diff - diff.normalized;
        var moveVector = new Vector3(Mathf.Round(diff2.x), Mathf.Round(diff2.z));

        await character.Move(moveVector);

        //Show dialog
        await DialogManager.Instance.ShowDialog(dialog, () =>
        {
            print("Start trainer battle");
            GameController.Instance.StartTrainerBattle(this);
        });
    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Left)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Right)
            angle = 270f;

        fov.transform.eulerAngles = new Vector3(0f, angle, 0f);
    }

    public async void Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);
        if (!battleLost)
        {
            await DialogManager.Instance.ShowDialog(dialog, () =>
            {
                print("Start trainer battle");
                GameController.Instance.StartTrainerBattle(this);
            });
        }
        else
        {
            await DialogManager.Instance.ShowDialog(lostDialog);
        }
    }
}
