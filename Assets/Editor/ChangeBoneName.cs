using UnityEngine;
using UnityEditor;

static class ChangeBoneName
{
    [MenuItem("Answers/Apply Prefix to Children")]
    public static void ApplyPrefix()
    {
        if (!EditorUtility.DisplayDialog("Child Prefix Warning",
            "You are about to rename children of your current selection. Undo is not possible. Are you sure you want to continue?",
            "Yes, I understand the risk", "No, I changed my mind")) return;

        GameObject[] gos = Selection.gameObjects;
        foreach (GameObject go in gos)
        {
            var children = go.GetComponentsInChildren(typeof(Transform));
            foreach (Transform child in children)
            {
                // Don't apply to root object.
                if (child == go.transform)
                    continue;

                child.name = child.name.Replace("9", "");
            }
        }
    }
}