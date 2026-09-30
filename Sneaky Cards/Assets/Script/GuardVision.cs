using UnityEngine;

public class GuardVision : MonoBehaviour
{
    public Transform player;

    [Header("Vision Settings")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;

    [Header("Detection")]
    public float detection = 0f;
    public float detectionSpeed = 30f;
    public float detectionDecay = 20f;
    public float detectionThreshold = 100f;

    [Header("Eye Position")]
    public float eyeHeight = 1.5f;

    private bool playerCaught = false;

    private void Update()
    {
        bool canSeePlayer = CheckForPlayer();

        if (canSeePlayer)
        {
            detection += detectionSpeed * Time.deltaTime;
        }
        else
        {
            detection -= detectionDecay * Time.deltaTime;
        }

        detection = Mathf.Clamp(
            detection,
            0f,
            detectionThreshold
        );

        Debug.Log("Detection: " + detection);

        if (detection >= detectionThreshold && !playerCaught)
        {
            playerCaught = true;
            Debug.Log("PLAYER CAUGHT!");
        }
    }

    private bool CheckForPlayer()
    {
        if (player == null)
            return false;

        Vector3 eyePosition =
            transform.position + Vector3.up * eyeHeight;

        Vector3 playerPosition =
            player.position + Vector3.up * 1f;

        Vector3 directionToPlayer =
            playerPosition - eyePosition;

        float distanceToPlayer =
            directionToPlayer.magnitude;

        if (distanceToPlayer > viewDistance)
            return false;

        float angleToPlayer = Vector3.Angle(
            transform.forward,
            directionToPlayer
        );

        if (angleToPlayer > viewAngle / 2f)
            return false;

        if (Physics.Raycast(
            eyePosition,
            directionToPlayer.normalized,
            out RaycastHit hit,
            distanceToPlayer))
        {
            if (hit.transform == player ||
                hit.transform.IsChildOf(player))
            {
                return true;
            }
        }

        return false;
    }
}