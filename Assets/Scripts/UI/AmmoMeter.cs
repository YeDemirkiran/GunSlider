using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoMeter : MonoBehaviour
{
    [SerializeField] private GunController gun;
    [SerializeField] private TMP_Text ammoText;

    [SerializeField] private Color normalColor, depletedColor;

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            ammoText.text = $"{gun.currentAmmo} / {gun.maxAmmo}";

            if (gun.currentAmmo <= 0)
            {
                ammoText.color = depletedColor;
            }
            else
            {
                ammoText.color = normalColor;
            }
        }  
    }
}
