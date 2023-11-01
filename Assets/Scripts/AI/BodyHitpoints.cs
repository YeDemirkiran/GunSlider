using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyHitpoints : MonoBehaviour
{
    [SerializeField] EntityHealth health;
    public Hitpoint[] hitpoints;
    [SerializeField] private AudioSource defaultAudioSource;
    [SerializeField] private AudioClip[] defaultHitClips;

    public float collectiveDamageThisFrame { get; private set; }

    private void Awake()
    {
        foreach (Hitpoint hitpoint in hitpoints)
        {
            if (hitpoint.audioSource == null)
            {
                hitpoint.audioSource = defaultAudioSource;
            }

            if (hitpoint.clipsOnHits == null || hitpoint.clipsOnHits.Length <= 0)
            {
                hitpoint.clipsOnHits = defaultHitClips;
            }
        }
    }

    private void Update()
    {
        collectiveDamageThisFrame = 0f;

        foreach (Hitpoint hitpoint in hitpoints)
        {
            if (hitpoint.hitThisFrame)
            {
                collectiveDamageThisFrame += hitpoint.currentBullet.damage * hitpoint.damageMultiplier;
            }
        }

        if (collectiveDamageThisFrame > 0f)
        {
            health.AddHealth(-collectiveDamageThisFrame);
            //Debug.Log("Damage this frame:" + collectiveDamageThisFrame);
        }
    }
}

#if UNITY_EDITOR
public class BodyHitpointsEditor : Editor
{
    
}
#endif