using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Heat System/Heat Meter", 2)]

public class HeatMeter : MonoBehaviour
{
    [SerializeField] private HeatSensor sensor;
    [SerializeField] private Slider heatSlider;

    [SerializeField] private Gradient heatColor;

    private Image fillRectImage;

    private void Awake()
    {
        fillRectImage = heatSlider.fillRect.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentHeat = sensor.currentHeat / sensor.maxHeat;
        heatSlider.value = currentHeat;

        fillRectImage.color = heatColor.Evaluate(currentHeat);
    }
}