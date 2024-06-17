using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolController : MonoBehaviour
{
    public Transform[] Waypoints;  // Array de waypoints para la patrulla
    public float PatrolSpeed = 2.0f;  // Velocidad de patrulla
    private int currentWaypointIndex = 0;  // Índice del waypoint actual

    void Update()
    {
        // Si no hay waypoints, no hace nada
        if (Waypoints.Length == 0)
            return;

        // Movimiento hacia el siguiente waypoint
        Transform targetWaypoint = Waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position += direction * PatrolSpeed * Time.deltaTime;

        // Comprueba si ha llegado al waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            // Cambia al siguiente waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % Waypoints.Length;
        }
    }
}
