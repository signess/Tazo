using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private bool isPlayerUnit;
    [SerializeField] private BattleHUD hud;

    public Tazo Tazo { get; set; }

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector3 originalPos, originalScale;
    private Color originalColor;

    public bool IsPlayerUnit { get => isPlayerUnit; }
    public BattleHUD HUD { get => hud; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        originalPos = transform.localPosition;
        originalScale = transform.localScale;
        originalColor = spriteRenderer.color;
    }

    public void Setup(Tazo tazo, bool isTrainerTazo = false)
    {
        Tazo = tazo;
        if (isPlayerUnit)
        {
            spriteRenderer.sprite = Tazo.Base.BackSprite;
            animator.runtimeAnimatorController = Tazo.Base.AnimBack;
            transform.localScale = Vector3.zero;
        }
        else
        {
            spriteRenderer.sprite = Tazo.Base.FrontSprite;
            animator.runtimeAnimatorController = Tazo.Base.AnimFront;
            transform.localPosition = originalPos;
            if (isTrainerTazo)
                transform.localScale = Vector3.zero;
            else
                transform.localScale = new Vector3(.7f, .7f, .7f);
        }
        hud.gameObject.SetActive(true);
        hud.SetData(tazo);

        spriteRenderer.DOFade(1, 0);
        //PlayEnterAnimation();
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public IEnumerator PlayWildEnterAnimation()
    {
        yield return transform.DOJump(transform.position, 1, 2, 1.5f).WaitForCompletion();
    }

    public IEnumerator PlayEnterAnimation()
    {
        transform.localScale = Vector3.zero;
        var sequence = DOTween.Sequence();
        yield return sequence.Append(transform.DOScale(originalScale, 1f)).SetEase(Ease.OutSine).Join(spriteRenderer.DOFade(1, .5f)).WaitForCompletion();
    }

    public IEnumerator PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            yield return sequence.Append(transform.DOPunchPosition(new Vector3(1f, 0f, 0f), 0.5f, 5)).WaitForCompletion();
        else
            yield return sequence.Append(transform.DOPunchPosition(new Vector3(-1f, 0f, 0f), 0.5f, 5)).WaitForCompletion();
    }

    public IEnumerator PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        yield return sequence.Append(spriteRenderer.DOColor(Color.gray, 0.1f)).Append(spriteRenderer.DOColor(originalColor, 0.1f))
            .Append(spriteRenderer.DOColor(Color.gray, 0.1f)).Append(spriteRenderer.DOColor(originalColor, 0.1f))
            .Append(spriteRenderer.DOColor(Color.gray, 0.1f)).Append(spriteRenderer.DOColor(originalColor, 0.1f)).WaitForCompletion();
    }

    public IEnumerator PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        yield return sequence.Append(transform.DOScale(Vector3.zero, .5f)).Join(spriteRenderer.DOFade(0, .5f)).WaitForCompletion();
    }

    public IEnumerator PlayReturnAnimation()
    {
        var sequence = DOTween.Sequence();
        yield return sequence.Append(transform.DOScale(Vector3.zero, .5f)).Join(spriteRenderer.DOFade(0, .5f)).WaitForCompletion();
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        yield return sequence.Append(spriteRenderer.DOFade(0, .5f))
            .Join(transform.DOLocalMoveY(originalPos.y + 1f, .5f))
            .Join(transform.DOScale(new Vector3(.3f, .3f, .3f), .5f))
            .WaitForCompletion();
    }
    public void PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(spriteRenderer.DOFade(1, .5f))
            .Join(transform.DOLocalMoveY(originalPos.y, .5f))
            .Join(transform.DOScale(originalScale, .5f));
    }
}