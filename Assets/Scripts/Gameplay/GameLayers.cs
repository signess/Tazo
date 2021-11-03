using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    public static GameLayers Instance { get; set; }

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask interactablesLayer;
    [SerializeField] private LayerMask wildAreaLayer;
    [SerializeField] private LayerMask fovLayer;

    public LayerMask PlayerLayer { get => playerLayer; }
    public LayerMask SolidObjectsLayer { get => solidObjectsLayer; }
    public LayerMask InteractablesLayer { get => interactablesLayer; }
    public LayerMask WildAreaLayer { get => wildAreaLayer; }
    public LayerMask FOVLayer { get => fovLayer; }

    private void Awake()
    {
        Instance = this;
    }
}
