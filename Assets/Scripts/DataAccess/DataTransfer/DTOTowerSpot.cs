using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

[FirestoreData]
public class DTOTowerSpot
{
    private List<string> _spotsToAttack;
    private string _connectionPoint;

    [FirestoreProperty] public List<string> SpotsToAttack { get; set; }
    [FirestoreProperty] public string ConnectionPoint { get; set; }
}