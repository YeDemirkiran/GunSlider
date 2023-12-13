using UnityEngine;

public class UIScaleWithOrigin : MonoBehaviour
{
    [SerializeField] private Axis2D axis;
    [SerializeField] private float origin, originMaxDifference;
    [SerializeField] private Vector2 maxScale, minScale;

    new RectTransform transform;

    void Awake()
    {
        transform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = CurrentScale();
    }

    public Vector2 CurrentScale()
    {
        float originDifference = 0f;

        switch (axis)
        {
            case Axis2D.x:
                originDifference = origin - transform.localPosition.x;
                break;

            case Axis2D.y:
                originDifference = origin - transform.localPosition.y;
                break;
        }

        originDifference = Mathf.Abs(originDifference) / originMaxDifference;

        return Vector2.Lerp(minScale, maxScale, 1f - originDifference);
    }
}
