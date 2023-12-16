using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelNumber {  get; private set; }
    public new RectTransform transform {  get; private set; }

    Button button;
    TMP_Text buttonText;

    // Start is called before the first frame update
    void Awake()
    {
        transform = GetComponent<RectTransform>();

        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TMP_Text>();
    }

    public void Init(int levelNumber, UnityAction clickAction)
    {
        this.levelNumber = levelNumber;
        buttonText.text = levelNumber.ToString();
        button.onClick.AddListener(clickAction);
    }

    void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}