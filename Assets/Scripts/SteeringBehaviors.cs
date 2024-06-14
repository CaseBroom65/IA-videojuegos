using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    public enum GuardState
    {
        Normal,
        Alert,
        Attack
    };

    public GuardState CurrentState = GuardState.Normal;

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

    public float MaxSpeed = 20.0f;
    public float Force = 10.0f;
    public float AlertDuration = 5.0f; // Duration for which the guard stays in alert state
    public float AttackVisionTime = 1.0f; // Total time the infiltrator needs to be in vision for attack state

    public Rigidbody rb;
    public AgentSenses Senses;
    public Rigidbody EnemyRigidbody;
    public float ToleranceRadius = 1.0f;
    public float ObstacleAvoidanceRadius = 5.0f; // Radius for obstacle avoidance

    private Vector3 initialPosition;
    private Vector3 lastSeenPosition;
    private Vector3 MouseWorldPos = Vector3.zero;
    private Vector3 WanderTargetPosition = Vector3.zero;

    private float alertTimer = 0.0f;
    private float visionTimer = 0.0f;

    void Awake()
    {
        Init();
        EnemyRigidbody = GameObject.Find("Infiltrator").GetComponent<Rigidbody>();
    }

    void Start()
    {
        initialPosition = transform.position;
    }

    protected void Init()
    {
        rb = GetComponent<Rigidbody>();
        Senses = GetComponent<AgentSenses>();
    }

    void Update()
    {
        MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseWorldPos.z = transform.position.z;

        bool MouseIsInRange = Senses.TargetIsInRange(MouseWorldPos);

        if (CurrentState == GuardState.Alert || CurrentState == GuardState.Normal)
        {
            bool targetInVision = Senses.TargetIsInVisionCone(EnemyRigidbody.position);
            if (targetInVision)
            {
                lastSeenPosition = EnemyRigidbody.position;
                if (CurrentState == GuardState.Alert)
                {
                    visionTimer += Time.deltaTime;
                    if (visionTimer >= AttackVisionTime)
                    {
                        CurrentState = GuardState.Attack;
                        visionTimer = 0.0f;
                    }
                }
                else
                {
                    CurrentState = GuardState.Alert;
                    alertTimer = 0.0f;
                }
            }
            else if (CurrentState == GuardState.Alert)
            {
                alertTimer += Time.deltaTime;
                if (alertTimer >= AlertDuration)
                {
                    CurrentState = GuardState.Normal;
                    alertTimer = 0.0f;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (EnemyRigidbody == null)
            return;

        Vector3 currentSteeringForce = Vector3.zero;

        switch (CurrentBehavior)
        {
            case SteeringBehaviorType.Seek:
                currentSteeringForce = Seek(EnemyRigidbody.position);
                break;
            case SteeringBehaviorType.Flee:
                currentSteeringForce = Flee(EnemyRigidbody.position);
                break;
            case SteeringBehaviorType.Pursuit:
                currentSteeringForce = Pursuit(EnemyRigidbody.position, EnemyRigidbody.velocity);
                break;
            case SteeringBehaviorType.Evade:
                currentSteeringForce = Evade(EnemyRigidbody.position, EnemyRigidbody.velocity);
                break;
            case SteeringBehaviorType.SeekTheMouse:
                currentSteeringForce = Seek(MouseWorldPos);
                break;
            case SteeringBehaviorType.Arrive:
                currentSteeringForce = Arrive(MouseWorldPos, 5.0f);
                break;
            case SteeringBehaviorType.Wander:
                currentSteeringForce = WanderNaive();
                break;
        }

        currentSteeringForce += SemiObstacleAvoidance();

        currentSteeringForce = Vector3.ClampMagnitude(currentSteeringForce, Force);
        rb.AddForce(currentSteeringForce, ForceMode.Acceleration);

        if (CurrentState == GuardState.Alert)
        {
            if (!InsideToleranceRadius(lastSeenPosition))
            {
                CurrentBehavior = SteeringBehaviorType.Seek;
            }
            else
            {
                CurrentState = GuardState.Normal;
                CurrentBehavior = SteeringBehaviorType.Wander;
            }
        }
        else if (CurrentState == GuardState.Normal)
        {
            if (!InsideToleranceRadius(initialPosition))
            {
                CurrentBehavior = SteeringBehaviorType.Seek;
            }
            else
            {
                CurrentBehavior = SteeringBehaviorType.Wander;
            }
        }
        else if (CurrentState == GuardState.Attack)
        {
            // Implement attack behavior here
        }
    }

    Vector3 SemiObstacleAvoidance()
    {
        Vector3 avoidanceForce = Vector3.zero;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ObstacleAvoidanceRadius);

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Obstacle"))
            {
                Vector3 obstaclePosition = col.transform.position;
                Vector3 avoidanceDirection = transform.position - obstaclePosition;
                avoidanceDirection.Normalize();

                float distanceToObstacle = avoidanceDirection.magnitude;
                float fleeRadius = 2.0f; // Radius for activating Flee behavior

                if (distanceToObstacle < fleeRadius)
                {
                    avoidanceForce += avoidanceDirection * MaxSpeed;
                }
            }
        }

        return avoidanceForce;
    }

    protected bool InsideToleranceRadius(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < ToleranceRadius)
        {
            rb.velocity = Vector3.zero;
            return true;
        }
        return false;
    }

    protected Vector3 WanderNaive()
    {
        if (InsideToleranceRadius(WanderTargetPosition))
        {
            float x = Random.Range(-1.0f, 1.0f);
            float z = Random.Range(-1.0f, 1.0f);
            Vector3 randomDirection = new Vector3(x, 0.0f, z).normalized;
            WanderTargetPosition = transform.position + (randomDirection * 15.0f);
        }
        return Arrive(WanderTargetPosition, 5.0f);
    }

    protected Vector3 Arrive(Vector3 targetPosition, float SlowDownRadius)
    {
        Vector3 desiredDirection = targetPosition - transform.position;
        Vector3 desiredDirectionNormalized = desiredDirection.normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (InsideToleranceRadius(targetPosition))
        {
            return Vector3.zero;
        }
        Vector3 desiredVelocity = desiredDirectionNormalized * MaxSpeed;
        if (distance < SlowDownRadius)
        {
            desiredVelocity *= distance / SlowDownRadius;
        }
        Vector3 steeringForce = desiredVelocity - rb.velocity;
        return steeringForce;
    }

    protected Vector3 Evade(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        return -Pursuit(targetPosition, targetCurrentVelocity);
    }

    protected Vector3 Pursuit(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        float timeToReachTargetPosition = (targetPosition - transform.position).magnitude / MaxSpeed;
        Vector3 predictedTargetPosition = targetPosition + targetCurrentVelocity * timeToReachTargetPosition;
        return Seek(predictedTargetPosition);
    }

    protected Vector3 Flee(Vector3 targetPosition)
    {
        return -Seek(targetPosition);
    }

    protected Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 desiredDirection = targetPosition - transform.position;
        Vector3 desiredDirectionNormalized = desiredDirection.normalized;
        Vector3 steeringForce = desiredDirectionNormalized * MaxSpeed;
        return steeringForce - rb.velocity;
    }

    void OnDrawGizmos()
    {
        if (EnemyRigidbody != null)
        {
            float timeToReachTargetPosition = (EnemyRigidbody.position - transform.position).magnitude / MaxSpeed;
            Vector3 predictedTargetPosition = EnemyRigidbody.position + EnemyRigidbody.velocity * timeToReachTargetPosition;

            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawSphere(predictedTargetPosition, 1.0f);
        }

        if (rb != null)
        {
            Gizmos.DrawLine(transform.position, rb.velocity * 10000);
        }

        Gizmos.color = UnityEngine.Color.green;
        Gizmos.DrawSphere(WanderTargetPosition, 1.0f);

        DrawObstacleAvoidanceGizmos();
    }

    void DrawObstacleAvoidanceGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireSphere(transform.position, ObstacleAvoidanceRadius);
    }
}
