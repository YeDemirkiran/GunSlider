using UnityEngine;

[AddComponentMenu("Heat System/Heat Damage", 1)]
public class HeatDamage : MonoBehaviour
{
    [SerializeField] private EntityHealth health;
    [SerializeField] private HeatSensor sensor;

    [SerializeField] private float maxDamagePerSecond, damageThreshold;
    [SerializeField] private AnimationCurve damageCurve;

    // Update is called once per frame
    void Update()
    {
        if (sensor.currentHeat > damageThreshold)
        {
            float eval = (sensor.currentHeat - damageThreshold) / (sensor.maxHeat - damageThreshold);
            health.AddHealth(-maxDamagePerSecond * damageCurve.Evaluate(eval) * Time.deltaTime);
        }
    }
}