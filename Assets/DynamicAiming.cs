using UnityEngine;

public class DynamicAiming : MonoBehaviour
{
    [SerializeField] private AimTarget aim;
    [SerializeField] private BodyHitpoints targetHitpoints;
    [SerializeField] private LayerMask aimMask;

    private Hitpoint currentTarget;

    float timer = 1f;

    // Update is called once per frame
    void Update()
    {
        if (timer >= 1f)
        {
            timer = 0f;

            currentTarget = targetHitpoints.hitpoints[Random.Range(0, targetHitpoints.hitpoints.Length)];
            aim.target = currentTarget.transform;


            Debug.Log("Current Target:" + currentTarget.name);

            Vector3 direction = (aim.target.position - aim.transform.position).normalized;
            
            if (Physics.Raycast(aim.transform.position, direction, out RaycastHit hit, Mathf.Infinity, aimMask))
            {
                Debug.Log("Hit Transform Name: " + hit.collider.transform.name);

                if (hit.collider == currentTarget.collider)
                {
                    Debug.Log("Target directly hit");
                }
                else if (!hit.collider.CompareTag("BodyCollider"))
                {
                    Debug.Log("Something between the target");
                }
            }
            else
            {
                Debug.Log("3");
            }
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
