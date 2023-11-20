using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageTransition : MonoBehaviour
{
    [SerializeField] private MaskableGraphic image;

    [SerializeField] Color defaultColor;

    // Start is called before the first frame update
    void Awake()
    {
        image.color = defaultColor;

        FadeIn(10f, true);
    }

    public void FadeIn(float duration, bool unscaled)
    {
        StopAllCoroutines();

        Color targetColor = image.color;
        targetColor.a = 0f;

        StartCoroutine(FadeCoroutine(targetColor, duration, unscaled));
    }

    public void FadeOut(float duration, bool unscaled)
    {
        StopAllCoroutines();

        Color targetColor = image.color;
        targetColor.a = 1f;

        StartCoroutine(FadeCoroutine(targetColor, duration, unscaled));
    }

    private IEnumerator FadeCoroutine(Color targetColor, float duration, bool unscaled)
    {
        float lerp = 0f;
        Color startingColor = image.color;

        while (lerp < 1f) 
        { 
            lerp += (unscaled ? Time.unscaledDeltaTime : Time.deltaTime) / duration;
            image.color = Color.Lerp(startingColor, targetColor, lerp);

            yield return null;
        }
    }
}