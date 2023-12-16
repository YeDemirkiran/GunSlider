using UnityEngine;

public static class DynamicAim
{
    public static void ChooseTarget(ref TargetAim aimTarget, BodyHitpoints targetHitpoints)
    {
        Hitpoint target = targetHitpoints.hitpoints[Random.Range(0, targetHitpoints.hitpoints.Length)];
        aimTarget.target = target;
    }

    public static bool CheckForAimObstacles(TargetAim aimTarget, out RaycastHit hit, LayerMask aimMask)
    {
        Vector3 direction = (aimTarget.target.transform.position - aimTarget.transform.position).normalized;

        hit = default;

        if (Physics.Raycast(aimTarget.transform.position, direction, out RaycastHit rayHit, Mathf.Infinity, aimMask))
        {
            //Debug.Log("Hit Transform Name: " + hit.collider.transform.name);      

            if (rayHit.collider == aimTarget.target.collider || rayHit.collider.CompareTag("BodyCollider"))
            {
                //Debug.Log("Target directly hit");
                hit = rayHit;
                return true;
            }
            else
            {
                //Debug.Log("Something between the target");
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
