using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[AddComponentMenu("Heat System/Heat Burn Marker", 5)]
public class HeatBurnMarker : MonoBehaviour
{
    private enum BurnMode { SingleMesh, Humanoid }

    [Header("GENERAL")]
    [SerializeField] private BurnMode burnMode;

    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private HeatSensor sensor;
    [SerializeField] private float heatThreshold;

    [Header("BURNS")]
    [SerializeField] private Material[] decalMaterials;
    [SerializeField] private Vector3 markMinSize, markMaxSize;
    [SerializeField] private int maxMarkCount;
    [SerializeField] private float markAppearDelay;
    [SerializeField] private float burningTime, healingSpeed;

    [Header("EXTRAs")]
    [SerializeField] private GameObject fireVFXPrefab;

    private int currentMarkCount;
    private float delayTimer;

    private void Update()
    {
        if (currentMarkCount < maxMarkCount && sensor.currentHeat > heatThreshold)
        {
            if (delayTimer > markAppearDelay)
            {
                delayTimer = 0f;
                currentMarkCount++;

                StartCoroutine(CreateMark(sensor.currentHeatSource.transform, burningTime));
            }
            else
            {
                delayTimer += Time.deltaTime;
            }
        }
    }

    private IEnumerator CreateMark(Transform heatSource, float duration)
    {
        float lerp = 0f;

        GameObject mark = new GameObject("BurnMark");
        DecalProjector projector = mark.AddComponent<DecalProjector>();

        float distance = Vector3.Distance(heatSource.position, transform.position);
        Vector3 raycastDirection = (transform.position - heatSource.position).normalized;

        Physics.Raycast(heatSource.position, raycastDirection, out RaycastHit hit, distance);

        mark.transform.position = hit.point;
        mark.transform.rotation = Quaternion.LookRotation(raycastDirection);
        mark.transform.parent = transform;

        projector.size = Vector3Extensions.Random(markMinSize, markMaxSize);
        projector.pivot = new Vector3(0f, 0f, projector.size.z / 2f);

        projector.material = decalMaterials.GetRandom();
        projector.renderingLayerMask = 256;
        projector.fadeFactor = 0f;

        if (fireVFXPrefab != null)
        {
            GameObject fire = Instantiate(fireVFXPrefab);

            fire.transform.position = hit.point;
            fire.transform.rotation = Quaternion.LookRotation(-raycastDirection);
            fire.transform.parent = mark.transform;
        }

        Bounds meshBounds = meshRenderer.bounds;


        //RaycastHit[] hits = Physics.RaycastAll(heatSource.position, raycastDirection, distance);

        //foreach (var item in collection)
        //{

        //}

        while (lerp < 1f)
        {
            lerp += Time.deltaTime / duration;

            projector.fadeFactor = Mathf.Lerp(0f, 1f, lerp);

            yield return null;
        }
    }
}