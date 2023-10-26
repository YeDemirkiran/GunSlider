using UnityEngine;

public class HeatSource : MonoBehaviour
{
    [SerializeField] private new SphereCollider collider;
    [SerializeField] private float maxDamagePerSecond;
    [SerializeField] private AnimationCurve damageCurve;


    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out EntityHealth health))
        {
            float distance = collider.radius - Vector3.Distance(transform.position, health.transform.position);
            float damage = damageCurve.Evaluate(distance / collider.radius) * maxDamagePerSecond * Time.deltaTime;
            health.AddHealth(-damage);
        }
    }
}
