using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public abstract class Spot : MonoBehaviour
{
    [SerializeField] protected Character soldier;
    protected Dictionary<Spot, float> spotsToAttack;
    [SerializeField] protected bool taken;
    public Dictionary<Spot, float> getSpotsToAttack => spotsToAttack;

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

    private void Awake()
    {
        spotsToAttack = new Dictionary<Spot, float>();
        taken = false;
        soldier = null;
        retrieveData();
    }
    
    protected void GetSpotsToAttackFromDTO(Dictionary<string, float> dto)
    {
        foreach(var spot in dto)
        {
            spotsToAttack.Add(GameObject.Find(spot.Key).GetComponent<Spot>(), spot.Value);
        }
        
        
    }
    protected abstract void retrieveData();

}
