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
        Bounds bounds = new Bounds();

        switch (calculationMethod)
        {
            case CalculationMethod.MeshBased:
                bounds = CalculateBounds(meshes);

                break;

            case CalculationMethod.ColliderBased:
                bounds = CalculateBounds(colliders);

                break;
        }

        return bounds;
    }

    private Bounds CalculateBounds(Renderer[] meshes)
    {
        if (meshes.Length == 0) return new Bounds(transform.position, Vector3.zero);

        Bounds bounds = meshes[0].bounds;

        foreach (Renderer renderer in meshes)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    private Bounds CalculateBounds(Collider[] colliders)
    {
        if (colliders.Length == 0) return new Bounds(transform.position, Vector3.zero);

        Bounds bounds = colliders[0].bounds;

        foreach (Collider collider in colliders)
        {
            bounds.Encapsulate(collider.bounds);
        }

        return bounds;
    }
}