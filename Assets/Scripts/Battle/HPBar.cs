using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image health;
    [SerializeField] TextMeshProUGUI healthText;

    private int _hp;
    private int _maxHp;

    public void SetHP(int hp, int maxHp)
    {
        _hp = hp;
        _maxHp = maxHp;
        health.fillAmount = (float)hp / maxHp;
        healthText.text = $"{hp}/{maxHp}";
    }

    public IEnumerator SetHPAsync(int newHp)
    {
        WaitForSeconds wait = new WaitForSeconds(1f/ 60f);
        int currentHp = _hp;
        int stepAmount;

        if (newHp - currentHp < 0)
            stepAmount = Mathf.FloorToInt((newHp - currentHp) / (60f * _maxHp));
        else
            stepAmount = Mathf.CeilToInt((newHp - currentHp) / (60f * _maxHp));
        if(currentHp < newHp)
        {
            while(currentHp < newHp)
            {
                currentHp += stepAmount;
                if(currentHp > newHp)
                {
                    currentHp = newHp;
                }
                healthText.text = $"{currentHp}/{_maxHp}";
                health.fillAmount = (float)currentHp/_maxHp;
                yield return wait;
            }
            _hp = newHp;
        }
        else
        {
            while (currentHp > newHp)
            {
                currentHp += stepAmount;
                if (currentHp < newHp)
                {
                    currentHp = newHp;        
                }
                healthText.text = $"{currentHp}/{_maxHp}";
                health.fillAmount = (float)currentHp / _maxHp;
                yield return wait;
            }
            _hp = newHp;
        }
    }
}
