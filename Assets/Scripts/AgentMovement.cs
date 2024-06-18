using System.Collections;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    //En el tutorial de Unity "IA básica en Unity" https://learn.unity.com/course/artificial-intelligence-for-beginners
    //Se utilizo chat para la busqueda de información.
    public float viewDistance = 10f; // Distancia del cono de visión
    public float viewAngle = 45f; // Ángulo del cono de visión
    public LayerMask targetMask; // Máscara para los objetivos que el guardia puede ver
    public LayerMask obstacleMask; // Máscara para los obstáculos que pueden bloquear la visión
    public float rotationInterval = 5f; // Intervalo de tiempo entre cada rotación
    public float rotationAngle = 45f; // Ángulo de rotación
    private Transform infiltrator; // Referencia al infiltrador
    private bool isAlert = false; // Estado de alerta del guardia

    void Start()
    {
        // Encuentra el infiltrador en la escena usando su etiqueta
        infiltrator = GameObject.FindWithTag("Infiltrator")?.transform;
        if (infiltrator == null)
        {
            Debug.LogError("Infiltrator no encontrado. Asegúrate de que el infiltrador tenga la etiqueta 'Infiltrator'.");
        }
        else
        {
            // Inicia la rutina de rotación del guardia
            StartCoroutine(RotateGuard());
        }
        
    }

    void Update()
    {
        if (infiltrator != null && CanSeeTarget(infiltrator))
        {
            isAlert = true; // El guardia entra en estado de alerta
            // Aquí puedes agregar comportamiento adicional para el estado de alerta
            Debug.Log("Guardia en estado de alerta!");
        }
    }

    IEnumerator RotateGuard()
    {
        while (true)
        {
            // Espera el intervalo de rotación
            yield return new WaitForSeconds(rotationInterval);
            if (!isAlert)
            {
                // Rota el guardia si no está en estado de alerta
                transform.Rotate(Vector3.up, rotationAngle);
            }
        }
    }

    void OnDrawGizmos()
    {
        // Dibuja los límites del cono de visión
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * viewDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * viewDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);

        // Dibuja un arco para representar el cono de visión
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, viewDistance);
    }

    // Función para verificar si el guardia puede ver al objetivo
    public bool CanSeeTarget(Transform target)
    {
        if (target == null) return false;

        Vector3 dirToTarget = (target.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
        {
            float dstToTarget = Vector3.Distance(transform.position, target.position);
            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
            {
                return true; // El objetivo está dentro del cono de visión y no hay obstáculos
            }
        }
        return false; // El objetivo está fuera del cono de visión o hay obstáculos
    }
}
