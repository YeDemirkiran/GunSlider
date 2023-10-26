using UnityEngine;

public class HeatSensor : MonoBehaviour
{
    public float minHeat = 20f, maxHeat = 50f, degreeGravityPerSecond;

    public float currentHeat = 50f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sensorAudio;
    [SerializeField] private float soundHeatThreshold = 40f, minBPM, maxBPM;
    private float currentBPM;
    private float soundTimer;

    void Update()
    {
        if (currentHeat > minHeat)
        {
            currentHeat -= Time.deltaTime * degreeGravityPerSecond * (currentHeat > maxHeat ? 10f : 1f);
        }
        else
        {
            currentHeat += Time.deltaTime * degreeGravityPerSecond;
        }

        //Debug.Log("Current Heat: " + currentHeat);

        if (currentHeat > soundHeatThreshold)
        {
            currentBPM = Mathf.Lerp(minBPM, maxBPM, (currentHeat - soundHeatThreshold) / (maxHeat - soundHeatThreshold));

            if (soundTimer > 60f / currentBPM)
            {
                soundTimer = 0f;
                audioSource.PlayOneShot(sensorAudio);
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

    public void AddHeat(float heat)
    {
        currentHeat += heat;
    }
}