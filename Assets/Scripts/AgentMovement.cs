using System.Collections;
using UnityEngine;

public class Guard : MonoBehaviour
{
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
        infiltrator = GameObject.FindWithTag("Infiltrator")?.transform;
        if (infiltrator == null)
        {
            Debug.LogError("Infiltrator no encontrado. Asegúrate de que el infiltrador tenga la etiqueta 'Infiltrator'.");
        }
        else
        {
            StartCoroutine(RotateGuard());
        }
    }

    void Update()
    {
        if (infiltrator != null && CanSeeTarget(infiltrator))
        {
            isAlert = true;
            // Aquí puedes agregar comportamiento adicional para el estado de alerta
            Debug.Log("Guardia en estado de alerta!");
        }
    }

    IEnumerator RotateGuard()
    {
        while (true)
        {
            yield return new WaitForSeconds(rotationInterval);
            if (!isAlert)
            {
                transform.Rotate(Vector3.up, rotationAngle);
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * viewDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * viewDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);

        // Dibujar un arco para representar el cono de visión
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, viewDistance);
    }

    public bool CanSeeTarget(Transform target)
    {
        if (target == null) return false;

        Vector3 dirToTarget = (target.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
        {
            float dstToTarget = Vector3.Distance(transform.position, target.position);
            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
            {
                return true;
            }
        }
        return false;
    }
}
