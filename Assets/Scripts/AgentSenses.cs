using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script es para darle (simular) a un agente de IA sistemas sensoriales.
// Por ejemplo, la vista o el oído.
public class AgentSenses : MonoBehaviour
{
    // Queremos que nuestro agente de IA tenga un rango de visión.
    public float VisionRange = 10.0f; // Rango máximo de visión del agente en unidades del juego.

    // Ángulo de visión del agente
    public float VisionAngle = 60.0f; // Ángulo total del cono de visión del agente en grados.

    // Poner las cosas de interés seteadas de antemano.
    // en este caso, eso podría ser poner una referencia del objeto que nos interesa en el editor.
    public Transform InfiltratorTransform = null; // Transform del objetivo que queremos detectar.

    // Método de pitágoras para cálculo de la hipotenusa de un triángulo rectángulo.
    // Ésto lo hará con un vector3.
    // Nos regresa un solo valor, que es la magnitud de un vector.
    float Magnitude(Vector3 in_Vector)
    {
        float sqrX = in_Vector.x * in_Vector.x; // Cuadrado del componente x.
        float sqrY = in_Vector.y * in_Vector.y; // Cuadrado del componente y.
        float sqrZ = in_Vector.z * in_Vector.z; // Cuadrado del componente z.
        return Mathf.Sqrt(sqrX + sqrY + sqrZ); // Raíz cuadrada de la suma de los cuadrados de los componentes, devuelve la magnitud del vector.
    }

    // Una función que nos regresa un vector que es la diferencia entre el vector Destino menos el vector Origen.
    Vector3 VectorDiff(Vector3 destination, Vector3 origin)
    {
        return new Vector3(destination.x - origin.x, destination.y - origin.y, destination.z - origin.z); // Resta componente a componente.
    }

    // Update is called once per frame
    void Update()
    {
        // Comprueba si el objetivo está dentro del rango y del cono de visión.
        if (TargetIsInRange(InfiltratorTransform.position) && TargetIsInVisionCone(InfiltratorTransform.position))
        {
            Debug.Log("Lo veo"); // Si está dentro del rango y del cono, imprime "Lo veo".
        }
        else
        {
            Debug.Log("No lo veo"); // Si no, imprime "No lo veo".
        }
    }

    // Función para comprobar si el objetivo está dentro del rango de visión.
    public bool TargetIsInRange(Vector3 targetPosition)
    {
        Vector3 distVector = VectorDiff(targetPosition, transform.position); // Calcula el vector de diferencia.
        float distMagnitude = Magnitude(distVector); // Calcula la magnitud del vector de diferencia.
        return distMagnitude <= VisionRange; // Devuelve true si la magnitud es menor o igual al rango de visión.
    }

    // Función para comprobar si el objetivo está dentro del cono de visión.
    public bool TargetIsInVisionCone(Vector3 targetPosition)
    {
        Vector3 directionToTarget = VectorDiff(targetPosition, transform.position).normalized; // Calcula y normaliza la dirección hacia el objetivo.
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget); // Calcula el ángulo entre la dirección del agente y la dirección hacia el objetivo.
        return angleToTarget <= VisionAngle / 2.0f; // Devuelve true si el ángulo está dentro de la mitad del ángulo de visión (cono).
    }

    // Gizmo
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Color del gizmo por defecto.
        Gizmos.DrawLine(transform.position, InfiltratorTransform.position); // Dibuja una línea desde el agente hasta el objetivo.

        // Si el objetivo está dentro del rango y del cono, cambia el color a rojo.
        if (TargetIsInRange(InfiltratorTransform.position) && TargetIsInVisionCone(InfiltratorTransform.position))
        {
            Gizmos.color = Color.red; // Cambia el color del gizmo a rojo.
        }
        else
        {
            Gizmos.color = Color.green; // Mantiene el color del gizmo en verde.
        }

        // Dibuja el cono de visión.
        Vector3 leftBoundary = Quaternion.Euler(0, -VisionAngle / 2, 0) * transform.forward * VisionRange; // Calcula el límite izquierdo del cono de visión.
        Vector3 rightBoundary = Quaternion.Euler(0, VisionAngle / 2, 0) * transform.forward * VisionRange; // Calcula el límite derecho del cono de visión.

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary); // Dibuja la línea del límite izquierdo.
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary); // Dibuja la línea del límite derecho.

        // Opcional: dibujar una línea indicando la dirección hacia adelante.
        Gizmos.color = Color.blue; // Cambia el color del gizmo a azul.
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * VisionRange); // Dibuja una línea hacia adelante indicando la dirección de visión.
    }
}
