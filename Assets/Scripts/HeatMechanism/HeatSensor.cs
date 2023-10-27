using UnityEngine;

public class HeatSensor : MonoBehaviour
{
    public float minHeat = 20f, maxHeat = 50f, degreeChangePerSecond, degreeGravityPerSecond;

    public float currentHeat = 50f;

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

        Debug.Log("Current Heat: " + currentHeat);
        Debug.Log("Desired Heat: " + desiredHeat);
    }

    public void AddHeat(float heat)
    {
        currentHeat += heat;
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("HEHEEHE");

        if (other.TryGetComponent(out HeatSource heatSource))
        {
            float distance = heatSource.radius - Vector3.Distance(transform.position, heatSource.transform.position);

            desiredHeat = heatSource.coreHeat * heatSource.heatCurve.Evaluate(distance / heatSource.radius);
        }
    }
}