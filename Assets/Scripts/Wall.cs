using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Wall : MonoBehaviour
{
    [SerializeField] private SiegeTowerPoint siegeTowerPoint;
    [SerializeField] private SiegeTowerDestinationPoint siegeTowerDestinationPoint;

    public GameObject topOfTheWall;
    private Life life;
    public Life Life => life;
    public SiegeTowerPoint GetSiegeTowerPoint => siegeTowerPoint;
    public SiegeTowerDestinationPoint GetSiegeTowerDestinationPoint => siegeTowerDestinationPoint;

    private void Start()
    {
        life = GetComponent<Life>();
        life.onDeath.AddListener(DestroyWall);
    }

    private void DestroyWall()
    {
        topOfTheWall.SetActive(false);
        life.onDeath.RemoveListener(DestroyWall);
    }

    public bool IsWallDestroyed()
    {
        if(topOfTheWall.activeSelf)
            return false;
        return true;
    }

    
}