using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    // Forward es la dirección hacie el frente del GameObject que sea dueño de este script.
    public Vector3 MovementDirection = Vector3.forward;

    public Vector3 GravityForce = Vector3.up * -9.81f;

    public Vector3 Velocity = Vector3.zero;


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
        // CUADRO 1 DE ACTUALIZACIÓN

        // qué cambio causa el tener una aceleración?
        // la aceleración incrementa la velocidad. Cuánto la incrementa? 
        // pues la magnitud de esa aceleración cada segundo.
        Velocity += GravityForce * Time.deltaTime;

        // si deltatime = 16ms
        // 0, 0, 0 += (0, -9.8, 0) * 0.016s;
        // 0, 0, 0, += (0, -.1568, 0)

        //Ahí ya aplicamos aceleración a la velocidad.
        // ahora toca aplicar la velocidad a la posición.
        transform.position += Velocity * Time.deltaTime;

        // si delta time = 16ms = 0.016s
        // (0, 0, 0) += (0, -.1568, 0) * 0.016s;
        // (0, -0.0025088, 0)

        // ACÁ HAREMOS CUADRO 2 DE ACTUALIZACIÓN

        // aplicar aceleración a la velocidad
        // si deltatime = 0.025s = 25ms
        // 0, -.1568, 0 += (0, -9.8, 0) * 0.025s;
        // 0, -.1568, 0, += (0, -.245, 0)
        // 0, -.4018, 0  VELOCIDAD AL FINAL DEL CUADRO 2

        // aplicamos velocidad a la posición:
        // (0, -0.0025088, 0) += (0, -.4018, 0) * .025s
        // (0, -0.0025088, 0) += (0, -.01, 0) 


        // Qué dice esta línea de código?
        // transform.position es la posición del GameObject dueño de este script.
        // súmale a la posición de ese gameobject la dirección de movimiento.
        // transform.position += MovementDirection * Time.deltaTime;

        // considerar velocidad

        // considerar aceleración.
        // VelocidadActual = aceleraciónDeGravedad*Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        MovementDirection = MovementDirection.normalized;
        Gizmos.DrawLine(transform.position, transform.position + MovementDirection * 10000f);
    }
}
