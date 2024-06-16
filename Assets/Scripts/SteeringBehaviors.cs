using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    //citios Steering Behaviors Tutorial from Unity Learn: https://www.youtube.com/watch?app=desktop&v=tIfC00BE6z8
    //Steering Behaviors in Game Design:  https://webdocs.cs.ualberta.ca/~games/299/Gamasutra.pdf
    //Steering Behaviors in Unity with the Behavior Designer https://www.youtube.com/watch?v=BXrsPpHOxtM


    // Enum para los estados de la guardia
    public enum GuardState
    {
        Normal,
        Alert,
        Attack
    };

    public GuardState CurrentState = GuardState.Normal;

    // Enum para los tipos de comportamiento de steering
    public enum SteeringBehaviorType
    {
        Seek = 0,
        Flee,
        Pursuit,
        Evade,
        SeekTheMouse,
        Arrive,
        Wander,
        MAX
    };

    public SteeringBehaviorType CurrentBehavior = SteeringBehaviorType.Seek;

    public float MaxSpeed = 20.0f; // Velocidad máxima del agente
    public float Force = 10.0f; // Fuerza máxima de steering
    public float AlertDuration = 5.0f; // Duración del estado de alerta
    public float AttackDuration = 5.0f; // Duración del estado de ataque
    public float AttackVisionTime = 1.0f; // Tiempo necesario para cambiar a estado de ataque

    public Rigidbody rb; // Componente Rigidbody del agente
    public AgentSenses Senses; // Componente para detectar al infiltrador
    public Rigidbody EnemyRigidbody; // Rigidbody del enemigo
    public float ToleranceRadius = 1.0f; // Radio de tolerancia para llegar a un objetivo
    public float ObstacleAvoidanceRadius = 5.0f; // Radio para evitar obstáculos

    private Vector3 initialPosition; // Posición inicial del agente
    private Vector3 lastSeenPosition; // Última posición vista del enemigo
    private Vector3 MouseWorldPos = Vector3.zero; // Posición del ratón en el mundo
    private Vector3 WanderTargetPosition = Vector3.zero; // Objetivo actual para el comportamiento wander

    private float alertTimer = 0.0f; // Temporizador para el estado de alerta
    private float visionTimer = 0.0f; // Temporizador para el tiempo necesario para cambiar a estado de ataque
    private float attackTimer = 0.0f; // Temporizador para la duración del estado de ataque

    private PursuitTarget infiltrator; // Referencia al infiltrador
    private bool infiltratorDestroyed = false; // Estado del infiltrador

    void Awake()
    {
        Init(); // Inicializa componentes y referencias
        infiltrator = GameObject.Find("Infiltrator").GetComponent<PursuitTarget>(); // Encuentra al infiltrador
        if (infiltrator != null)
        {
            EnemyRigidbody = infiltrator.GetComponent<Rigidbody>(); // Obtiene el Rigidbody del infiltrador
        }
    }

    void Start()
    {
        initialPosition = transform.position; // Guarda la posición inicial del agente
    }

    // Inicializa componentes y referencias
    protected void Init()
    {
        rb = GetComponent<Rigidbody>(); // Obtiene el componente Rigidbody
        Senses = GetComponent<AgentSenses>(); // Obtiene el componente AgentSenses
    }

    void Update()
    {
        // Permite reaparecer al infiltrador con la tecla R si está destruido
        if (infiltratorDestroyed)
        {
            if (Input.GetKeyDown(KeyCode.R)) // Tecla R para reaparecer al infiltrador
            {
                ReappearInfiltrator(new Vector3(10, 0, 10)); // Posición de reaparición del infiltrador
            }
        }

        MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Convierte la posición del ratón a coordenadas del mundo
        MouseWorldPos.z = transform.position.z; // Ajusta la posición z del ratón

        if (CurrentState == GuardState.Alert || CurrentState == GuardState.Normal)
        {
            if (infiltrator != null && infiltrator.gameObject.activeSelf)
            {
                // Verifica si el infiltrador está en el cono de visión
                bool targetInVision = Senses.TargetIsInVisionCone(EnemyRigidbody.position);
                if (targetInVision)
                {
                    lastSeenPosition = EnemyRigidbody.position; // Actualiza la última posición vista del enemigo
                    if (CurrentState == GuardState.Alert)
                    {
                        visionTimer += Time.deltaTime; // Incrementa el temporizador de visión
                        if (visionTimer >= AttackVisionTime)
                        {
                            CurrentState = GuardState.Attack; // Cambia al estado de ataque
                            attackTimer = 0.0f; // Reinicia el temporizador de ataque
                        }
                    }
                    else
                    {
                        CurrentState = GuardState.Alert; // Cambia al estado de alerta
                        alertTimer = 0.0f; // Reinicia el temporizador de alerta
                    }
                }
                else if (CurrentState == GuardState.Alert)
                {
                    alertTimer += Time.deltaTime; // Incrementa el temporizador de alerta
                    if (alertTimer >= AlertDuration)
                    {
                        CurrentState = GuardState.Normal; // Cambia al estado normal
                        alertTimer = 0.0f; // Reinicia el temporizador de alerta
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (infiltratorDestroyed)
            return;

        Vector3 currentSteeringForce = Vector3.zero;

        // Determina el comportamiento actual del agente
        switch (CurrentBehavior)
        {
            case SteeringBehaviorType.Seek:
                currentSteeringForce = Seek(EnemyRigidbody.position); // Comportamiento Seek hacia la posición del enemigo
                break;
            case SteeringBehaviorType.Flee:
                currentSteeringForce = Flee(EnemyRigidbody.position); // Comportamiento Flee desde la posición del enemigo
                break;
            case SteeringBehaviorType.Pursuit:
                currentSteeringForce = Pursuit(EnemyRigidbody.position, EnemyRigidbody.velocity); // Comportamiento Pursuit hacia el enemigo
                break;
            case SteeringBehaviorType.Evade:
                currentSteeringForce = Evade(EnemyRigidbody.position, EnemyRigidbody.velocity); // Comportamiento Evade desde el enemigo
                break;
            case SteeringBehaviorType.SeekTheMouse:
                currentSteeringForce = Seek(MouseWorldPos); // Comportamiento Seek hacia la posición del ratón
                break;
            case SteeringBehaviorType.Arrive:
                currentSteeringForce = Arrive(MouseWorldPos, 5.0f); // Comportamiento Arrive hacia la posición del ratón
                break;
            case SteeringBehaviorType.Wander:
                currentSteeringForce = WanderNaive(); // Comportamiento Wander
                break;
        }

        // Agrega la fuerza de evitación de obstáculos
        currentSteeringForce += SemiObstacleAvoidance();

        // Limita la fuerza de steering y la aplica al rigidbody
        currentSteeringForce = Vector3.ClampMagnitude(currentSteeringForce, Force);
        rb.AddForce(currentSteeringForce, ForceMode.Acceleration);

        // Comportamiento según el estado de la guardia
        if (CurrentState == GuardState.Alert)
        {
            if (!InsideToleranceRadius(lastSeenPosition))
            {
                CurrentBehavior = SteeringBehaviorType.Seek; // Cambia a Seek si está fuera del radio de tolerancia
            }
            else
            {
                CurrentState = GuardState.Normal; // Cambia al estado normal
                CurrentBehavior = SteeringBehaviorType.Wander; // Cambia a Wander
            }
        }
        else if (CurrentState == GuardState.Normal)
        {
            if (!InsideToleranceRadius(initialPosition))
            {
                CurrentBehavior = SteeringBehaviorType.Seek; // Cambia a Seek si está fuera del radio de tolerancia
            }
            else
            {
                CurrentBehavior = SteeringBehaviorType.Wander; // Cambia a Wander
            }
        }
        else if (CurrentState == GuardState.Attack)
        {
            attackTimer += Time.deltaTime; // Incrementa el temporizador de ataque
            if (InsideToleranceRadius(EnemyRigidbody.position))
            {
                StartCoroutine(DestroyInfiltratorAfterDelay(2.0f)); // Destruye el infiltrador después de un retraso
            }
            else if (attackTimer >= AttackDuration)
            {
                ReturnToInitialPosition(); // Regresa a la posición inicial después de la duración del ataque
            }
        }
    }

    // Comportamiento de evitación de obstáculos
    protected Vector3 SemiObstacleAvoidance()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ObstacleAvoidanceRadius);
        Vector3 avoidanceForce = Vector3.zero;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject && hitCollider.tag == "Obstacle")
            {
                avoidanceForce += Flee(hitCollider.transform.position); // Evita el obstáculo huyendo de él
            }
        }

        return avoidanceForce;
    }

    // Comportamiento Arrive
    protected Vector3 Arrive(Vector3 targetPosition, float slowDownRadius = 5.0f)
    {
        Vector3 desiredDirection = targetPosition - transform.position;
        float distance = desiredDirection.magnitude;
        desiredDirection.Normalize();
        float speed = MaxSpeed * (distance / slowDownRadius); // Ajusta la velocidad en función de la distancia al objetivo
        Vector3 desiredVelocity = desiredDirection * speed;
        Vector3 steeringForce = desiredVelocity - rb.velocity;
        return steeringForce;
    }

    // Comportamiento Pursuit
    protected Vector3 Pursuit(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        float timeToReachTargetPosition = (targetPosition - transform.position).magnitude / MaxSpeed;
        Vector3 predictedTargetPosition = targetPosition + targetCurrentVelocity * timeToReachTargetPosition; // Predice la posición futura del objetivo
        return Seek(predictedTargetPosition);
    }

    // Comportamiento Evade
    protected Vector3 Evade(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        return -Pursuit(targetPosition, targetCurrentVelocity); // Invierte el comportamiento Pursuit
    }

    // Comportamiento Flee
    protected Vector3 Flee(Vector3 targetPosition)
    {
        return -Seek(targetPosition); // Invierte el comportamiento Seek
    }

    // Comportamiento Seek
    protected Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 desiredDirection = targetPosition - transform.position;
        desiredDirection.Normalize();
        Vector3 desiredVelocity = desiredDirection * MaxSpeed;
        Vector3 steeringForce = desiredVelocity - rb.velocity;
        return steeringForce;
    }

    // Corrutina para destruir el infiltrador después de un retraso
    IEnumerator DestroyInfiltratorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (InsideToleranceRadius(EnemyRigidbody.position))
        {
            infiltrator.DestroyInfiltrator(); // Llama al método para destruir el infiltrador
            infiltratorDestroyed = true;
            ReturnToInitialPosition(); // Regresa a la posición inicial
        }
    }

    // Reaparece el infiltrador en una posición específica
    void ReappearInfiltrator(Vector3 position)
    {
        infiltrator.ReappearInfiltrator(position); // Llama al método para reaparecer el infiltrador
        EnemyRigidbody = infiltrator.GetComponent<Rigidbody>();
        infiltratorDestroyed = false;
    }

    // Regresa la guardia a su posición inicial
    void ReturnToInitialPosition()
    {
        CurrentBehavior = SteeringBehaviorType.Seek; // Cambia a Seek
        if (InsideToleranceRadius(initialPosition))
        {
            CurrentState = GuardState.Normal; // Cambia al estado normal
            CurrentBehavior = SteeringBehaviorType.Wander; // Cambia a Wander
        }
    }

    // Verifica si el agente está dentro del radio de tolerancia del objetivo
    protected bool InsideToleranceRadius(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < ToleranceRadius)
        {
            rb.velocity = Vector3.zero; // Detiene el agente
            return true;
        }
        return false;
    }

    // Comportamiento Wander
    protected Vector3 WanderNaive()
    {
        if (InsideToleranceRadius(WanderTargetPosition))
        {
            float x = Random.Range(-1.0f, 1.0f);
            float z = Random.Range(-1.0f, 1.0f);
            Vector3 randomDirection = new Vector3(x, 0.0f, z).normalized; // Genera una dirección aleatoria
            WanderTargetPosition = transform.position + (randomDirection * 15.0f); // Calcula una nueva posición objetivo para wander
        }
        return Arrive(WanderTargetPosition, 5.0f); // Llama al comportamiento Arrive para la posición objetivo de wander
    }

    // Dibuja gizmos para depuración
    void OnDrawGizmos()
    {
        if (EnemyRigidbody != null)
        {
            float timeToReachTargetPosition = (EnemyRigidbody.position - transform.position).magnitude / MaxSpeed;
            Vector3 predictedTargetPosition = EnemyRigidbody.position + EnemyRigidbody.velocity * timeToReachTargetPosition;

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(predictedTargetPosition, 1.0f); // Dibuja una esfera en la posición predicha del objetivo
        }

        if (rb != null)
        {
            Gizmos.DrawLine(transform.position, rb.velocity * 10000); // Dibuja una línea desde la posición del agente en la dirección de su velocidad
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(WanderTargetPosition, 1.0f); // Dibuja una esfera en la posición objetivo de wander

    }
}
