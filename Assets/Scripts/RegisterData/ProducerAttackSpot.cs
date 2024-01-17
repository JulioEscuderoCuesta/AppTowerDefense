using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProducerAttackSpot : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Distancia de ataque máxima")]
    private float maxAttackDistance;

    [SerializeField]
    [Tooltip("Punto desde el que se accede a este punto")]
    private MovementSpot connectionPoint;

    [SerializeField] [Tooltip("Conjunto de puntos a los que se puede atacar desde los puntos de ataque")]
    private LayerMask attackableSpots;

    private Dictionary<string, float> attackSpots;



    private void Start()
    {
        attackSpots = new Dictionary<string, float>();
        CalculateAttackableSpots();
        Consumer.SharedInstance.AddDataAttackSpot(gameObject.name, attackSpots, connectionPoint.name);
    }

    private void CalculateAttackableSpots()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxAttackDistance, attackableSpots);
        foreach (var collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < maxAttackDistance)
                attackSpots.Add(collider.name, distance);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color color = Color.blue;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, maxAttackDistance);
    }
}
