using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

[FirestoreData]
public class DTOMovementSpot
{
    private Dictionary<string, float> _targetWalls;
    private List<string> _spotsToMoveForward;
    private Dictionary<string, float> _spotsToAttack;
    private string _closestWallToClimb;
    private string _door;
    private string _towerSpot;
    private bool _spawner;
    private Dictionary<string, float> _jumpPoint;


    [FirestoreProperty] public Dictionary<string, float> TargetWalls { get; set; }
    [FirestoreProperty] public List<string> SpotsToMoveForward { get; set; }
    [FirestoreProperty] public string Right { get; set; }
    [FirestoreProperty] public string Left { get; set; }
    [FirestoreProperty] public Dictionary<string, float> SpotsToAttack { get; set; }
    [FirestoreProperty] public string ClosestWallToClimb { get; set; }
    [FirestoreProperty] public string Door { get; set; }
    [FirestoreProperty] public string TowerSpot { get; set; }
    [FirestoreProperty] public bool Spawner { get; set; }
    [FirestoreProperty] public Dictionary<string, float> JumpPoint { get; set; }



}
