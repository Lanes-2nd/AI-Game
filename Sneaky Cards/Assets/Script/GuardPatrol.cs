using UnityEngine;

public class GuardPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitTime = 2f;

    private int currentPoint = 0;
    private float waitTimer = 0f;

    private void Update()
    {
        if (patrolPoints.Length == 0)
            return;

        Transform target = patrolPoints[currentPoint];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            patrolSpeed * Time.deltaTime
        );

        Vector3 direction = target.position - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                5f * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                currentPoint++;

                if (currentPoint >= patrolPoints.Length)
                {
                    currentPoint = 0;
                }

                waitTimer = 0f;
            }
        }
    }
}