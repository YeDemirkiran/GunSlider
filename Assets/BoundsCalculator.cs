using UnityEngine;

public class BoundsCalculator : MonoBehaviour
{
    private enum CalculationMethod { MeshBased, ColliderBased }

    [SerializeField] private CalculationMethod calculationMethod;

    [SerializeField] private Renderer[] meshes;
    [SerializeField] private Collider[] colliders;

    [SerializeField] private bool drawBoundsAsGizmo;

    public Bounds bounds { get; private set; }

    private void Update()
    {
        switch (calculationMethod)
        {
            case CalculationMethod.MeshBased:
                bounds = CalculateBounds(meshes);

                break;

            case CalculationMethod.ColliderBased:
                bounds = CalculateBounds(colliders);

                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawBoundsAsGizmo) return;

        if (!Application.isPlaying)
        {
            switch (calculationMethod)
            {
                case CalculationMethod.MeshBased:
                    bounds = CalculateBounds(meshes);

                    break;

                case CalculationMethod.ColliderBased:
                    bounds = CalculateBounds(colliders);

                    break;
            }
        }    

        Gizmos.DrawWireCube(bounds.center, bounds.size);

        //if (meshes.Length > 0)
        //{
        //    bounds = CalculateBounds(meshes);
        //    Gizmos.DrawWireCube(bounds.center, bounds.size);

        //    //Gizmos.DrawSphere(bounds.center, 0.1f);
        //    //Gizmos.DrawSphere(bounds.GetFront() + bounds.GetTop(), 0.1f);
        //    //Gizmos.DrawSphere(bounds.GetBack(), 0.1f);
        //    //Gizmos.DrawSphere(bounds.GetRight(), 0.1f);
        //    //Gizmos.DrawSphere(bounds.GetLeft(), 0.1f);
        //    //Gizmos.DrawSphere(bounds.GetTop(), 0.1f);
        //    //Gizmos.DrawSphere(bounds.GetBottom(), 0.1f);
        //    //Gizmos.DrawSphere(bounds.GetBottom(), 0.1f);

        //Gizmos.DrawSphere(bounds.GetCross(new Vector3(0.5f, 1f, 0f)), 0.1f);
        //}        
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

    public Bounds CalculateBounds(Collider[] colliders)
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
