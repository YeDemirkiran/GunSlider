using UnityEngine;

public class BoundsCalculator : MonoBehaviour
{
    private enum CalculationMethod { MeshBased, ColliderBased }

    [SerializeField] private CalculationMethod calculationMethod;

    [SerializeField] private Renderer[] meshes;
    [SerializeField] private Collider[] colliders;

    [SerializeField] private bool drawBoundsAsGizmo;

    private void OnDrawGizmosSelected()
    {
        if (!drawBoundsAsGizmo) return;  

        Bounds bounds = CalculateBounds();

        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    public Bounds CalculateBounds()
    {
        switch (calculationMethod)
        {
            case CalculationMethod.MeshBased:
                return CalculateBounds(meshes);

            case CalculationMethod.ColliderBased:
                return CalculateBounds(colliders);
        }

        return default;
    }

    private Bounds CalculateBounds(Renderer[] meshes)
    {
        if (meshes.Length == 0) return new Bounds(transform.position, Vector3.zero);

        Bounds bounds = meshes[0].bounds;

        if (meshes.Length > 1)
        {
            foreach (Renderer renderer in meshes)
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        return bounds;
    }

    private Bounds CalculateBounds(Collider[] colliders)
    {
        if (colliders.Length == 0) return new Bounds(transform.position, Vector3.zero);

        Bounds bounds = colliders[0].bounds;

        if (colliders.Length > 1)
        {
            foreach (Collider collider in colliders)
            {
                bounds.Encapsulate(collider.bounds);
            }
        }      

        return bounds;
    }
}