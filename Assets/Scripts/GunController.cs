using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeneralUtilities;

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

    private Vector3 rotation;

    private bool clicked = false;

    private void Start()
    {
        rotation = transform.localEulerAngles;

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

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            Rotate(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), invertY);

            if (Input.GetKey(KeyCode.Mouse0))
            {
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
        }
    }

    public void Rotate(float deltaX, float deltaY, bool invertY)
    {
        rotation.x += (invertY ? -1 : 1) * deltaX * sensitivity;
        rotation.x = Mathf.Clamp(rotation.x, xAngleClamp.x, xAngleClamp.y);

        rotation.y += deltaY * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, yAngleClamp.x, yAngleClamp.y);

        transform.localEulerAngles = rotation;
    }

    public void Shoot()
    {
        if (canShoot)
        {
            if (currentAmmo > 0)
            {
                for (int i = 0; i < bullets.Count; i++)
                {
                    if (!bullets[currentBullet].hasFired)
                    {
                        canShoot = false;

                        bullets[currentBullet].transform.SetParent(null);
                        bullets[currentBullet].gameObject.SetActive(true);
                        bullets[currentBullet].Shoot(transform.forward * bulletForce);
                        //bullets[currentBullet].Shoot(transform.forward * bulletForce, false);

                        currentBullet++;

                        if (currentBullet >= bullets.Count) currentBullet = 0;

                        // EFFECTS
                        muzzleFlash.Play();
                        CameraEffects.Instance.Shake(shootShakeDuration, shootShakeAmplitude, shootShakeFrequency);

                        //audioSource.pitch = Random.Range(pitchRandomness.x, pitchRandomness.y);
                        //audioSource.PlayOneShot(shotClips[Random.Range(0, shotClips.Length)]);
                        PlayRandomSound(audioSource, shotClips, pitchRandomness);

                        break;
                    }
                }

                currentAmmo--;
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