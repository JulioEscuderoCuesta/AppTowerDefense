using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegeTowerDestinationPoint : MonoBehaviour
{
    [SerializeField] private List<MovementSpot> _spotsToMoveFromDestinationTowerPoint;
    public List<MovementSpot> GetDestinationPoints => _spotsToMoveFromDestinationTowerPoint;

    [SerializeField] private Character soldier;
    [SerializeField]private bool taken;

    public Character Soldier
    {
        get
        {
            return soldier;
        }
        set
        {
            soldier = value;
        }
    }
    public bool Taken
    {
        get
        {
            return taken;    
        }
        set
        {
            taken = value;
        }
    }

    async void Awake()
    {
        DTOSiegeTowerDestinationPoint dto = await DAOSpot.SharedInstance.GetSiegeTowerDestinationPoint(gameObject.name);
        taken = false;
        GetPointsToMoveFromDTO(dto.SpotsToMove);
    }

    private void GetPointsToMoveFromDTO(List<string> dto)
    {
        foreach(var spot in dto)
        {
            _spotsToMoveFromDestinationTowerPoint.Add(GameObject.Find(spot).GetComponent<MovementSpot>());
        }
    }

}
