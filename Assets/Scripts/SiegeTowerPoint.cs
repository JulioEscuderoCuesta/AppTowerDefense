using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegeTowerPoint : MonoBehaviour
{
    public GameObject firstPartOfTheSiegeTower;
    public GameObject secondPartOfTheSiegeTower;
    public GameObject thirdPartOfTheSiegeTower;

    private float _time;
    private bool _siegeTowerFinished;
    [SerializeField]private bool taken;
    [SerializeField] private Character soldierWhoStartsBuildingTheTower;
    [SerializeField] private Character soldier;

    public bool IsSiegeTowerBuilt => _siegeTowerFinished;
    public Character WhoIsBuilding => soldierWhoStartsBuildingTheTower;
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

    void Start()
    {
        _siegeTowerFinished = false;
        soldierWhoStartsBuildingTheTower = null;
        taken = false;
        _time = 3;
    }

    public void StartBuildingTower(Character g)
    {
        soldierWhoStartsBuildingTheTower = g;
    }

    public void BuildTower()
    {
        _time -= Time.deltaTime;

        if (_time < 3f && _time >= 2f)
        {       

            firstPartOfTheSiegeTower.SetActive(true);
        }
        if (_time < 2f && _time >= 1f)
        { 

            secondPartOfTheSiegeTower.SetActive(true);
        }
        if (_time < 1f && _time > 0f)
        {        

            thirdPartOfTheSiegeTower.SetActive(true);
        }
        if (_time <= 0f)
        {
            _time = 3f;
            _siegeTowerFinished = true;
        }
    }

}


