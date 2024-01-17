using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpot : Spot
{
    [SerializeField] private List<AttackSpot> attackSpots;
    [SerializeField] private MovementSpot connectionSpot;

    public MovementSpot GetConnectionSpot => connectionSpot;

    public List<AttackSpot> AttackSpots => attackSpots;
    
    protected override async void retrieveData()
    {
        if (gameObject.name != "Auxiliar Spot")
        {
            DTOTowerSpot dto = await DAOSpot.SharedInstance.GetTowerSpot(gameObject.name);
            GetConnectionSpotFromDTO(dto.ConnectionPoint);
            GetSpotsToAttackFromDTO(dto.SpotsToAttack);
        }

    }
    
    private void GetConnectionSpotFromDTO(string dto)
    {
        connectionSpot = GameObject.Find(dto).GetComponent<MovementSpot>();
    }

    private void GetSpotsToAttackFromDTO(List<string> dto)
    {
        foreach (var x in dto)
        {
            attackSpots.Add(GameObject.Find(x).GetComponent<AttackSpot>());
        }
    }

    public bool DefendersInTowers()
    {
        foreach (AttackSpot x in attackSpots)
        {
            if (x.Soldier != null)
            {
                Debug.Log($"dentro !null");
                Debug.Log($"{soldier}");
                return true;
            }
        }

        return false;
    }
}
