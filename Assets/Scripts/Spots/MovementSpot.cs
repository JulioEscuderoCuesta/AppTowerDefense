using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
public class MovementSpot : Spot
{

    [SerializeField] private List<MovementSpot> spotsToMoveForward;
    [SerializeField] private MovementSpot leftSpot;
    [SerializeField] private MovementSpot rightSpot;
    [SerializeField] private Dictionary<MovementSpot, float> keySpots;
    [SerializeField] private List<Spot> closestStairs;
    [SerializeField] private bool spawner;
    [SerializeField] private Dictionary<Wall, float> targetWalls;
    [SerializeField] private Wall closestWallToClimb;

    //Marca los puntos clave que se usarán para calcular el movimiento del defensor.
    [SerializeField] private bool keySpot;
    [SerializeField] private MovementSpot jumpPoint;
    [SerializeField] private Door door;
	[SerializeField] private TowerSpot towerSpot;
    private float distanceToJumpPoint;
    [SerializeField] private bool endPoint;


    public Dictionary<MovementSpot,float> KeySpots => keySpots;
    public MovementSpot LeftSpot => leftSpot;
    public MovementSpot RightSpot => rightSpot;
    public Wall ClosestWallToClimb => closestWallToClimb;
    public Dictionary<Wall, float> GetTargetWalls => targetWalls;
    public List<Spot> ClosestStairs => closestStairs;
    public MovementSpot JumpPoint => jumpPoint;
	public TowerSpot TowerSpot => towerSpot;
    public bool EndPoint => endPoint;
    public Door Door => door;
    public bool IsKeySpot => keySpot;
    public float DistanceToJumpPoint => distanceToJumpPoint;

    protected override async void retrieveData()
    {

        spotsToMoveForward = new List<MovementSpot>();
        targetWalls = new Dictionary<Wall, float>();
        keySpots = new Dictionary<MovementSpot, float>();

        if (gameObject.name != "Auxiliar Spot")
        {
            
            DTOMovementSpot dtoMovementSpot = await DAOSpot.SharedInstance.GetMovementSpot(gameObject.name);
            if(gameObject.tag == "D1" || gameObject.tag == "D2" || gameObject.tag == "D3")
            {
                
                DTOMovementSpotsForDefenders dtoMovementSpotForDefenders = await DAOSpot.SharedInstance.GetMovementSpotForDefenders(gameObject.name);
                GetKeySpots(dtoMovementSpotForDefenders.KeySpots);
                if(gameObject.tag != "D3")
                    GetClosestsStairs(dtoMovementSpotForDefenders.ClosestStairs);
                keySpot = dtoMovementSpotForDefenders.IsKeySpot;
            }      
            GetTargetWallsFromDTO(dtoMovementSpot.TargetWalls);
            GetSpotsToMoveForwardFromDTO(dtoMovementSpot.SpotsToMoveForward);
            GetSpotsToMoveToTheSidesFromDTO(dtoMovementSpot.Right, dtoMovementSpot.Left);
            GetJumpPointFromDTO(dtoMovementSpot.JumpPoint);
            GetSpotsToAttackFromDTO(dtoMovementSpot.SpotsToAttack);
            GetClosestWallToClimbFromDTO(dtoMovementSpot.ClosestWallToClimb);
            spawner = dtoMovementSpot.Spawner;
            GetDoorFromDTO(dtoMovementSpot.Door);
			GetTowerSpotFromDTO(dtoMovementSpot.TowerSpot);

        }
    }

    private void GetSpotsToMoveToTheSidesFromDTO(string dtoRight, string dtoLeft)
    {
        string leftString = gameObject.tag + '_' + dtoLeft;
        string rightString = gameObject.tag + '_' + dtoRight;
	
        leftSpot = GameObject.Find(leftString).GetComponent<MovementSpot>();
        rightSpot = GameObject.Find(rightString).GetComponent<MovementSpot>();
    }


    private void GetSpotsToMoveForwardFromDTO(List<string> dto)
    {	

        foreach (var spot in dto)
        {
            spotsToMoveForward.Add(GameObject.Find(spot).GetComponent<MovementSpot>());
        }
    }

    private void GetKeySpots(Dictionary<string, float> dtoKeySpots)
    {		
        foreach (var pair in dtoKeySpots)
        {
            keySpots.Add(GameObject.Find(pair.Key).GetComponent<MovementSpot>(), pair.Value);
        }
    }
    
    private void GetClosestsStairs(List<string> dto)
    {		

        foreach (var spot in dto)
        {
            closestStairs.Add(GameObject.Find(spot).GetComponent<MovementSpot>());
        }
    }

    private void GetTargetWallsFromDTO(Dictionary<string, float> dtoTargetWalls)
    {		

        foreach (var wall in dtoTargetWalls)
        {
            targetWalls.Add(GameObject.Find(wall.Key).GetComponent<Wall>(), wall.Value);
        }
    }

