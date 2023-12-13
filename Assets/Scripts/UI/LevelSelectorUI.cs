using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelSelectorUI : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Button startButton;
    [SerializeField] float buttonMoveDuration = 0.25f;

    [Header("Button Creation")]
    [SerializeField] float buttonSpacing = 150f;
    [SerializeField] GameObject levelButtonPrefab, linePrefab;

    List<LevelButton> levelButtons = new List<LevelButton>();

    int currentSelectedLevel;

    bool canMove = true;

    // Start is called before the first frame update
    void OnEnable()
    {
        currentSelectedLevel = GameManager.Instance.currentLevel;

        GameObject centerButton = null;

        GameObject previousButton = null;
        RectTransform previousRect = null;

        canMove = true;

        // Center and right buttons
        for (int i = 0; i < 4; i++)
        {
            GameObject button = Instantiate(levelButtonPrefab);
            button.transform.SetParent(transform, false);

            // Initialize the button
            LevelButton levelButton = button.GetComponent<LevelButton>();
            levelButtons.Add(levelButton);

            UnityAction action = () => 
            { 
                //Debug.Log($"Button {levelButton.levelNumber} is selected.");

                StartCoroutine(MoveToButton(levelButton.levelNumber));

                currentSelectedLevel = levelButton.levelNumber;

                startButton.onClick.RemoveAllListeners();
                //startButton.onClick.AddListener(() => Debug.Log($"Level {levelButton.levelNumber} will be loaded."));
            };


            levelButton.Init(GameManager.Instance.currentLevel + i, action);

            // Position the button
            if (i == 0)
            {             
                button.transform.localPosition = Vector3.zero + (new Vector3(buttonSpacing, 0f, 0f) * i);

                centerButton = button;             
            }
            else
            {
                RectTransform rect = button.GetComponent<RectTransform>();

                Vector2 currentButtonScale = button.GetComponent<UIScaleWithOrigin>().CurrentScale();
                Vector2 previousButtonScale = previousButton.GetComponent<UIScaleWithOrigin>().CurrentScale();

                float xPos = (rect.rect.width * currentButtonScale.x / 2f) + (previousRect.rect.width * previousButtonScale.x / 2f) + buttonSpacing;
                button.transform.localPosition = Vector3.zero + (new Vector3(xPos, 0f, 0f) * i);
            }

            // Prepare for the next iteration
            previousButton = button;
            previousRect = previousButton.GetComponent<RectTransform>();
        }

        previousButton = centerButton;
        previousRect = centerButton.GetComponent<RectTransform>();

        // Left buttons
        if (GameManager.Instance.currentLevel > 3)
        {
            // Instantiate buttons on the left
            for (int i = -1; i >= -3; i--)
            {
                GameObject button = Instantiate(levelButtonPrefab);
                button.transform.SetParent(transform, false);

                // Initialize the button
                LevelButton levelButton = button.GetComponent<LevelButton>();

                levelButtons.Add(levelButton);

                UnityAction action = () =>
                {
                    //Debug.Log($"Button {levelButton.levelNumber} created");

                    StartCoroutine(MoveToButton(levelButton.levelNumber));

                    currentSelectedLevel = levelButton.levelNumber;

                    startButton.onClick.RemoveAllListeners();
                    //startButton.onClick.AddListener(() => Debug.Log($"Level {levelButton.levelNumber} will be loaded."));
                };

                levelButton.Init(GameManager.Instance.currentLevel + i, action);

                RectTransform rect = button.GetComponent<RectTransform>();
                    
                Vector2 currentButtonScale = button.GetComponent<UIScaleWithOrigin>().CurrentScale();
                Vector2 previousButtonScale = previousButton.GetComponent<UIScaleWithOrigin>().CurrentScale();

                float xPos = (rect.rect.width * currentButtonScale.x / 2f) + (previousRect.rect.width * previousButtonScale.x / 2f) + buttonSpacing;
                button.transform.localPosition = Vector3.zero + (new Vector3(xPos, 0f, 0f) * i);

                previousButton = button;
                previousRect = previousButton.GetComponent<RectTransform>();
            }
        }

        //Debug.Log("Current number of Level Buttons: " + levelButtons.Count);

        // Sort the Level Buttons list

        levelButtons = levelButtons.OrderBy(x => x.levelNumber).ToList();
    }

    void OnDisable()
    {
        for (int i = levelButtons.Count - 1; i >= 0; i--)
        {
            Destroy(levelButtons[i].gameObject);
        }

        levelButtons.Clear();
    }

    IEnumerator MoveToButton(int targetButtonNumber)
    {       
        int levelOffset = currentSelectedLevel - targetButtonNumber;

        // No need to move if the current button is clicked again

        if (levelOffset == 0 || !canMove)
        {
            yield break;
        }

        canMove = false;

        // Initialization
        float lerp = 0f;
        int levelOffsetSign = System.Math.Sign(levelOffset);
        List<Vector3> startingPositions = new List<Vector3>();
        List<Vector3> targetPositions = new List<Vector3>();

        // Instantiate new buttons first
        for (int i = 0; i < Mathf.Abs(levelOffset); i++)
        {
            LevelButton lastButton;
            LevelButton lastButton2;

            if (levelOffsetSign < 0)
            {
                lastButton = levelButtons[levelButtons.Count - 1];
                lastButton2 = levelButtons[levelButtons.Count - 2];
            }
            else
            {
                lastButton = levelButtons[0];
                lastButton2 = levelButtons[1];
            }

            InstantiateButton(lastButton.levelNumber - levelOffsetSign, lastButton.transform.localPosition + (CalculateButtonPositionOffset(lastButton, lastButton2) * -levelOffsetSign));
        }


        // Iterate through all the buttons and set starting and target positions for lerping
        int a = 0;
        foreach (var button in levelButtons)
        {
            Vector3 startingPosition;

            startingPosition = button.transform.localPosition;
            startingPositions.Add(startingPosition);

            //Debug.Log("a: " + (a));
            //Debug.Log("a with offset: " + (a + levelOffset));

            int b = a + levelOffset;

            Vector3 targetPosition;

            if (b < 0)
            {
                targetPosition = startingPosition - Vector3.right * 300f;
            }
            else if (b > levelButtons.Count - 1)
            {
                targetPosition = startingPosition + Vector3.right * 300f;
            }
            else
            {
                targetPosition = levelButtons[b].transform.localPosition;
            }

            targetPositions.Add(targetPosition);

            //Debug.Log("Starting position: " + startingPosition);
            //Debug.Log("Target position: " + targetPosition);

            a++;
        }

        // Begin movement
        while (lerp < 1f)
        {
            lerp += Time.deltaTime / buttonMoveDuration;

            int i = 0;

            foreach (var button in levelButtons)
            {
                //Debug.Log("CURRENT BUTTON INDEX: " + i);
                button.transform.localPosition = Vector3.Lerp(startingPositions[i], targetPositions[i], lerp);
                i++;
            }

            yield return null;
        }

        // Remove the out-of-screen buttons
        if (levelButtons.Count > 7)
        {
            //Debug.Log("1. Level Offset: " + levelOffset);

            for (int i = 0; i < Mathf.Abs(levelOffset); i++)
            {
                //Debug.Log("1.0");

                if (levelOffsetSign < 0)
                {
                    //Debug.Log("1.1");

                    Destroy(levelButtons[0].gameObject);
                    levelButtons.RemoveAt(0);
                }
                else
                {
                    //Debug.Log("1.2");

                    int last = levelButtons.Count - 1;

                    Destroy(levelButtons[last].gameObject);
                    levelButtons.RemoveAt(last);
                }
            }
        }
        else
        {
            //Debug.Log("2");
        }

        canMove = true;
    }

    void InstantiateButton(int levelNumber, Vector3 localPosition)
    {
        GameObject button = Instantiate(levelButtonPrefab);
        button.transform.SetParent(transform, false);

        LevelButton levelButton = button.GetComponent<LevelButton>();

        UnityAction action = () =>
        {
            //Debug.Log($"Button {levelButton.levelNumber} is selected.");

            StartCoroutine(MoveToButton(levelButton.levelNumber));

            currentSelectedLevel = levelButton.levelNumber;

            startButton.onClick.RemoveAllListeners();
            //startButton.onClick.AddListener(() => Debug.Log($"Level {levelButton.levelNumber} will be loaded."));
        };

        levelButton.Init(levelNumber, action);

        button.transform.localPosition = localPosition;

        levelButtons.Add(levelButton);
        levelButtons = levelButtons.OrderBy(x => x.levelNumber).ToList();
    }

    Vector3 CalculateButtonPositionOffset(LevelButton button, LevelButton previousButton)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        RectTransform previousRect = previousButton.GetComponent<RectTransform>();

        Vector2 currentButtonScale = button.GetComponent<UIScaleWithOrigin>().CurrentScale();
        Vector2 previousButtonScale = previousButton.GetComponent<UIScaleWithOrigin>().CurrentScale();

        float xPos = (rect.rect.width * currentButtonScale.x / 2f) + (previousRect.rect.width * previousButtonScale.x / 2f) + buttonSpacing;
        return new Vector3(xPos, 0f, 0f);
    }
}