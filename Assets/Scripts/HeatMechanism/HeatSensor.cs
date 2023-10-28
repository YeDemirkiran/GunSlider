using UnityEngine;

[AddComponentMenu("Heat System/Heat Sensor", 4)]
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class HeatSensor : MonoBehaviour
{
    public float minHeat = 20f, maxHeat = 50f, degreeChangePerSecond, degreeGravityPerSecond;

    public float currentHeat = 50f;

    public HeatSource currentHeatSource {  get; private set; }

    private float desiredHeat;

    void Update()
    {
        desiredHeat -= degreeGravityPerSecond * Time.deltaTime;
        desiredHeat = Mathf.Clamp(desiredHeat, minHeat, maxHeat);

        if (currentHeat > desiredHeat)
        {
            currentHeat -= Time.deltaTime * degreeChangePerSecond * (currentHeat > maxHeat ? 10f : 1f);
        }
        else
        {
            currentHeat += Time.deltaTime * degreeChangePerSecond;
        }
    }

    public void AddHeat(float heat)
    {
        currentHeat += heat;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out HeatSource heatSource))
        {
            currentHeatSource = heatSource;

            float distance = heatSource.radius - Vector3.Distance(transform.position, heatSource.transform.position);

            desiredHeat = heatSource.coreHeat * heatSource.heatCurve.Evaluate(distance / heatSource.radius);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out HeatSource heatSource))
        {
            currentHeatSource = null;
        }
    }
}