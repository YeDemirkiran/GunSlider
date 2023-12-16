using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioUtilities;

public class GunController : MonoBehaviour
{
    [Header("GENERAL")]
    [SerializeField] private float sensitivity;
    [SerializeField] private Vector2 xAngleClamp, yAngleClamp;
    [SerializeField] private bool invertY = true;

    [SerializeField] private Transform bulletHolder;
    private List<Bullet> bullets = new List<Bullet>();
    [SerializeField] private float bulletForce;

    [SerializeField] private ParticleSystem muzzleFlash;
    private int currentBullet = 0;

    [Header("SHOOTING SETTINGS")]
    public int maxAmmo = 7;
    [SerializeField] float bulletDamage = 10f;
    [HideInInspector] public int currentAmmo;
    [SerializeField] private float shootingCooldown = 0.125f;
    private bool canShoot = true, isCooldownRunning = false;

    [Header("SHAKE SETTINGS")]
    [SerializeField] private float shootShakeDuration = 0.125f;
    [SerializeField] private float shootShakeAmplitude = 0.1f;
    [SerializeField] private float shootShakeFrequency = 25f;

    [Header("AUDIO")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] shotClips, reloadClips, clickClips;
    [SerializeField] private Vector2 pitchRandomness;

    private bool clicked = false;

    Vector3 rotation;

    private void Start()
    {
        rotation = transform.eulerAngles;

        foreach (Transform item in bulletHolder)
        {
            if (item.TryGetComponent(out Bullet bullet))
            {
                bullets.Add(bullet);
            }
        }

        foreach (var bullet in bullets)
        {
            bullet.gameObject.SetActive(false);
        }

        currentAmmo = maxAmmo;
    }

    public void Rotate(float deltaX, float deltaY, bool invertY)
    {
        if (GameManager.isPaused) return;

        rotation.x += (invertY ? -1 : 1) * deltaX * sensitivity * Time.deltaTime;
        rotation.x = Mathf.Clamp(rotation.x, xAngleClamp.x, xAngleClamp.y);

        rotation.y += deltaY * sensitivity * Time.deltaTime;
        rotation.y = Mathf.Clamp(rotation.y, yAngleClamp.x, yAngleClamp.y);

        transform.eulerAngles = rotation;
    }


    public void Shoot()
    {
        if (GameManager.isPaused) return; 

        if (canShoot)
        {
            if (currentAmmo > 0)
            {
                for (int i = 0; i < bullets.Count; i++)
                {
                    if (!bullets[currentBullet].hasFired)
                    {
                        canShoot = false;

                        bullets[currentBullet].SetDamage(bulletDamage);

                        bullets[currentBullet].transform.SetParent(null);
                        bullets[currentBullet].gameObject.SetActive(true);
                        bullets[currentBullet].Shoot(transform.forward, bulletForce);

                        currentBullet++;

                        if (currentBullet >= bullets.Count) currentBullet = 0;

                        // EFFECTS
                        muzzleFlash.Play();
                        CameraEffects.Instance.Shake(shootShakeDuration, shootShakeAmplitude, shootShakeFrequency);

                        PlayRandomSound(audioSource, shotClips, pitchRandomness);

                        break;
                    }
                }

                currentAmmo--;

                if (!isCooldownRunning)
                {
                    StartCoroutine(ShootingCooldown());
                }
            }

            else
            {
                if (!clicked)
                {
                    PlayRandomSound(audioSource, clickClips, pitchRandomness);
                    clicked = true;
                }
            }
        }
        else
        {
            if (!isCooldownRunning)
            {
                StartCoroutine(ShootingCooldown());
            }
        }
    }

    public void Reload()
    {
        if (GameManager.isPaused) return;

        if (currentAmmo < maxAmmo)
        {
            currentAmmo = maxAmmo;
            PlayRandomSound(audioSource, reloadClips, pitchRandomness);
            clicked = false;
        }
    }

    private IEnumerator ShootingCooldown()
    {
        isCooldownRunning = true;

        float timer = 0f;

        while (timer < shootingCooldown)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        canShoot = true;
        isCooldownRunning = false;
    }
}