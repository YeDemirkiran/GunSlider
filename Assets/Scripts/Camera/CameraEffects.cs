using System.Collections;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [HideInInspector] public static CameraEffects Instance;

    public float moveDuration { get; set; } = 1f;

    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine currentShake;
    private Coroutine currentMoveTowards;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
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

    public void MoveTowards(Transform target)
    {
        if (currentMoveTowards != null)
        {
            StopCoroutine(currentMoveTowards);
        }

        currentMoveTowards = StartCoroutine(IMoveTowards(target));
    }

    private IEnumerator IMoveTowards(Transform target)
    {
        float lerp = 0f;

        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        Vector3 targetPosition = target.position;
        Quaternion targetRotation = target.rotation;

        while (lerp < 1f)
        {
            lerp += Time.deltaTime / moveDuration;

            transform.position = Vector3.Lerp(initialPosition, targetPosition, moveCurve.Evaluate(lerp));
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, lerp);

            yield return null;
        }
    }
}