using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    // Forward es la dirección hacie el frente del GameObject que sea dueño de este script.
    public Vector3 MovementDirection = Vector3.forward;

    // Start is called before the first frame update
    void Start()
    {
        // normalizamos el vector de MovementDirection para estar 100% seguros de que es una dirección normalizada.
        //
        if (MovementDirection.magnitude == 0.0f)
        {
            // Entonces no podemos normalizarlo. 
            // Aquí imprimiríamos un error y abortaríamos el programa.
            Debug.LogError("ERROR, se trató de normalizar un vector 0");
        }
        else
        {
            // Si su magnitud no es 0, entonces sí se puede normalizar.
            MovementDirection = MovementDirection.normalized;
        }


    }

    // Update is called once per frame
    // cuántas veces se ejecuta la función update cada segundo?
    // Depende.
    // 60 FPS
    // 1000 milisegundos, si queremos 60 FPS, pues dividimos 1000 entre 60.
    // 1000/60 = 16.666... milisegundos.
    void Update()
    {
        // Qué dice esta línea de código?
        // transform.position es la posición del GameObject dueño de este script.
        // súmale a la posición de ese gameobject la dirección de movimiento.
        transform.position += MovementDirection * Time.deltaTime;

        // considerar velocidad

        // considerar aceleración.
        // VelocidadActual = aceleraciónDeGravedad*Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        MovementDirection = MovementDirection.normalized;
        Gizmos.DrawLine(transform.position, transform.position + MovementDirection * 1f);
    }
}


