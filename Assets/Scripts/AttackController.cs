using System.Collections;  // Necesario para IEnumerator y WaitForSeconds
using System.Collections.Generic;  // No se utiliza en este script pero está presente por defecto
using UnityEngine;  // Necesario para trabajar con elementos de Unity

public class AttackController : MonoBehaviour
{
    public float MaxSpeed = 20.0f;  // Velocidad máxima del guardia en ataque
    public float Force = 10.0f;  // Fuerza aplicada al guardia para moverse
    public float AttackDuration = 5.0f;  // Duración del ataque antes de volver a la posición inicial
    private float attackTimer = 0.0f;  // Temporizador del ataque

    public Rigidbody rb;  // Rigidbody del guardia
    public Rigidbody EnemyRigidbody;  // Rigidbody del enemigo/infiltrador

    void FixedUpdate()
    {
        // Calcula la fuerza de dirección hacia el infiltrador
        Vector3 steeringForce = Seek(EnemyRigidbody.position);
        steeringForce = Vector3.ClampMagnitude(steeringForce, Force);  // Limita la fuerza de dirección
        rb.AddForce(steeringForce, ForceMode.Acceleration);  // Aplica la fuerza al guardia

        attackTimer += Time.deltaTime;  // Incrementa el temporizador del ataque
        if (InsideToleranceRadius(EnemyRigidbody.position))
        {
            // Si el infiltrador está dentro del radio de tolerancia, inicia la destrucción del infiltrador
            StartCoroutine(DestroyInfiltratorAfterDelay(2.0f));
        }
        else if (attackTimer >= AttackDuration)
        {
            // Si el ataque dura más que AttackDuration, vuelve a la posición inicial
            ReturnToInitialPosition();
        }
    }

    // Método Seek para calcular la fuerza de dirección hacia la posición objetivo
    private Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 desiredDirection = targetPosition - transform.position;  // Dirección deseada hacia el objetivo
        desiredDirection.Normalize();  // Normaliza la dirección
        return desiredDirection * MaxSpeed - rb.velocity;  // Calcula la fuerza de dirección
    }

    // Comprueba si el objetivo está dentro del radio de tolerancia
    private bool InsideToleranceRadius(Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition) < 1.0f;
    }

    // Coroutine para destruir al infiltrador después de un retraso
    IEnumerator DestroyInfiltratorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Espera por el tiempo especificado
        if (InsideToleranceRadius(EnemyRigidbody.position))
        {
            // Supone que el infiltrador tiene un método para manejar su destrucción
            // infiltrator.DestroyInfiltrator();
        }
    }

    // Método para volver a la posición inicial (lógica no implementada)
    private void ReturnToInitialPosition()
    {
        // Implementar la lógica para volver a la posición inicial
    }
}
