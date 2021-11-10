using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectsLoader : MonoBehaviour
{
    [SerializeField] private GameObject essentialObjectsPrefab;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();
        if (existingObjects.Length == 0)
            Instantiate(essentialObjectsPrefab, Vector3.zero, Quaternion.identity);
    }
}
