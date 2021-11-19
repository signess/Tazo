using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CharacterAnimator))]
//Base class for all characters entities
public class Character : MonoBehaviour
{
    public float MoveSpeed;

    private CharacterAnimator animator;

    public CharacterAnimator Animator { get => animator; }

    public bool IsMoving { get; private set; }

    private void Awake()
    {
        animator = GetComponentInChildren<CharacterAnimator>();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    //recives a input then move the character
    public IEnumerator Move(Vector2 moveVector, System.Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.z += moveVector.y;

        if (!IsPathClear(targetPos))
            yield break;

        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, MoveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;
        var hitDetection = Physics.BoxCastAll(transform.position + dir, new Vector3(0.2f, 0.2f, 0.2f), dir, Quaternion.identity, diff.magnitude - 1, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.InteractablesLayer | GameLayers.Instance.PlayerLayer);
        if (hitDetection.Length > 0)
        {
            print("Colide com: " + hitDetection[0].collider.name);
            foreach (var hit in hitDetection)
            {
                var triggerable = hit.collider.GetComponent<DoorwayHandler>();
                if(triggerable != null)
                {
                    Animator.IsMoving = false;
                    triggerable.OnPlayerTrigger(this);
                    break;
                }
            }
            
            return false;
        }
        return true;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        Collider[] hitCollider = Physics.OverlapSphere(targetPos + new Vector3(0, 0.5f, 0), 0.3f, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.InteractablesLayer);
        if (hitCollider.Length > 0)
        {
            return false;
        }
        return true;
    }

    public void LookTowards(Vector3 targetPos)
    {
        var xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(targetPos.z) - Mathf.Floor(transform.position.z);

        if(xDiff == 0 || yDiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);
        }

    }
}
