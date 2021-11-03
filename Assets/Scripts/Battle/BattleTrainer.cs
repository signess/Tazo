using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrainer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer trainerSprite;
    public SpriteRenderer TrainerSprite { get => trainerSprite;}

    public IEnumerator PlayExitAnimation()
    {
        TrainerSprite.color = new Color(TrainerSprite.color.r, TrainerSprite.color.g, TrainerSprite.color.b, 255);
        var sequence = DOTween.Sequence();
        yield return sequence.Append(transform.DOLocalMoveX(3.5f, 1f)).Join(TrainerSprite.DOFade(0, 1f)).SetEase(Ease.OutSine).WaitForCompletion();
    }
}
