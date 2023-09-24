using TMPro;
using UnityEngine;

public class HealthMeter : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    [SerializeField] private TMP_Text healthText;

    [SerializeField] private Color normalColor, depletedColor;

    // Update is called once per frame
    void Update()
    {
        healthText.text = $"{health.currentHealth}";

        healthText.color = Color.Lerp(normalColor, depletedColor, 1f - (health.currentHealth / health.maxHealth));
    }
}
