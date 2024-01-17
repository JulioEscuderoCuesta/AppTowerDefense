using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

public class WaveManager : MonoBehaviour
{
    public static WaveManager SharedInstance;
    [SerializeField] private List<MovementSpot> spawnSpots;
    public Character archerAttackerPrefab;
    public Character infanteryAttackerPrefab;
    public Character smallArtilleryAttackerPrefab;
    public Character bigArtilleryAttackerPrefab;
    public Character morterAttackerPrefab;

    private Wave _lastWave;
    private int _numOfwaves;
    public bool spawning;
    private float _time;
    private List<Wave> _waves;
    public UnityEvent OnWaveFinished;

    //DEVOLER TODA LA LISTA O SOLO LA ÚLTIMA OLEADA????
    public List<Wave> Waves => _waves;
    public Wave LastWave => _lastWave;

    private void Awake()
    {
        if (SharedInstance == null)
            SharedInstance = this;
        retrieveData();
    }

    private void Start()
    {
        spawning = false;
        _lastWave = null;
        _numOfwaves = 30;
        _time = 0;
        _waves = new List<Wave>();
        
    }

    private void Update()
    {

        if (spawning == false && GameManager.SharedInstance.InPositioningStage == false)
        {
            StartCoroutine(SpawnWave());
        }
        /*else if (_lastWave.EnemiesLeft == 0)
        {
            GameManager.SharedInstance.NextWave++;
            Debug.Log($"Enemigo muere");

            OnWaveFinished.Invoke();
        }*/

    }

    private async void retrieveData()
    {
        List<string> spawnSpotsDAO = await DAOWaves.SharedInstance.getSpawnSpots();
        foreach (var spot in spawnSpotsDAO)
        {
            spawnSpots.Add(GameObject.Find(spot).GetComponent<MovementSpot>());
        }
    }

    private IEnumerator SpawnWave()
    {
        //Crear la oleada
        CreateWave();
        
        //Señalar que comienzan a aparecer los atacantes
        spawning = true;
        yield return new WaitForSeconds(5);
        _time += Time.deltaTime;

        //Se invocan los enemigos poco a poco TODO decidir tiempo entre invocaciones o cada
        //cuantos se invoca
        for (int i = 0; i < _lastWave.Enemies; i++)
        {
            //TODO qué tipo de enemigo se instancia en función de qué parámetros
            Spot s = spawnSpots[Random.Range(0, spawnSpots.Count)];
            SelectEnemyToSpawn(s);
            yield return new WaitForSeconds(0.5f);
        }

		_lastWave.EnemiesLeft = _lastWave.Enemies;
        yield break;
    }

    private void CreateWave()
    {
        List<Character> enemies = new List<Character>();
        int numberOfEnemies = 0;
        int reward = 0;
        
        if (_lastWave != null)
		{
            numberOfEnemies = _lastWave.Enemies + 10;
        	reward = _lastWave.Reward + 100;
		}

        else
		{
            numberOfEnemies = 10;
			reward = 100;
		}
        
        Wave wave = new Wave(_waves.Count + 1, numberOfEnemies, reward);
        _lastWave = wave;
        AddEnemies(numberOfEnemies);

    }

	private void AddEnemies(int numberOfEnemies)
    {
        if (_lastWave.WaveNumber < 10)
        {
            _lastWave.Archers = numberOfEnemies * 30 / 100;
            _lastWave.Infantery = numberOfEnemies * 30 / 100;
            _lastWave.SmallArtillery = numberOfEnemies * 20 / 100;
            _lastWave.BigArtillery = numberOfEnemies * 20 / 100;
        }
        else
        {
            int r = Random.Range(0, 1);
            if (r == 1)
            {
                _lastWave.Archers = numberOfEnemies * 30 / 100;
                _lastWave.Infantery = numberOfEnemies * 30 / 100;
                _lastWave.SmallArtillery = numberOfEnemies * 15 / 100;
                _lastWave.BigArtillery = numberOfEnemies * 15 / 100;
                _lastWave.BigArtillery = numberOfEnemies * 10 / 100;
            }
            else
            {
                _lastWave.Archers = numberOfEnemies * 20 / 100;
                _lastWave.Infantery = numberOfEnemies * 20 / 100;
                _lastWave.SmallArtillery = numberOfEnemies * 20 / 100;
                _lastWave.BigArtillery = numberOfEnemies * 20 / 100;
                _lastWave.BigArtillery = numberOfEnemies * 20 / 100;
            }
            
        }
        
        
    }

    private void SelectEnemyToSpawn(Spot s)
    {
        bool spawned = false;
        Character enemy = null;
        while (!spawned)
        {
            int r = Random.Range(0, 4);
            Debug.Log($"r: {r}");
            switch (r)
            {
                case 0:
                    if (_lastWave.Archers> 0)
                    {
                        enemy = Instantiate(archerAttackerPrefab, s.transform.position, s.transform.rotation);
                        enemy.CharacterKilled.AddListener(EnemyDied);
                        spawned = true;
                    }
                    break;
                case 1:
                    if (_lastWave.Infantery > 0)
                    {
                        enemy = Instantiate(infanteryAttackerPrefab, s.transform.position, s.transform.rotation);
                        enemy.CharacterKilled.AddListener(EnemyDied);
                        spawned = true;
                    }
                    break;
                case 2:
                    if (_lastWave.SmallArtillery > 0)
                    {
                        enemy = Instantiate(smallArtilleryAttackerPrefab, s.transform.position, s.transform.rotation);
                        enemy.CharacterKilled.AddListener(EnemyDied);
                        spawned = true;
                    }
                    break;
                case 3:
                    if (_lastWave.BigArtillery > 0)
                    {
                        enemy = Instantiate(bigArtilleryAttackerPrefab, s.transform.position, s.transform.rotation);
                        enemy.CharacterKilled.AddListener(EnemyDied);
                        spawned = true;
                    }
                    break;
            }
        }


    }

	private void EnemyDied()
	{
		_lastWave.RemoveEnemy();
	}
}