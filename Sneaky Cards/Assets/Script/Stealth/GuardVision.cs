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
    public float playerAimHeight = 1f;

    private bool playerCaught = false;
    private bool suspiciousMessageShown = false;

    private void Update()
    {
        bool canSeePlayer = CanSeePlayer();

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

        // Show detection value in the Console while testing.
        Debug.Log("Detection: " + detection.ToString("F1"));

        // Show the suspicious message once per detection buildup.
        if (detection >= 30f && !suspiciousMessageShown)
        {
            suspiciousMessageShown = true;
            Debug.Log("GUARD IS SUSPICIOUS!");
        }

        // Allow the suspicious message to appear again
        // after detection has dropped below 30.
        if (detection < 30f)
        {
            suspiciousMessageShown = false;
        }

        // Catch the player once detection reaches its threshold.
        if (detection >= detectionThreshold && !playerCaught)
        {
            playerCaught = true;
            Debug.Log("PLAYER CAUGHT!");
        }
    }

    //Check for player, play could hide behind objects or crouch to dodge vision
    public bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 eyePosition =
            transform.position + Vector3.up * eyeHeight;

        Vector3 playerPosition =
            player.position + Vector3.up * playerAimHeight;

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