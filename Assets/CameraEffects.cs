using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [HideInInspector] public static CameraEffects Instance;

    private Coroutine currentShake;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Shake(float duration, float amplitude, float frequency)
    {
        if (currentShake != null)
        {
            StopCoroutine(currentShake);
        }
        
        currentShake = StartCoroutine(IShake(duration, amplitude, frequency));
    }

    private IEnumerator IShake(float duration, float amplitude, float frequency)
    {
        float durationTimer = 0f;
        float frequencyTimer = 0f;
        float frequencyThreshold = 1 / frequency;

        Vector3 defaultPosition = transform.localPosition;


        while (durationTimer < duration)
        {
            if (frequencyTimer < frequencyThreshold)
            {
                frequencyTimer += Time.deltaTime;
            } else
            {
                frequencyTimer = 0f;

                transform.localPosition = defaultPosition + Random.insideUnitSphere * amplitude;
            }

            durationTimer += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = defaultPosition;
    }
}
