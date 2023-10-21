using UnityEngine;

public class Hitpoint : MonoBehaviour
{
    public bool hitThisFrame {  get; private set; }
    public Collision currentCollision { get; private set; }

    public float hitDamage = 1f;

    public AudioSource audioSource;
    public AudioClip[] clipsOnHits;

    [SerializeField] Vector2 pitchRandomness = new Vector2(0.9f, 1.1f);
    [SerializeField] LayerMask layerMask;

    // Update is called once per frame
    void Update()
    {
        // RESET
        currentCollision = null;
        hitThisFrame = false;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if ((layerMask & (1 << collision.gameObject.layer)) != 0)
        {
            hitThisFrame = true;
            currentCollision = collision;

            PlayAudio();

            Debug.Log("HIT THIS FRAME:");
            Debug.Log(hitThisFrame);
        }
    }

    public void PlayAudio()
    {
        AudioUtilities.PlayRandomSound(audioSource, clipsOnHits, pitchRandomness);
    }
}