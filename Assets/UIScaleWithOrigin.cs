using UnityEngine;

public class UIScaleWithOrigin : MonoBehaviour
{
    [SerializeField] private Axis2D axis;
    [SerializeField] private float origin, originMaxDifference;
    [SerializeField] private Vector2 maxScale, minScale;

    private new RectTransform transform;

    // Start is called before the first frame update
    void Awake()
    {
        transform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
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

        transform.localScale = Vector2.Lerp(minScale, maxScale, 1f - originDifference);
    }
}
