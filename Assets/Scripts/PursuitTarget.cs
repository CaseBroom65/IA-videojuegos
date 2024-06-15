using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public List<Transform> waypoints; // List to store waypoint positions
    public float arriveRadius = 1.0f; // Radius for considering waypoint reached
    public float force = 10.0f; // Movement force

    private Rigidbody rb;
    private int currentWaypointIndex = 0; // Index of the current waypoint
    private bool isMoving = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (waypoints.Count > 0 && isMoving) // Check for waypoints and movement state
        {
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            Vector3 direction = targetPosition - transform.position;

            if (direction.magnitude < arriveRadius)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count; // Loop to next waypoint
                if (currentWaypointIndex == 0 && waypoints.Count > 1) // Check for cycle completion
                {
                    // Optional: Implement action for completing the cycle (e.g., animation)
                }
                isMoving = false;
                rb.velocity = Vector3.zero; // Stop on reaching waypoint
            }
            else
            {
                Vector3 arriveVector = Arrive(targetPosition);
                rb.AddForce(arriveVector * force, ForceMode.Acceleration);
            }
        }
    }

    Vector3 Arrive(Vector3 target)
    {
        // Existing Arrive function (no modification needed)
        Vector3 desiredVelocity = target - transform.position;
        float distance = desiredVelocity.magnitude;
        float speed = rb.velocity.magnitude;

        if (distance < arriveRadius)
        {
            speed = Mathf.Lerp(0, speed, distance / arriveRadius);
        }

        desiredVelocity = desiredVelocity.normalized * speed;
        Vector3 steering = desiredVelocity - rb.velocity;
        return steering;
    }

    public void AddWaypoint(Transform waypoint) // Function to add waypoints
    {
        waypoints.Add(waypoint);
    }

    public void RemoveWaypoint(Transform waypoint) // Function to remove waypoints
    {
        waypoints.Remove(waypoint);
    }

    public void StartMoving() // Function to initiate movement along waypoints
    {
        if (waypoints.Count > 0)
        {
            isMoving = true;
        }
    }
}