    private void GetClosestWallToClimbFromDTO(string dtoclosestWallToClimb)
    {
        if (dtoclosestWallToClimb == "Auxiliar Wall")
            closestWallToClimb = GameObject.Find("Auxiliar Wall").GetComponent<Wall>();
        else
            closestWallToClimb = GameObject.Find(dtoclosestWallToClimb).GetComponent<Wall>();
    }

    private void GetJumpPointFromDTO(Dictionary<string, float> dtoJumpPointFromDTO)
    {
        		

        foreach (var pair in dtoJumpPointFromDTO)
        {
            if (pair.Key != "Auxiliar Spot")
            {
                jumpPoint = GameObject.Find(pair.Key).GetComponent<MovementSpot>();
                distanceToJumpPoint = pair.Value;
            }
            else
            {
                jumpPoint = GameObject.Find("Auxiliar Spot").GetComponent<MovementSpot>();;
                distanceToJumpPoint = 0;

            }
        }
    }

    private void GetDoorFromDTO(string dto)
    {
        if (dto == "Auxiliar Wall")
            door = GameObject.Find("Auxiliar Wall").GetComponent<Door>();
        else
            door = GameObject.Find(dto).GetComponent<Door>();
    }

	private void GetTowerSpotFromDTO(string dto)
    {
        if (dto == "Auxiliar Spot")
            towerSpot = GameObject.Find("Auxiliar Spot").GetComponent<TowerSpot>();
        else
            towerSpot = GameObject.Find(dto).GetComponent<TowerSpot>();
    }
    private Spot GetSpotToMoveSameRing(string direction)
    {
        if (gameObject.tag == "U1")
            return null;
        MovementSpot spotToGo = null;
        if (direction == "left")
        {
            if (leftSpot.Soldier == null)
                spotToGo = leftSpot;
            else if (rightSpot.Soldier == null)
                spotToGo = rightSpot;
        }
        else
        {
            if (rightSpot.Soldier == null)
                spotToGo = rightSpot;
            else if (leftSpot.Soldier == null)
                spotToGo = leftSpot;
        }
        return spotToGo;
    }

    private bool CanMoveForward()
    {
        bool can = false;
        foreach (var spot in spotsToMoveForward)
        {
            if (spot.Soldier == null)
                can = true;
        }
        return can;
    }

    private Spot GetSpotToMoveForward()
    {
        MovementSpot spotToGo = null;
		List<MovementSpot> possibleSpotsToMove = new List<MovementSpot>();
        MovementSpot possibleSpotToGo = null;
		if(gameObject.tag == "A3")
		{
			foreach(MovementSpot aux in spotsToMoveForward)
			{
				if(soldier.GetWeapon.WeaponValues.Type == Class.INFANTERY && aux.gameObject.tag == "A4")
					possibleSpotsToMove.Add(aux);
				else if ((soldier.GetWeapon.WeaponValues.Type == Class.SMALL_ARTILLERY ||
                         soldier.GetWeapon.WeaponValues.Type == Class.BIG_ARTILLERY)&& 
                         aux.gameObject.tag == "A5")
                    possibleSpotsToMove.Add(aux);
            }
		}
		
        if(possibleSpotsToMove.Count != 0)
            possibleSpotToGo = possibleSpotsToMove[Random.Range(0, possibleSpotsToMove.Count)];
        else 
            possibleSpotToGo = spotsToMoveForward[Random.Range(0, spotsToMoveForward.Count)];
        
        if (possibleSpotToGo.Soldier == null)
            spotToGo = possibleSpotToGo;
        return spotToGo;
    }

    public Spot getSpotToMove(string direction)
    {
        Spot spotToGo = null;
        
        //Si desde el punto actual se puede bajar las almenas, se baja
        if (jumpPoint.name != "Auxiliar Spot" && jumpPoint.gameObject.tag == "D2" 
                                               && jumpPoint.Soldier == null)
            return jumpPoint;
        //Si no, si desde el punto actual se puede acceder a las almenas y hay defensores en ellas
        //se contempla la posibilidad de subir ahí.
        if (towerSpot.name != "Auxiliar Spot" && towerSpot.Soldier == null && towerSpot.DefendersInTowers())
            return towerSpot;
        
        //Si no, se intenta avanzar siempre hacia delante. En caso contrario se avanza lateralmente
        if (spotsToMoveForward.Count == 0)
            spotToGo = GetSpotToMoveSameRing(direction);
        else
        {
            if (CanMoveForward())
                spotToGo = GetSpotToMoveForward();
            else
                spotToGo = GetSpotToMoveSameRing(direction);
        }
        return spotToGo;

    }

    public bool CanMoveToTheLeft()
    {
        if (leftSpot.Soldier == null)
            return true;
        return false;
    }

    public bool CanMoveToTheRight()
    {
        if (rightSpot.Soldier == null)
            return true;
        return false;
    }




}