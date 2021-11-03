using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class PlayerController : MonoBehaviour
{
    public event Action OnEncountered;
    public event Action<Collider> OnEnterTrainersView;

    [SerializeField] private string name;
    [SerializeField] private Sprite sprite;

    private Vector2 input;

    private Character character;

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                //if have input calls move on character script
                character.Move(input, OnMoveOver).GetAwaiter();
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    private void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, 0, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        Collider[] hitCollider = Physics.OverlapSphere(interactPos, 0.3f, GameLayers.Instance.InteractablesLayer);
        if(hitCollider.Length > 0)
        {
            hitCollider[0].GetComponent<IInteractable>()?.Interact(transform);
        }

    }

    private void OnMoveOver()
    {
        CheckForEncounters();
        CheckForTrainersView();
    }

    private void CheckForEncounters()
    {
        Collider[] wildAreaCollider = Physics.OverlapSphere(transform.position, 0.1f, GameLayers.Instance.WildAreaLayer);
        if (wildAreaCollider.Length > 0)
        {
            Debug.Log("Wild Area");
            if(UnityEngine.Random.Range(1, 101) <= 10)
            {
                character.Animator.IsMoving = false;
                OnEncountered();
            }
        }
    }

    private void CheckForTrainersView()
    {
        Collider[] fovColliders = Physics.OverlapSphere(transform.position, 0.1f, GameLayers.Instance.FOVLayer);
        if (fovColliders.Length > 0)
        {
            character.Animator.IsMoving = false;
            OnEnterTrainersView?.Invoke(fovColliders[0]);          
        }
    }
}
