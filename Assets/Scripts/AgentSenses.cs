using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AgentSenses : MonoBehaviour
{
    // Rango de visi�n del agente
    public float VisionRange = 10.0f;
    // �ngulo de visi�n del agente
    public float VisionAngle = 45.0f;
    // Transform del objetivo a detectar
    public Transform InfiltratorTransform = null;

    // M�todo Update se llama una vez por frame
    void Update()
    {
        // Verifica si el objetivo est� dentro del campo de visi�n
        if (TargetIsInVision(InfiltratorTransform.position))
        {
            // Si el objetivo est� en visi�n, imprime "Lo veo"
            Debug.Log("Lo veo");
        }
        else
        {
            // Si el objetivo no est� en visi�n, imprime "No lo veo"
            Debug.Log("No lo veo");
        }
    }

    // M�todo que verifica si el objetivo est� en el campo de visi�n
    private bool TargetIsInVision(Vector3 targetPosition)
    {
        // Calcula la direcci�n al objetivo
        Vector3 directionToTarget = targetPosition - transform.position;
        // Calcula la distancia al objetivo
        float distanceToTarget = directionToTarget.magnitude;

        // Si la distancia al objetivo es mayor que el rango de visi�n, devuelve false
        if (distanceToTarget > VisionRange)
        {
            return false;
        }

        // Calcula el �ngulo entre la direcci�n hacia adelante y la direcci�n al objetivo
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        // Si el �ngulo es mayor que la mitad del �ngulo de visi�n, devuelve false
        if (angleToTarget > VisionAngle / 2)
        {
            return false;
        }

        // Si el objetivo est� dentro del rango y �ngulo de visi�n, devuelve true
        return true;
    }

    // M�todo OnDrawGizmos se usa para dibujar elementos de depuraci�n en la vista de escena
    void OnDrawGizmos()
    {
        // Si no hay objetivo asignado, no dibuja nada y sale del m�todo
        if (InfiltratorTransform == null) return;

        // Establece el color de los gizmos seg�n si el objetivo est� en el campo de visi�n (rojo) o no (verde)
        Gizmos.color = TargetIsInVision(InfiltratorTransform.position) ? Color.red : Color.green;

        // Calcula la direcci�n hacia adelante multiplicada por el rango de visi�n
        Vector3 forward = transform.forward * VisionRange;
        // Calcula los l�mites izquierdo y derecho del cono de visi�n aplicando rotaciones
        Vector3 leftBoundary = Quaternion.Euler(0, -VisionAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, VisionAngle / 2, 0) * forward;

        // Dibuja l�neas desde la posici�n del agente hacia los l�mites del cono de visi�n
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);

        // Divide el cono de visi�n en segmentos
        int segments = 20;
        float angleStep = VisionAngle / segments;
        for (int i = 0; i < segments; i++)
        {
            // Calcula los puntos de los segmentos y dibuja l�neas entre ellos
            Vector3 from = Quaternion.Euler(0, -VisionAngle / 2 + angleStep * i, 0) * forward;
            Vector3 to = Quaternion.Euler(0, -VisionAngle / 2 + angleStep * (i + 1), 0) * forward;
            Gizmos.DrawLine(transform.position, transform.position + from);
            Gizmos.DrawLine(transform.position + from, transform.position + to);
        }
    }
}

