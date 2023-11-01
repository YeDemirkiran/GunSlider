using UnityEngine;

public class CollisionData : MonoBehaviour
{
    public Debris[] debrises;
}

[System.Serializable]
public class Debris
{
    public GameObject[] debrisParticles;
}