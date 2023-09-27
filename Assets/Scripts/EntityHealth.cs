using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public float maxHealth;
    [HideInInspector] public float currentHealth;

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
            if (currentHealth <= 0f)
            {
                OnDeath();
            }
        }  
    }

    public void OnDeath()
    {
        Debug.Log("ded xp");
    }

    public void AddHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
