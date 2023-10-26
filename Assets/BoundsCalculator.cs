using UnityEngine;

public class BoundsCalculator : MonoBehaviour
{
    [SerializeField] private Renderer[] meshes;

    private Bounds bounds;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        bounds = CalculateBounds(meshes);
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    public Bounds CalculateBounds(Renderer[] meshes)
    {
        if (meshes.Length == 0) return new Bounds(transform.position, Vector3.zero);

        Bounds bounds = meshes[0].bounds;

        foreach (Renderer renderer in meshes)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }
}
