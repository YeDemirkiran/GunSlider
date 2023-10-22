using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    public float maxHealth;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public bool isDead = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] soundsOnHit, soundsOnDeath;

    [SerializeField] private Behaviour[] componentsDisabledAtDeath;

    [SerializeField] private UnityEvent onDeath;

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            if (!isDead & currentHealth <= 0f)
            {
                OnDeath();
                isDead = true;
            }
        }  
    }

    public void OnDeath()
    {
        if (audioSource != null && soundsOnDeath.Length > 0)
        {
            AudioUtilities.PlayRandomSound(audioSource, soundsOnDeath, Vector2.one, true);
        }

        foreach (var component in componentsDisabledAtDeath)
        {
            component.enabled = false;
        }

        onDeath.Invoke();
    }

    public void AddHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (!isDead && amount <= 0f)
        {
            if (audioSource != null && soundsOnHit.Length > 0)
            {
                AudioUtilities.PlayRandomSound(audioSource, soundsOnHit, Vector2.one);
            }
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        foreach (var component in componentsDisabledAtDeath)
        {
            component.enabled = true;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //Debug.Log("SOMETHING HIT");

    //    if (!isDead)
    //    {
    //        if (collision.transform.CompareTag("Bullet"))
    //        {
    //            //Debug.Log("WE'RE HIT!");
    //            AddHealth(-10f);

    //            if (audioSource != null && soundsOnHit.Length > 0)
    //            {
    //                AudioUtilities.PlayRandomSound(audioSource, soundsOnHit, Vector2.one);
    //            }
    //        }
    //    }    
    //}
}
