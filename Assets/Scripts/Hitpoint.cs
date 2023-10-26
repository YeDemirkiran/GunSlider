using UnityEngine;

public class Hitpoint : MonoBehaviour
{
    public bool hitThisFrame {  get; private set; }
    public Bullet currentBullet{ get; private set; }

    public new Collider collider;
    public float damageMultiplier = 1f;

    public AudioSource audioSource;
    public AudioClip[] clipsOnHits;

    [SerializeField] Vector2 pitchRandomness = new Vector2(0.9f, 1.1f);
    [SerializeField] LayerMask layerMask;

    // Update is called once per frame
    void Update()
    {
        // RESET
        currentBullet = null;
        hitThisFrame = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Bullet bullet))
        {
            hitThisFrame = true;
            currentBullet = bullet;

            PlayAudio();
        }

        //if ((layerMask & (1 << collision.gameObject.layer)) != 0)
        //{
        //    hitThisFrame = true;
        //    currentCollision = collision;

        //    PlayAudio();
        //}
    }

    public void PlayAudio()
    {
        AudioUtilities.PlayRandomSound(audioSource, clipsOnHits, pitchRandomness);
    }
}