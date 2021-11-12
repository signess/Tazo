using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TazoParty : MonoBehaviour
{
    [SerializeField] List<Tazo> tazos;

    public List<Tazo> Tazos { get => tazos; set => tazos = value; }

    private void Start()
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
        if(tazos.Count < 6)
        {
            tazos.Add(newTazo);
        }
        else
        {
            //transfer to pc
        }
    }
}
