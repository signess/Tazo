using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action OnEncountered;

    public float MoveSpeed;
    public LayerMask SolidObjectsLayer;
    public LayerMask InteractablesLayer;
    public LayerMask WildAreaLayer;

    private bool isMoving;

    private Vector2 input;

    private CharacterAnimator animator;

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.MoveX = input.x;
                animator.MoveY = input.y;

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.z += input.y;

                if(IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }

        animator.IsMoving = isMoving;

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    private void Interact()
    {
        var facingDir = new Vector3(animator.MoveX, 0, animator.MoveY);
        var interactPos = transform.position + facingDir;

        Collider[] hitCollider = Physics.OverlapSphere(interactPos, 0.3f, InteractablesLayer);
        if(hitCollider.Length > 0)
        {
            hitCollider[0].GetComponent<IInteractable>()?.Interact();
        }

    }

    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, MoveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        Collider[] hitCollider = Physics.OverlapSphere(targetPos + new Vector3(0, 0.5f, 0), 0.3f, SolidObjectsLayer | InteractablesLayer);
        if (hitCollider.Length > 0)
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        Collider[] wildAreaCollider = Physics.OverlapSphere(transform.position, 0.1f, WildAreaLayer);
        if (wildAreaCollider.Length > 0)
        {
            Debug.Log("Wild Area");
            if(UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.IsMoving = false;
                OnEncountered();
            }
        }
    }
}
