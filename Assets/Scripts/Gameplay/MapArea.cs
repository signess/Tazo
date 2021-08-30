using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] private List<Tazo> wildTazos;
    // Start is called before the first frame update
    public Tazo GetRandomWildTazo()
    {
        var wildTazo = wildTazos[Random.Range(0, wildTazos.Count)];
        wildTazo.Init();
        return wildTazo;
    }
}
