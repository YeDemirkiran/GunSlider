using UnityEngine;

public class HeatSensorAudio : MonoBehaviour
{
    [SerializeField] private HeatSensor sensor;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sensorClip;
    [SerializeField] private float soundHeatThreshold = 40f, minBPM, maxBPM;

    private float currentBPM;
    private float soundTimer;

    private void Update()
    {
        if (sensor.currentHeat > soundHeatThreshold)
        {
            currentBPM = Mathf.Lerp(minBPM, maxBPM, (sensor.currentHeat - soundHeatThreshold) / (sensor.maxHeat - soundHeatThreshold));

            if (soundTimer > 60f / currentBPM)
            {
                soundTimer = 0f;
                audioSource.PlayOneShot(sensorClip);
            }
            else
            {
                soundTimer += Time.deltaTime;
            }
        }
        else
        {
            soundTimer = 0f;
        }
    }
}
