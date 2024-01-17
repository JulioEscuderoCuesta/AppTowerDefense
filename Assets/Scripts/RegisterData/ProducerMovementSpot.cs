using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProducerMovementSpot : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Distancia de visión")]
    private float movementDistance;

    [SerializeField]
    [Tooltip("Distancia de ataque máxima")]
    private float maxAttackDistance;
    [SerializeField] private List<string> moveForwardTag;
    [SerializeField] private LayerMask attackableSpots;
    [SerializeField] private LayerMask attackableWalls;
    private string leftSpot = "";
    private string rightSpot = "";

    [SerializeField]
    [Tooltip("Si es un punto de instanciación de los enemigos")]
    private bool spawner;

    [SerializeField]
    [Tooltip("Qué muro se puede escalar")]
    private Wall closestWallToClimb;

    [SerializeField]
    [Tooltip("Punto de conexión al siguiente anillo")]
    private MovementSpot jumpPoint;

    [SerializeField] [Tooltip("Puerta hacia la que se puede ir desde este punto")]
    private Door door;

	[SerializeField] [Tooltip("Punto central de la torre más próxima (si tiene)")]
	private TowerSpot towerSpot;

    private List<string> nearest3Spots;
    private Dictionary<string, float> distance3Spots;
    private Dictionary<string, float> attackSpots;
    private Dictionary<string, float> wallsToAttack;
    private Dictionary<string, float> jumpPointAndDistance;
    private string closestWallToClimbString = "";
    private string doorString = "";
	private string towerSpotString = "";
    private int numberOfObjectsWithSameTag;

    public MovementSpot JumpPoint => jumpPoint;



    private void Start()
    {
        nearest3Spots = new List<string>();
        distance3Spots = new Dictionary<string, float>();
        attackSpots = new Dictionary<string, float>();
        wallsToAttack = new Dictionary<string, float>();
        jumpPointAndDistance = new Dictionary<string, float>();
        numberOfObjectsWithSameTag = GameObject.FindGameObjectsWithTag(gameObject.tag).Length;
        CalculateReachableSpots();
        CalculateHorizontalSpots();
        CalculateAttackableSpots();
        CalculateWallsToAttack();
        CalculateclosestWallToClimbString();
        CalculateJumpPoint();
        CalculateDoor();
        CalculateTower();

        Consumer.SharedInstance.AddDataMovementSpot(name, nearest3Spots, rightSpot, leftSpot, attackSpots, closestWallToClimbString, wallsToAttack, spawner, jumpPointAndDistance, doorString, towerSpotString);

    }
    
    //Comprueba si un spot está más cerca que los 3 spots más cercanos hasta el momento
    //del spot actual
    private void Nearest3Spots(Collider collider, float distance)
    {
        Debug.Log($"ñlajfsdñalk: {collider.name}");
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

    //Calcula el spot más cercano de los 3 más cercanos al spot que llama al método
    private string GetFurthestDistance()
    {
        string furthestCollider = "";
        float furthestDistance = 0.0F;
        foreach (var collider in distance3Spots)
        {
            if (collider.Value > furthestDistance)
            {
                furthestDistance = collider.Value;
                furthestCollider = collider.Key;
            }
        }
        return furthestCollider;
    }

    private void CalculateReachableSpots()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, movementDistance);
        Debug.Log($"colliders: {colliders.Length}");
        foreach (Collider colliderMeasured in colliders)
        {
            if(moveForwardTag.Contains(colliderMeasured.tag))
                Nearest3Spots(colliderMeasured, Vector3.Distance(transform.position, colliderMeasured.transform.position));
        }

        foreach (var pair in distance3Spots)
        {
            nearest3Spots.Add(pair.Key);
        }
    }

    private void CalculateHorizontalSpots()
    {
        string[] subString = gameObject.name.Split('_');
        int rightNumber = 0;
        int leftNumber = 0;
        bool success = int.TryParse(subString[1], out int number);
        if (success)
        {
            number++;
            rightNumber = number;
            number -= 2;
            leftNumber = number;
            if (rightNumber > numberOfObjectsWithSameTag)
                rightNumber = 1;
            if (leftNumber == 0)
            {
                leftNumber = numberOfObjectsWithSameTag;
            }
        }
        rightSpot = rightNumber.ToString();
        leftSpot = leftNumber.ToString();

    }



    private void CalculateAttackableSpots()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxAttackDistance, attackableSpots);
        foreach (var collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < maxAttackDistance)
            {
                attackSpots.Add(collider.name, distance);
            }  
        }
    }

    private void CalculateclosestWallToClimbString()
    {
        if (closestWallToClimb != null)
            closestWallToClimbString = closestWallToClimb.name;
        else
            closestWallToClimbString = "Auxiliar Wall";
    }

    private void CalculateWallsToAttack()
    {
        //Collider[] colliders = Physics.OverlapSphere(transform.position, maxAttackDistance, attackableWalls);
        //Debug.Log($"collider:{colliders.Length}");
        /*foreach (var collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            Wall w = collider.GetComponent<Wall>();
            if (w != null)
            {
                if (distance < maxAttackDistance)
                {
                    wallsToAttack.Add(collider.name, distance);
                }  
            }
           
        }*/
        wallsToAttack.Add("pared norte 2", 50f);

    }

    private void CalculateJumpPoint()
    {
        if (jumpPoint == null)
        {
            jumpPointAndDistance.Add("Auxiliar Spot", 0);
        }
        else
        {
            jumpPointAndDistance.Add(jumpPoint.gameObject.name, Vector3.Distance(transform.position, jumpPoint.gameObject.transform.position));
        }
    }

    private void CalculateDoor()
    {
        if (door == null)
            doorString = "Auxiliar Wall";
        else
            doorString = door.name;
    }

	private void CalculateTower()
    {
        if (towerSpot == null)
            towerSpotString = "Auxiliar Spot";
        else
            towerSpotString = towerSpot.name;
    }

    private void OnDrawGizmosSelected()
    {
        //Visión esfera
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxAttackDistance);
        
        //Visión esfera
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, movementDistance);

    }
}
