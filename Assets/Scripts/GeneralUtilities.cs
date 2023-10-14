using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AudioUtilities
{
    public static void PlayRandomSound(AudioSource source, AudioClip[] clips, Vector2 pitchRandomness)
    {
        source.pitch = Random.Range(pitchRandomness.x, pitchRandomness.y);
        source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}

public class AnimationUtilities
{
    public class LerpFloat
    {
        public float value;
        public bool isLerping { get; private set; }
        private Coroutine currentCoroutine;

        public LerpFloat(float value)
        {
            this.value = value;
            currentCoroutine = null;
        }

        public static implicit operator float(LerpFloat lerpFloat)
        {
            return lerpFloat.value;
        }

        public void Lerp(MonoBehaviour callingScript, float target, float duration, bool unscaledTime = false)
        {
            if (value == target) { return; }

            if (currentCoroutine != null)
            {
                callingScript.StopCoroutine(currentCoroutine);
            }

            currentCoroutine = callingScript.StartCoroutine(LerpCoroutine(target, duration, unscaledTime));
        }

        private IEnumerator LerpCoroutine(float target, float duration, bool unscaledTime = false)
        {
            isLerping = true;

            float timer = 0f;
            float originalValue = value;

            while (timer < 1.1f)
            {
                timer += (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) / duration;
                //timer += Time.deltaTime / duration;

                value = Mathf.Lerp(originalValue, target, timer);

                yield return null;
            }

            value = target;

            isLerping = false;
            currentCoroutine = null;
        }
    } 
}