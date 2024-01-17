using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProducerSiegeTowerDestinationPointData : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Distancia de medición")]
    private float distance;

    private Dictionary<string, float> aux;
    private List<string> _pointsToMove;
    private Dictionary<string, float> distance3Spots;



    private void Start()
    {
        aux = new Dictionary<string, float>();
        distance3Spots = new Dictionary<string, float>();
        CalculateSpotsToMove();
        _pointsToMove = distance3Spots.Keys.ToList();
        Consumer.SharedInstance.AddDataSiegeTowerDestinationPointData(gameObject.name, _pointsToMove);

    }

    private void CalculateSpotsToMove()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance);
        foreach (Collider colliderMeasured in colliders)
        {
            if(colliderMeasured.gameObject.tag == "D1" || colliderMeasured.gameObject.tag == "D3")
                Nearest3Spots(colliderMeasured, Vector3.Distance(transform.position, colliderMeasured.transform.position));
        }

        foreach (var pair in aux)
        {
            _pointsToMove.Add(pair.Key);
        }
    }

    //Comprueba si un spot está más cerca que los 3 spots más cercanos hasta el momento
    //del spot actual
    private void Nearest3Spots(Collider collider, float distance)
    {
        if (distance3Spots.Count < 3)
        {
            distance3Spots.Add(collider.name, distance);
        }
        else
        {
            string furthestCollider = GetFurthestDistance();
            if (distance < distance3Spots[furthestCollider])
            {
                distance3Spots.Remove(furthestCollider);
                distance3Spots.Add(collider.name, distance);
            }
        }
    }

    private string GetFurthestDistance()
    {
        float furthestDistance = 0.0f;
        string furthestCollider = "";
        foreach (var collider in aux)
        {
            if (collider.Value > furthestDistance)
            {
                furthestCollider = collider.Key;
                furthestDistance = collider.Value;
            }
        }
        return furthestCollider;
    }


    private void OnDrawGizmosSelected()
    {

    }
}
