using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSenses : MonoBehaviour
//canal de youtube para un poco de programación de unity https://www.youtube.com/c/SebastianLague
//Se utilizo chat para la busqueda de información.

{
    public float VisionRange = 10.0f; // Rango de visión del agente
    public float VisionAngle = 45.0f; // Ángulo de visión en grados
    public Transform InfiltratorTransform = null; // Referencia al transform del infiltrador

    void Start()
    {
        TestVision(); // Llama a la función para probar la visión al iniciar
    }

    void Update()
    {
        CheckVision(InfiltratorTransform.position); // Verifica la visión del infiltrador en cada frame
    }

    void TestVision()
    {
        Vector3 testPosition = new Vector3(1, 2, 3); // Posición de prueba
        bool canSee = IsTargetVisible(testPosition); // Verifica si puede ver la posición de prueba

        if (canSee)
        {
            Debug.Log("Lo veo"); // Si puede ver la posición de prueba, imprime "Lo veo"
        }
        else
        {
            Debug.Log("No lo veo"); // Si no puede ver la posición de prueba, imprime "No lo veo"
        }
    }

    // Verifica si el objetivo es visible (dentro del rango y ángulo de visión)
    bool IsTargetVisible(Vector3 targetPosition)
    {
        if (!TargetIsInRange(targetPosition))
            return false;

        return IsWithinVisionCone(targetPosition);
    }

    // Verifica si el objetivo está dentro del rango de visión
    public bool TargetIsInRange(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        return distance <= VisionRange;
    }

    // Verifica si el objetivo está dentro del ángulo de visión
    bool IsWithinVisionCone(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        return angleToTarget <= VisionAngle / 2;
    }

    // Verifica si el objetivo está tanto en el rango como en el ángulo de visión
    public bool TargetIsInVisionCone(Vector3 targetPosition)
    {
        return TargetIsInRange(targetPosition) && IsWithinVisionCone(targetPosition);
    }

    // Verifica la visión del objetivo y imprime un mensaje según el resultado
    void CheckVision(Vector3 targetPosition)
    {
        if (IsTargetVisible(targetPosition))
        {
            Debug.Log("Lo veo");
        }
        else
        {
            Debug.Log("No lo veo");
        }
    }

    // Dibuja Gizmos en la escena para visualizar el rango y el ángulo de visión del agente
    void OnDrawGizmos()
    {
        // Dibuja un círculo para representar el rango de visión
        Gizmos.color = TargetIsInRange(InfiltratorTransform.position) ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, VisionRange);

        // Dibuja las líneas del ángulo de visión
        Vector3 forward = transform.forward * VisionRange;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-VisionAngle / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(VisionAngle / 2, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftRayDirection);
        Gizmos.DrawRay(transform.position, rightRayDirection);

        // Dibuja una línea hacia adelante para representar la dirección del agente
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * VisionRange);
    }
}
