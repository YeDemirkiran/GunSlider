using UnityEngine;

[AddComponentMenu("Heat System/Heat Source", 0)]
public class HeatSource : MonoBehaviour
{
    [SerializeField] private new SphereCollider collider;
    public float radius { get {  return collider.radius; } }

    [Header("HEAT")]
    public float coreHeat;
    public AnimationCurve heatCurve;
}