using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController))]
public class VRHeadLeanLocomotion : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float maxLeanDistance = 0.35f;     // Distance at which movement reaches max speed
    public float meanLeanDistance = 0.15f;    // Dead zone radius (no movement before this)

    public Transform headTransform;          // Assign the XR Camera (VR headset)O
    public bool showDebugGizmos = false;

    private CharacterController controller;
    private Vector3 neutralHeadLocalPos;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (headTransform == null)
        {
            Debug.LogError("[VRHeadLeanLocomotion] Please assign the VR camera (headTransform).");
            enabled = false;
            return;
        }

        neutralHeadLocalPos = GetLocalHeadPosition();
    }

    void Update()
    {
        Vector3 localHeadPos = GetLocalHeadPosition();
        Vector3 offset = localHeadPos - neutralHeadLocalPos;
        offset.y = 0; // ignore vertical motion

        float distance = offset.magnitude;

        // Dead zone: ignore small lean movements
        if (distance < meanLeanDistance)
        {
            controller.SimpleMove(Vector3.zero);
            return;
        }

        // Compute lean intensity between dead zone and max
        float normalized = Mathf.InverseLerp(meanLeanDistance, maxLeanDistance, distance);
        normalized = Mathf.Clamp01(normalized);

        Vector3 direction = offset.normalized;
        Vector3 worldDirection = transform.TransformDirection(direction);
        Vector3 move = worldDirection * normalized * moveSpeed;

        controller.SimpleMove(move);
    }

    Vector3 GetLocalHeadPosition()
    {
        return transform.InverseTransformPoint(headTransform.position);
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos || headTransform == null) return;

        Gizmos.color = Color.cyan;
        Vector3 headLocal = GetLocalHeadPosition();
        Vector3 headWorld = transform.TransformPoint(headLocal);
        Gizmos.DrawLine(transform.position, headWorld);
        Gizmos.DrawSphere(headWorld, 0.05f);

        // Draw dead zone + max zone in Scene view
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.7f, meanLeanDistance);
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.7f, maxLeanDistance);
    }
}
