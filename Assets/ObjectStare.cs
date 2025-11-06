using UnityEngine;
using System.Collections.Generic;

public class ObjectStare : MonoBehaviour
{
    public Transform target; // The target GameObject to look at
    public List<Transform> affectedObjects; // List of GameObjects to be affected

    void Update()
    {
        // Check if the target is set and if the game objects are active
        if (target != null && affectedObjects.Count > 0)
        {
            // Get the direction from the affected objects to the target
            Vector3 direction = target.position - transform.position;

            // Get the angle on the Z-axis between the two positions
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Loop through each affected object and make them look at the target on the Z-axis
            foreach (Transform affectedObject in affectedObjects)
            {
                if (affectedObject != null && affectedObject.gameObject.activeInHierarchy)
                {
                    affectedObject.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
        }
    }
}
