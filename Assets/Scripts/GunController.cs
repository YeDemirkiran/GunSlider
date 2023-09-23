using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("GENERAL")]
    [SerializeField] private float sensitivity;
    [SerializeField] private Vector2 angleClamp;
    [SerializeField] private bool invertY = true;

    [SerializeField] private Bullet[] bullets;
    [SerializeField] private float bulletForce;
    private int currentBullet = 0;

    [Header("SHOOTING SETTINGS")]
    [SerializeField] private int maxAmmo = 7;
    [SerializeField] private float shootingCooldown = 0.125f;
    private float shootingTimer = 0f;
    private bool canShoot = true;

    [Header("SHAKE SETTINGS")]
    [SerializeField] private float shootShakeDuration = 0.125f;
    [SerializeField] private float shootShakeAmplitude = 0.1f;
    [SerializeField] private float shootShakeFrequency = 25f;

    [Header("AUDIO")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] shotClips;
    [SerializeField] private Vector2 pitchRandomness;

    private Vector3 rotation;

    private void Start()
    {
        rotation = transform.localEulerAngles;
        
        foreach (var bullet in bullets)
        {
            bullet.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        rotation.x += (invertY ? -1 : 1) * Input.GetAxis("Mouse Y") * sensitivity;
        rotation.x = Mathf.Clamp(rotation.x, angleClamp.x, angleClamp.y);
        
        rotation.y += Input.GetAxis("Mouse X") * sensitivity;

        transform.localEulerAngles = rotation;

        if (canShoot)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                for (int i = 0; i < bullets.Length; i++)
                {
                    if (!bullets[currentBullet].hasFired)
                    {
                        canShoot = false;

                        bullets[currentBullet].transform.SetParent(null);
                        bullets[currentBullet].gameObject.SetActive(true);
                        bullets[currentBullet].Shoot(transform.forward * bulletForce, false);

                        currentBullet++;

                        if (currentBullet >= bullets.Length) currentBullet = 0;

                        // EFFECTS
                        CameraEffects.Instance.Shake(shootShakeDuration, shootShakeAmplitude, shootShakeFrequency);

                        audioSource.pitch = Random.Range(pitchRandomness.x, pitchRandomness.y);
                        audioSource.PlayOneShot(shotClips[Random.Range(0, shotClips.Length)]);

                        break;
                    }
                }
            }
        }
        else
        {
            if (shootingTimer < shootingCooldown)
            {
                shootingTimer += Time.deltaTime;
            }
            else
            {
                canShoot = true;
                shootingTimer = 0;
            }
        }  
    }
}