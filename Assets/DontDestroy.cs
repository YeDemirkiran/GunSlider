using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy Instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
            return;
        }
    }
}
