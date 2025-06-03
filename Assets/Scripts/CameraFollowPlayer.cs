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
