using UnityEngine;

public class GameLayers : MonoBehaviour
{
    public static GameLayers Instance { get; set; }

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask interactablesLayer;
    [SerializeField] private LayerMask wildAreaLayer;
    [SerializeField] private LayerMask fovLayer;
    [SerializeField] private LayerMask portalLayer;
    [SerializeField] private LayerMask triggersLayer;

    public LayerMask PlayerLayer { get => playerLayer; }
    public LayerMask SolidObjectsLayer { get => solidObjectsLayer; }
    public LayerMask InteractablesLayer { get => interactablesLayer; }
    public LayerMask WildAreaLayer { get => wildAreaLayer; }
    public LayerMask FOVLayer { get => fovLayer; }
    public LayerMask PortalLayer => portalLayer;
    public LayerMask TriggerableLayers { get => wildAreaLayer | fovLayer | portalLayer|triggersLayer; }

    private void Awake()
    {
        Instance = this;
    }
}