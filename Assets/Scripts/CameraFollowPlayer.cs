using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;
    private Vector3 offset = new Vector3(0, 3.5f, -4f);
    private float smoothSpeed = 5f;
    //private Vector3 offset = new Vector3(0, 1.5f, -3f);
    //private float smoothSpeed = 8f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Optional: Look at player
        transform.LookAt(target);
    }
}



///
/// /
/// /
/// /
/// ///
/// 
// using UnityEngine;

// public class FollowPlayer : MonoBehaviour
// {
//     [Tooltip("The Transform of the player (or whatever object) to follow.")]
//     public Transform target;

//     [Tooltip("Local‐space offset from the target: X = right, Y = up, Z = back.")]
//     public Vector3 localOffset = new Vector3(0f, 3.5f, -4f);

//     [Tooltip("How quickly the camera moves to catch up to the desired position (higher = snappier).")]
//     public float smoothSpeed = 5f;

//     void LateUpdate()
//     {
//         if (target == null) return;

//         // 1) Compute the rotated offset: 
//         //    If the player is rotated in world space, this takes (0,3.5,-4) and
//         //    turns it into “3.5 units up, 4 units behind whatever way the player is facing.”
//         Vector3 worldOffset = target.rotation * localOffset;

//         // 2) Desired camera position = player’s world position + that rotated offset
//         Vector3 desiredPosition = target.position + worldOffset;

//         // 3) Smoothly move the camera from its current position toward desiredPosition
//         transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

//         // 4) Make the camera look at the player (you can add a slight upward offset if you want to look at the player’s head)
//         transform.LookAt(target.position + Vector3.up * 1.2f);
//     }
// }
