using UnityEngine;

public class WallDetection : MonoBehaviour
{
    [Header("Obstacle Detection")]
    public float detectionDistance = 1.2f;
    public float turnAngle = 45f;
    public LayerMask obstacleLayers = ~0;

    private GuardPatrol guardPatrol;

    private void Awake()
    {
        guardPatrol = GetComponent<GuardPatrol>();
    }

    public Vector3 AvoidObstacle(Vector3 desiredDirection)
    {
        desiredDirection.y = 0f;

        if (desiredDirection.sqrMagnitude < 0.001f)
            return Vector3.zero;

        desiredDirection.Normalize();

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (!Physics.Raycast(
            origin,
            desiredDirection,
            detectionDistance,
            obstacleLayers))
        {
            return desiredDirection;
        }

        Vector3 leftDirection =
            Quaternion.Euler(0f, -turnAngle, 0f) * desiredDirection;

        Vector3 rightDirection =
            Quaternion.Euler(0f, turnAngle, 0f) * desiredDirection;

        bool leftBlocked = Physics.Raycast(
            origin, leftDirection, detectionDistance, obstacleLayers);

        bool rightBlocked = Physics.Raycast(
            origin, rightDirection, detectionDistance, obstacleLayers);

        if (!leftBlocked)
            return leftDirection;

        if (!rightBlocked)
            return rightDirection;

        return Vector3.zero;
    }
}