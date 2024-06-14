using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitTarget : MonoBehaviour
{
    public float arriveRadius = 1.0f; // Radio para considerar que ha llegado al objetivo
    public float Force = 10.0f; // Fuerza del movimiento
    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position; // Inicialmente el objetivo es la posición actual
    }

    void Update()
    {
        if (isMoving)
        {
            Vector3 direction = targetPosition - transform.position;
            if (direction.magnitude < arriveRadius)
            {
                isMoving = false;
                rb.velocity = Vector3.zero; // Detener el movimiento
            }
            else
            {
                Vector3 arriveVector = Arrive(targetPosition);
                rb.AddForce(arriveVector * Force, ForceMode.Acceleration);
            }
        }
    }

    void OnMouseDown()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10.0f; // Ajuste de profundidad para la cámara
        targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        isMoving = true; // Comenzar a moverse hacia el objetivo
    }

    Vector3 Arrive(Vector3 target)
    {
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, 1.0f);
    }
}
