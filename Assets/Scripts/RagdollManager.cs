using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    [SerializeField] private Rigidbody[] ragdollRigidbodies;
    [SerializeField] private bool activeAtStart;

    // Start is called before the first frame update
    void Awake()
    {
        if (activeAtStart)
        {
            ActivateRagdoll();
        }
        else
        {
            DeactivateRagdoll();
        }
    }

    public void ActivateRagdoll()
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.WakeUp();
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    public void DeactivateRagdoll()
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.Sleep();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}
