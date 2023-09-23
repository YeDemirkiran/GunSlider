using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private Vector2 pitchRandomness;

    private void OnCollisionEnter(Collision collision)
    {
        source.pitch = Random.Range(pitchRandomness.x, pitchRandomness.y);
        source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}
