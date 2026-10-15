using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target & Positioning")]
    public Transform target;                             // Drag your character here
    public Vector3 offset = new Vector3(0f, 2.5f, -4f);  // Centered behind (X=0) and slightly above (Y=2.5, Z=-4)

    [Header("Follow Settings")]
    public float smoothSpeed = 10f;                      // Camera follow smoothness
    public float lookAtHeight = 1.5f;                    // Aim camera at character's head/chest

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate position directly behind the character based on character's rotation
        Vector3 targetPosition = target.position + target.rotation * offset;

        // Smoothly interpolate camera position
        transform.position = Vector3.Lerp(transform.position, targetPosition, 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime));

        // Look directly at the center of the character
        Vector3 lookTarget = target.position + Vector3.up * lookAtHeight;
        transform.LookAt(lookTarget);
    }
}