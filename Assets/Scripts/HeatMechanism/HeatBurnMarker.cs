using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[AddComponentMenu("Heat System/Heat Burn Marker", 5)]
public class HeatBurnMarker : MonoBehaviour
{
    private enum BurnMode { MeshBased, ColliderBased }

    [Header("GENERAL")]
    [SerializeField] private BurnMode burnMode;

    [SerializeField] private Renderer meshRenderer;

    [SerializeField] private Collider[] burnableColliders;

    [SerializeField] private HeatSensor sensor;
    [SerializeField] private float heatThreshold;

    [Header("BURNS")]
    [SerializeField] private Material[] decalMaterials;
    [SerializeField] private int maxMarkCount;
    [SerializeField] private Vector3 markMinSize, markMaxSize;
    [SerializeField] private float markAppearDelay;
    [SerializeField] private float burningTime, emissionCooldownTime, healingSpeed;

    [Header("EXTRAs")]
    [SerializeField] private GameObject fireVFXPrefab;

    private int currentMarkCount;
    private float delayTimer;

    private List<DecalProjector> projectors = new List<DecalProjector>();

    private void Update()
    {
        if (sensor.currentHeatSource != null && sensor.currentHeat > heatThreshold)
        {
            if (currentMarkCount < maxMarkCount)
            {
                if (delayTimer > markAppearDelay)
                {
                    Transform heatSource = sensor.currentHeatSource.transform;
                    Bounds meshBounds = meshRenderer.bounds;

                    //float centerDistance = Vector3.Distance(heatSource.position, meshBounds.center);

                    // Calculate the top, center and bottom positions of the mesh
                    Vector3 center = meshBounds.center;
                    Vector3 bottom = center - (Vector3.up * meshBounds.extents.y);
                    Vector3 top = center + (Vector3.up * meshBounds.extents.y);

                    // Calculate the directions between the points and the heat source
                    Vector3 bottomDirection = (bottom - heatSource.position).normalized;
                    Vector3 centerDirection = (center - heatSource.position).normalized;
                    Vector3 topDirection = (top - heatSource.position).normalized;

                    Vector3 randomDirection;

                    // With each iteration, the direction will be either between bottom-center or center-top
                    switch (currentMarkCount % 2)
                    {
                        case 0:
                            randomDirection = Vector3Extensions.Random(bottomDirection, centerDirection);
                            break;

                        case 1:
                            randomDirection = Vector3Extensions.Random(centerDirection, topDirection);
                            break;

                        default:
                            randomDirection = centerDirection;
                            break;
                    }

                    RaycastHit[] hits = Physics.RaycastAll(heatSource.position, randomDirection, 10f);

                    foreach (var hit in hits)
                    {
                        if (hit.transform.root == transform.root)
                        {
                            foreach (var collider in burnableColliders)
                            {
                                if (collider == hit.collider)
                                {
                                    delayTimer = 0f;
                                    currentMarkCount++;

                                    StartCoroutine(CreateMark(hit.point, randomDirection, burningTime, hit.transform));

                                    Debug.Log("Hit Collider Name: " + hit.transform.name);

                                    break;
                                }

                                else
                                {
                                    Debug.Log("You fucked up my face");
                                }
                            }

                            break;
                        }

                        else
                        {
                            Debug.Log("Not hit a proper collider. Name: " + hit.transform.name);
                        }
                    }                    
                }
                else
                {
                    delayTimer += Time.deltaTime;
                }
            }  
        }
        else
        {
            

            if (projectors.Count > 0)
            {
                foreach (var projector in projectors)
                {
                    StartCoroutine(EmissionCooldown(projector));
                }

                projectors.Clear();
            }
        }
    }

    private IEnumerator CreateMark(Vector3 position, Vector3 orientation, float formingDuration, Transform parent = null)
    {
        float lerp = 0f;

        GameObject mark = new GameObject("BurnMark");
        DecalProjector projector = mark.AddComponent<DecalProjector>();

        mark.transform.position = position;
        mark.transform.rotation = Quaternion.LookRotation(orientation);
        mark.transform.parent = parent;

        projector.size = Vector3Extensions.Random(markMinSize, markMaxSize);
        projector.pivot = new Vector3(0f, 0f, projector.size.z / 2f);

        projector.material = decalMaterials.GetRandom();
        projector.material.SetFloat("_EmissionStrength", 10f);

        projector.renderingLayerMask = 256;
        projector.fadeFactor = 0f;

        projectors.Add(projector);

        if (fireVFXPrefab != null)
        {
            GameObject fire = Instantiate(fireVFXPrefab);

            fire.transform.position = position;
            fire.transform.rotation = Quaternion.LookRotation(-orientation);
            fire.transform.parent = mark.transform;
        }

        while (lerp < 1f)
        {
            lerp += Time.deltaTime / formingDuration;

            projector.fadeFactor = Mathf.Lerp(0f, 1f, lerp);

            yield return null;
        }
    }

    private IEnumerator EmissionCooldown(DecalProjector projector)
    {
        float lerp = 0f;

        Material material = projector.material;

        float initalEmission = material.GetFloat("_EmissionStrength");

        while (lerp < 1f)
        {
            lerp += Time.deltaTime / emissionCooldownTime;
            material.SetFloat("_EmissionStrength", Mathf.Lerp(initalEmission, 0f, lerp));

            yield return null;
        }
    }
}