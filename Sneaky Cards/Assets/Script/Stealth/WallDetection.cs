using UnityEngine;

public class WallDetection : MonoBehaviour
{
    public float detectionDistance = 2f;
    public float turnAngle = 45f;
    public float rayHeight = 0.5f;
    public float bodyRadius = 0.5f;

    public LayerMask obstacleLayers = ~0;

    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

        if (capsuleCollider != null)
        {
            bodyRadius = capsuleCollider.radius;
        }
    }

    public Vector3 AvoidObstacle(Vector3 desiredDirection)
    {
        desiredDirection.y = 0f;

        if (desiredDirection.sqrMagnitude < 0.001f)
            return Vector3.zero;

        desiredDirection.Normalize();

        Vector3 origin = transform.position;

        Vector3 right = transform.right;

        Vector3[] origins =
        {
            origin + Vector3.up * rayHeight,
            origin + Vector3.up * 1f,
            origin + Vector3.up * 1.5f
        };

        Vector3[] directions =
        {
            desiredDirection,
            Quaternion.Euler(0f, -turnAngle, 0f) * desiredDirection,
            Quaternion.Euler(0f, turnAngle, 0f) * desiredDirection,
            Quaternion.Euler(0f, -turnAngle * 2f, 0f) * desiredDirection,
            Quaternion.Euler(0f, turnAngle * 2f, 0f) * desiredDirection
        };

        bool IsBlocked(Vector3 direction)
        {
            foreach (Vector3 rayOrigin in origins)
            {
                Vector3 sideOffset = right * bodyRadius * 0.7f;

                if (Physics.Raycast(
                    rayOrigin - sideOffset,
                    direction,
                    detectionDistance,
                    obstacleLayers))
                    return true;

                if (Physics.Raycast(
                    rayOrigin + sideOffset,
                    direction,
                    detectionDistance,
                    obstacleLayers))
                    return true;
            }

            return false;
        }

        if (!IsBlocked(desiredDirection))
            return desiredDirection;

        foreach (Vector3 direction in directions)
        {
            if (!IsBlocked(direction))
                return direction;
        }

        return Vector3.zero;
    }
}