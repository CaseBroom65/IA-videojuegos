using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AgentSenses : MonoBehaviour
{
    // Rango de visión del agente
    public float VisionRange = 10.0f;
    // Ángulo de visión del agente
    public float VisionAngle = 45.0f;
    // Transform del objetivo a detectar
    public Transform InfiltratorTransform = null;

    // Método Update se llama una vez por frame
    void Update()
    {
        // Verifica si el objetivo está dentro del campo de visión
        if (TargetIsInVision(InfiltratorTransform.position))
        {
            // Si el objetivo está en visión, imprime "Lo veo"
            Debug.Log("Lo veo");
        }
        else
        {
            // Si el objetivo no está en visión, imprime "No lo veo"
            Debug.Log("No lo veo");
        }
    }

    // Método que verifica si el objetivo está en el campo de visión
    private bool TargetIsInVision(Vector3 targetPosition)
    {
        // Calcula la dirección al objetivo
        Vector3 directionToTarget = targetPosition - transform.position;
        // Calcula la distancia al objetivo
        float distanceToTarget = directionToTarget.magnitude;

        // Si la distancia al objetivo es mayor que el rango de visión, devuelve false
        if (distanceToTarget > VisionRange)
        {
            return false;
        }

        // Calcula el ángulo entre la dirección hacia adelante y la dirección al objetivo
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        // Si el ángulo es mayor que la mitad del ángulo de visión, devuelve false
        if (angleToTarget > VisionAngle / 2)
        {
            return false;
        }

        // Si el objetivo está dentro del rango y ángulo de visión, devuelve true
        return true;
    }

    // Método OnDrawGizmos se usa para dibujar elementos de depuración en la vista de escena
    void OnDrawGizmos()
    {
        // Si no hay objetivo asignado, no dibuja nada y sale del método
        if (InfiltratorTransform == null) return;

        // Establece el color de los gizmos según si el objetivo está en el campo de visión (rojo) o no (verde)
        Gizmos.color = TargetIsInVision(InfiltratorTransform.position) ? Color.red : Color.green;

        // Calcula la dirección hacia adelante multiplicada por el rango de visión
        Vector3 forward = transform.forward * VisionRange;
        // Calcula los límites izquierdo y derecho del cono de visión aplicando rotaciones
        Vector3 leftBoundary = Quaternion.Euler(0, -VisionAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, VisionAngle / 2, 0) * forward;

        // Dibuja líneas desde la posición del agente hacia los límites del cono de visión
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);

        // Divide el cono de visión en segmentos
        int segments = 20;
        float angleStep = VisionAngle / segments;
        for (int i = 0; i < segments; i++)
        {
            // Calcula los puntos de los segmentos y dibuja líneas entre ellos
            Vector3 from = Quaternion.Euler(0, -VisionAngle / 2 + angleStep * i, 0) * forward;
            Vector3 to = Quaternion.Euler(0, -VisionAngle / 2 + angleStep * (i + 1), 0) * forward;
            Gizmos.DrawLine(transform.position, transform.position + from);
            Gizmos.DrawLine(transform.position + from, transform.position + to);
        }
    }
}

