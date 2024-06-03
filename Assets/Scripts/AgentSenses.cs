using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script es para darle (simular) a un agente de IA sistemas sensoriales.
// Por ejemplo, la vista o el o�do.
public class AgentSenses : MonoBehaviour
{
    // Queremos que nuestro agente de IA tenga un rango de visi�n.
    public float VisionRange = 10.0f; // Rango m�ximo de visi�n del agente en unidades del juego.

    // �ngulo de visi�n del agente
    public float VisionAngle = 60.0f; // �ngulo total del cono de visi�n del agente en grados.

    // Poner las cosas de inter�s seteadas de antemano.
    // en este caso, eso podr�a ser poner una referencia del objeto que nos interesa en el editor.
    public Transform InfiltratorTransform = null; // Transform del objetivo que queremos detectar.

    // M�todo de pit�goras para c�lculo de la hipotenusa de un tri�ngulo rect�ngulo.
    // �sto lo har� con un vector3.
    // Nos regresa un solo valor, que es la magnitud de un vector.
    float Magnitude(Vector3 in_Vector)
    {
        float sqrX = in_Vector.x * in_Vector.x; // Cuadrado del componente x.
        float sqrY = in_Vector.y * in_Vector.y; // Cuadrado del componente y.
        float sqrZ = in_Vector.z * in_Vector.z; // Cuadrado del componente z.
        return Mathf.Sqrt(sqrX + sqrY + sqrZ); // Ra�z cuadrada de la suma de los cuadrados de los componentes, devuelve la magnitud del vector.
    }

    // Una funci�n que nos regresa un vector que es la diferencia entre el vector Destino menos el vector Origen.
    Vector3 VectorDiff(Vector3 destination, Vector3 origin)
    {
        return new Vector3(destination.x - origin.x, destination.y - origin.y, destination.z - origin.z); // Resta componente a componente.
    }

    // Update is called once per frame
    void Update()
    {
        // Comprueba si el objetivo est� dentro del rango y del cono de visi�n.
        if (TargetIsInRange(InfiltratorTransform.position) && TargetIsInVisionCone(InfiltratorTransform.position))
        {
            Debug.Log("Lo veo"); // Si est� dentro del rango y del cono, imprime "Lo veo".
        }
        else
        {
            Debug.Log("No lo veo"); // Si no, imprime "No lo veo".
        }
    }

    // Funci�n para comprobar si el objetivo est� dentro del rango de visi�n.
    public bool TargetIsInRange(Vector3 targetPosition)
    {
        Vector3 distVector = VectorDiff(targetPosition, transform.position); // Calcula el vector de diferencia.
        float distMagnitude = Magnitude(distVector); // Calcula la magnitud del vector de diferencia.
        return distMagnitude <= VisionRange; // Devuelve true si la magnitud es menor o igual al rango de visi�n.
    }

    // Funci�n para comprobar si el objetivo est� dentro del cono de visi�n.
    public bool TargetIsInVisionCone(Vector3 targetPosition)
    {
        Vector3 directionToTarget = VectorDiff(targetPosition, transform.position).normalized; // Calcula y normaliza la direcci�n hacia el objetivo.
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget); // Calcula el �ngulo entre la direcci�n del agente y la direcci�n hacia el objetivo.
        return angleToTarget <= VisionAngle / 2.0f; // Devuelve true si el �ngulo est� dentro de la mitad del �ngulo de visi�n (cono).
    }

    // Gizmo
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Color del gizmo por defecto.
        Gizmos.DrawLine(transform.position, InfiltratorTransform.position); // Dibuja una l�nea desde el agente hasta el objetivo.

        // Si el objetivo est� dentro del rango y del cono, cambia el color a rojo.
        if (TargetIsInRange(InfiltratorTransform.position) && TargetIsInVisionCone(InfiltratorTransform.position))
        {
            Gizmos.color = Color.red; // Cambia el color del gizmo a rojo.
        }
        else
        {
            Gizmos.color = Color.green; // Mantiene el color del gizmo en verde.
        }

        // Dibuja el cono de visi�n.
        Vector3 leftBoundary = Quaternion.Euler(0, -VisionAngle / 2, 0) * transform.forward * VisionRange; // Calcula el l�mite izquierdo del cono de visi�n.
        Vector3 rightBoundary = Quaternion.Euler(0, VisionAngle / 2, 0) * transform.forward * VisionRange; // Calcula el l�mite derecho del cono de visi�n.

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary); // Dibuja la l�nea del l�mite izquierdo.
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary); // Dibuja la l�nea del l�mite derecho.

        // Opcional: dibujar una l�nea indicando la direcci�n hacia adelante.
        Gizmos.color = Color.blue; // Cambia el color del gizmo a azul.
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * VisionRange); // Dibuja una l�nea hacia adelante indicando la direcci�n de visi�n.
    }
}
