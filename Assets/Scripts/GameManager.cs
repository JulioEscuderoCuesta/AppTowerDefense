using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    public static GameManager SharedInstance;

    public GameObject prefabArquero;
    public GameObject prefabInfanteria;
    public GameObject prefabArtilleriaMenuda;
    public GameObject prefabArtilleriaPesada;
    public GameObject prefabMozoMuro;

    private List<Character> defensores;
    private List<Wall> walls;
    public Wall knightsTower;

    [SerializeField]
    [Tooltip("Vida de la torre del homenaje")]
    private Life towerLife;
    private GameObject soldierSelected = null;
    private AttackSpot attackSpot;
    private bool _inPositioningStage;
    private int money = 100;
    public float moneyMultiplier = 1f;
    public float recoveryMultiplier = 0.2f;
    public float towerLevel = 0;
    public float wallsRecoveryMultiplier = 0f;
    private int nextWave;

    public GameObject SoldierSelected
    {
        get => soldierSelected;
        set => soldierSelected = value;
    }
    public AttackSpot AttackSpot
    {
        get => attackSpot;
        set => attackSpot = value;
    }

    public bool InPositioningStage => _inPositioningStage;
    public int GetPlayerMoney => money;

    public int GetNextWave
    {
        get => nextWave;
        set => nextWave = value;
    }


    private void Awake()
    {
        if (SharedInstance == null)
            SharedInstance = this;
		WaveManager.SharedInstance.OnWaveFinished.AddListener(BattleStage);
        defensores = new List<Character>();
        walls = new List<Wall>();
        /*GameObject[] wallsObject = GameObject.FindGameObjectsWithTag("Wall");
        foreach (var wall in wallsObject)
        {
            walls.Add(wall.GetComponent<Wall>());
        }
        knightsTower = GameObject.FindWithTag("KnightsTower").GetComponent<Wall>();*/

    }

    // Start is called before the first frame update
    void Start()
    {
        //Initial money


        //Get everyobject for each layer so when looking for object in the hierarchy, it takes less time. 
        

        //internalWalls = FindGameObjectsWithLayer(9);
        //towerWalls = FindGameObjectsWithLayer(10);
        

        /*foreach(var wall in externalWalls)
        {
            wall.GetComponent<Wall>().onWallDestroyed.AddListener(LoadWallDestroyed);
        }

        //Listeners to detect if the game ends or if the player survives a wave.
        towerLife.onDeath.AddListener(GameOver);
        WaveManager.SharedInstance.OnWaveFinished.AddListener(PositioningStage);
        WaveManager.SharedInstance.LastWaveFinished.AddListener(Victory);

        //Button listeners to know when each stage should be perform


        //The game starts with the purchase and positioning of the units
        displayPositioningMenu.Invoke();*/
        PositioningStage();

    }

    /*public GameObject[] FindGameObjectsWithTag(string tag)
    {
        GameObject[] goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        List<GameObject> goList = new List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].tag == tag)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0) return null;
        return goList.ToArray();

    }*/

    public void PositioningStage()
    {
        //Señalar que se está en la fase de colocación, de modo que no pueden aparecer enemigos
        _inPositioningStage = false;
        //Sumar el dinero obtenido de la oleada
        /*money += (int) Math.Round(WaveManager.SharedInstance.LastWave.Reward * moneyMultiplier);
        //Curar las unidades defensoras y la torre del homenaje y los muros
        //un porcentaje de la vida máxima de acuerdo a las mejoras obtenidas
        foreach (Character c in defensores)
        {
            c.Life.Amount += c.Life.MaximumLife * recoveryMultiplier;
        }
        foreach (Wall w in walls)
        {
            w.Life.Amount += w.Life.MaximumLife * wallsRecoveryMultiplier;
        }*/ 

    }

    public void BattleStage()
    {
        /*BORRAR Deshabilitar controles que permiten comprar y colocar unidades
       pero permitir el movimiento de unidades de una posición a otra.*/
        
		WaveManager.SharedInstance.spawning = false;
        _inPositioningStage = false;
        
		//Si era la última oleada, se acabó la partida
		if(WaveManager.SharedInstance.LastWave.WaveNumber == 30)
		{
			Victory();
		}
       
    }

    public void UnlockTrabuquete()
    {
        //Habilitar la compra del trabuquete en el menú.
    }

    public void PositioningAttackSpot(AttackSpot attackSpot)
    {
        Instantiate(GameManager.SharedInstance.SoldierSelected, transform.position, transform.rotation);
        Character aux = GameManager.SharedInstance.SoldierSelected.GetComponent<Character>();
        attackSpot.Soldier = aux;
        GameManager.SharedInstance.SoldierSelected.GetComponent<AllyAI>()._currentSpot = attackSpot;
    }

    private void LoadWallDestroyed(Wall wall)
    {
        string wallName = wall.gameObject.name;/*
        foreach(var destroyedWall in destroyedExteneralWalls)
        {
            if (destroyedWall.name == $"{wallName}_Roto")
            {
                wall.gameObject.SetActive(false);
            }
        }*/

    }

    private void GameOver()
    {
        //TODO Register puntuation
        SceneManager.LoadScene("Lose Scene");
    }

    private void Victory()
    {
        //TODO Register puntuation
        SceneManager.LoadScene("Win Scene");
    }
}
