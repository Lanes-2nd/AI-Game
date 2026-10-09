using UnityEngine;

public class GuardPatrol : MonoBehaviour
{
    //To create ai behavior, guard have different states
    public enum GuardState
    {
        Patrol,
        Investigate,
        Search,
        Chase
    }

    [Header("References")]
    public Transform[] patrolPoints;
    public GuardVision guardVision;
    public Transform player;
    public WallDetection wallDetection;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float investigateSpeed = 2.5f;
    public float chaseSpeed = 4f;
    public float rotationSpeed = 5f;
    public float arrivalDistance = 0.2f;
    public float patrolWaitTime = 2f;

    [Header("Investigation")]
    [Range(0f, 1f)]
    public float investigateThreshold = 0.5f;
    public float searchDuration = 4f;

    public GuardState currentState = GuardState.Patrol;

    private int currentPoint = 0;
    private float waitTimer = 0f;
    private float searchTimer = 0f;
    private Vector3 lastKnownPosition;
    private bool hasLastKnownPosition = false;
    private bool wasSeeingPlayer = false;

    private void Update()
    {
        if (guardVision == null || player == null)
            return;

        bool canSeePlayer = guardVision.CanSeePlayer();

        //If player is seen, save the last known location
        if (canSeePlayer)
        {
            lastKnownPosition = player.position;
            hasLastKnownPosition = true;
        }

        // Chase only when detection reaches 100%
        if (guardVision.detection >= guardVision.detectionThreshold)
        {
            if (canSeePlayer)
            {
                currentState = GuardState.Chase;
            }
            else if (currentState == GuardState.Chase)
            {
                // If the player escapes sight, investigate their last location
                currentState = GuardState.Investigate;
            }
        }
        else
        {
            // If the guard sees the player again before 100%, keep tracking
            // their last known location but investigate once sight is lost.
            if (canSeePlayer && currentState == GuardState.Patrol)
            {
                // Continue patrol while detection builds up
            }
            else if (!canSeePlayer &&
                     guardVision.detection >=
                     guardVision.detectionThreshold * investigateThreshold &&
                     hasLastKnownPosition &&
                     (currentState == GuardState.Patrol ||
                      currentState == GuardState.Chase))
            {
                currentState = GuardState.Investigate;
            }
        }

        wasSeeingPlayer = canSeePlayer;

        switch (currentState)
        {
            case GuardState.Patrol:
                Patrol();
                break;

            case GuardState.Investigate:
                Investigate();
                break;

            case GuardState.Search:
                Search();
                break;

            case GuardState.Chase:
                Chase();
                break;
        }
    }

    //Patrol between given points
    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        Transform target = patrolPoints[currentPoint];

        if (target == null)
            return;

        if (MoveTowards(target.position, patrolSpeed))
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= patrolWaitTime)
            {
                currentPoint = (currentPoint + 1) % patrolPoints.Length;
                waitTimer = 0f;
            }
        }
        else
        {
            waitTimer = 0f;
        }
    }

    //Investigate the player's last known location
    private void Investigate()
    {
        if (!hasLastKnownPosition)
        {
            currentState = GuardState.Patrol;
            return;
        }

        if (MoveTowards(lastKnownPosition, investigateSpeed))
        {
            currentState = GuardState.Search;
            searchTimer = 0f;
        }
    }

    //Search for a few seconds and return to patrol if the player isn't found
    private void Search()
    {
        searchTimer += Time.deltaTime;

        if (searchTimer >= searchDuration)
        {
            currentState = GuardState.Patrol;
            hasLastKnownPosition = false;
            waitTimer = 0f;
        }
    }

    //If the player is seen again at full detection, chase them
    private void Chase()
    {
        MoveTowards(player.position, chaseSpeed);
    }

    private bool MoveTowards(Vector3 destination, float speed)
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();

            // Check for walls and adjust the movement direction
            if (wallDetection != null)
            {
                Vector3 adjustedDirection =
                    wallDetection.AvoidObstacle(direction);

                if (adjustedDirection.sqrMagnitude < 0.001f)
                    return false;

                direction = adjustedDirection;
            }

            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            Vector3 movement = direction * speed * Time.deltaTime;

            transform.position += movement;
        }

        Vector3 flatDestination = new Vector3(
            destination.x,
            transform.position.y,
            destination.z
        );

        return Vector3.Distance(
            transform.position,
            flatDestination
        ) <= arrivalDistance;
    }
}