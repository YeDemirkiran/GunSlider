using UnityEngine;

public class GeneralUtilities
{
    public static void PlayRandomSound(AudioSource source, AudioClip[] clips, Vector2 pitchRandomness)
    {
        source.pitch = Random.Range(pitchRandomness.x, pitchRandomness.y);
        source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}
