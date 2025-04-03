using UnityEngine;

/// <summary>
/// Changes the material tint color of a highlighted object at runtime.
/// Used for visual feedback in interactive objects.
/// </summary>
public class ChangeHighlightMaterial : MonoBehaviour
{
    [SerializeField] private bool isFence;
    private MeshRenderer highlightMeshRenderer;
    private MeshRenderer[] highlightMeshRenderers;

    private void Awake()
    {
        if(!isFence)
            highlightMeshRenderer = GetComponentInChildren<MeshRenderer>();
        else
            highlightMeshRenderers = GetComponentsInChildren<MeshRenderer>();
            
    }

    private void Start()
    {
        if (!isFence)
        {
            highlightMeshRenderer.material.SetColor("_TintColor", Color.yellow);
        }
            
        else
            foreach (var mesh in highlightMeshRenderers)
                mesh.material.SetColor("_TintColor", Color.yellow);

    }
}
