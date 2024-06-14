using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSenses : MonoBehaviour
{
    public float VisionRange = 10.0f;
    public float VisionAngle = 45.0f; // Ángulo de visión en grados
    public Transform InfiltratorTransform = null;

    void Start()
    {
        TestVision();
    }

    void Update()
    {
        CheckVision(InfiltratorTransform.position);
    }

    void TestVision()
    {
        Vector3 testPosition = new Vector3(1, 2, 3);
        bool canSee = IsTargetVisible(testPosition);

        if (canSee)
        {
            Debug.Log("Lo veo");
        }
        else
        {
            Debug.Log("No lo veo");
        }
    }

    bool IsTargetVisible(Vector3 targetPosition)
    {
        if (!TargetIsInRange(targetPosition))
            return false;

        return IsWithinVisionCone(targetPosition);
    }

    public bool TargetIsInRange(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        return distance <= VisionRange;
    }

    bool IsWithinVisionCone(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        return angleToTarget <= VisionAngle / 2;
    }

    public bool TargetIsInVisionCone(Vector3 targetPosition)
    {
        return TargetIsInRange(targetPosition) && IsWithinVisionCone(targetPosition);
    }

    void CheckVision(Vector3 targetPosition)
    {
        if (IsTargetVisible(targetPosition))
        {
            Debug.Log("Lo veo");
        }
        else
        {
            Debug.Log("No lo veo");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = TargetIsInRange(InfiltratorTransform.position) ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, VisionRange);

        Vector3 forward = transform.forward * VisionRange;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-VisionAngle / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(VisionAngle / 2, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftRayDirection);
        Gizmos.DrawRay(transform.position, rightRayDirection);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * VisionRange);
    }
}
