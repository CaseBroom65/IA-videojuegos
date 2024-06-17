using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    // Enumeración de los estados del guardia
    public enum GuardState
    {
        Normal,  // Estado normal (patrulla)
        Alert,   // Estado de alerta
        Attack   // Estado de ataque
    };

    public GuardState CurrentState = GuardState.Normal;  // Estado actual del guardia, inicializado a Normal

    private PatrolController patrolBehavior;  // Comportamiento de patrulla
    private AlertController alertBehavior;    // Comportamiento de alerta
    private AttackController attackBehavior;  // Comportamiento de ataque

    void Awake()
    {
        // Obtiene las referencias a los componentes de comportamiento
        patrolBehavior = GetComponent<PatrolController>();
        alertBehavior = GetComponent<AlertController>();
        attackBehavior = GetComponent<AttackController>();
    }

    void Update()
    {
        // Gestiona los comportamientos del guardia según su estado actual
        switch (CurrentState)
        {
            case GuardState.Normal:
                EnableBehavior(patrolBehavior);   // Activa el comportamiento de patrulla
                DisableBehavior(alertBehavior);   // Desactiva el comportamiento de alerta
                DisableBehavior(attackBehavior);  // Desactiva el comportamiento de ataque
                break;
            case GuardState.Alert:
                EnableBehavior(alertBehavior);    // Activa el comportamiento de alerta
                DisableBehavior(patrolBehavior);  // Desactiva el comportamiento de patrulla
                DisableBehavior(attackBehavior);  // Desactiva el comportamiento de ataque
                break;
            case GuardState.Attack:
                EnableBehavior(attackBehavior);   // Activa el comportamiento de ataque
                DisableBehavior(patrolBehavior);  // Desactiva el comportamiento de patrulla
                DisableBehavior(alertBehavior);   // Desactiva el comportamiento de alerta
                break;
        }

        // Comprueba si el objetivo está en el cono de visión del guardia
        if (alertBehavior.Senses.TargetIsInVisionCone(alertBehavior.EnemyRigidbody.position))
        {
            CurrentState = GuardState.Alert;  // Cambia el estado a alerta si el objetivo está en visión
            alertBehavior.ResetAlertTimer();  // Resetea el temporizador de alerta
        }
    }

    // Método para activar un comportamiento
    private void EnableBehavior(MonoBehaviour behavior)
    {
        if (behavior != null)
            behavior.enabled = true;
    }

    // Método para desactivar un comportamiento
    private void DisableBehavior(MonoBehaviour behavior)
    {
        if (behavior != null)
            behavior.enabled = false;
    }
}
