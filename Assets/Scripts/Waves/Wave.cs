using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class Wave
{
    private int _waveNumber;
    private int _reward;
    
    //Se guarda tanto el número de enemigos de la oleada, como el número restante de enemigos.
    private int _enemies;
    private int _enemiesLeft;
    private int archers;
    private int infantery;
    private int smallArtillery;
    private int bigArtillery;
    private int morters;

    public int WaveNumber => _waveNumber;
    public int Archers
    {
        get => archers;
        set => archers = value;
    }
    public int Infantery
    {
        get => infantery;
        set => infantery = value;
    }
    public int BigArtillery
    {
        get => smallArtillery;
        set => smallArtillery = value;
    }
    public int SmallArtillery
    {
        get => bigArtillery;
        set => bigArtillery = value;
    }

    public int Morters
    {
        get => morters;
        set => morters = value;
    }

    public int Enemies=> _enemies;

    public int EnemiesLeft
    {
        get => _enemiesLeft;
        set => _enemiesLeft = value;
    }
    
    public int Reward => _reward;

    public Wave(int waveNumber, int numberOfEnemies, int reward)
    {
        _waveNumber = waveNumber;
        _enemies = numberOfEnemies;
        _reward = reward;
    }

    public void RemoveEnemy()
    {
        _enemiesLeft--;
    }

}
