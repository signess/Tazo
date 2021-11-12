using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TazoDB
{
    static Dictionary<string, TazoBase> tazos;

    public static void Init()
    {
        tazos = new Dictionary<string, TazoBase>();

        var tazosArray = Resources.LoadAll<TazoBase>("");
        foreach(var tazo in tazosArray)
        {
            if(tazos.ContainsKey(tazo.Name))
            {
                Debug.LogError($"There are two tazo with the name {tazo.Name}");
                continue;
            }

            tazos[tazo.Name] = tazo;
        }
    }

    public static TazoBase GetTazoByName(string name)
    {
        if(!tazos.ContainsKey(name))
        {
            Debug.LogError($"Tazo with name {name} not found in the database!");
            return null;
        }

        return tazos[name];
    }
}
