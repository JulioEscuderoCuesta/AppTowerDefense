using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProducerTowerSpot : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Distancia de ataque máxima")]
    private List<AttackSpot> attackSpots;

    [SerializeField]
    [Tooltip("Punto desde el que se accede a este punto")]
    private MovementSpot connectionPoint;

    private List<string> attackSpotsString;
    
    private void Start()
    {
        attackSpotsString = new List<string>();
        CalculateAttackSpotsString();
        Consumer.SharedInstance.AddDataTowerSpot(gameObject.name, attackSpotsString, connectionPoint.name);
    }

    private void CalculateAttackSpotsString()
    {
        foreach (var x in attackSpots)
        {
            attackSpotsString.Add(x.gameObject.name);
        }
    }
}