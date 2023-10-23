using UnityEngine;

[ExecuteInEditMode]
public class LaserColor : MonoBehaviour
{
    private Laser laser;
    private Light laserLight;

    [ColorUsage(false, false)][SerializeField] private Color laserColor;
    [SerializeField] private float colorIntensity;

    // Start is called before the first frame update
    void Awake()
    {
        laser = GetComponent<Laser>();
        laserLight = laser.laserLight.GetComponent<Light>();

        laser.lineRenderer.sharedMaterial.EnableKeyword("_EmissionColor");
    }

    // Update is called once per frame
    void Update()
    {
        laser.lineRenderer.sharedMaterial.color = laserColor;
        laser.lineRenderer.sharedMaterial.SetColor("_EmissionColor", laserColor * Mathf.Pow(2, colorIntensity));
        laserLight.color = laserColor;
    }
}