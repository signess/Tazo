using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class TazoParty : MonoBehaviour
{
    public event Action OnUpdate;

    [SerializeField] private List<Tazo> tazos;

    public List<Tazo> Tazos
    {
        get => tazos;
        set
        {
            tazos = value;
            OnUpdate?.Invoke();
        }
    }

    private void Awake()
    {
        foreach (var tazo in tazos)
        {
            tazo.Init();
        }
    }

    public Tazo GetHealthyTazo()
    {
        return tazos.Where(x => x.HP > 0).FirstOrDefault();
    }

    public void AddTazo(Tazo newTazo)
    {
        if (tazos.Count < 6)
        {
            tazos.Add(newTazo);
            OnUpdate?.Invoke();
        }
        else
        {
            //transfer to pc
        }
    }

    public static TazoParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<TazoParty>();
    }
}