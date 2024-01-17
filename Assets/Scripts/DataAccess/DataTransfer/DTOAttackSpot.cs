using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

[FirestoreData]
public class DTOAttackSpot
{
    private Dictionary<string, float> _spotsToAttack;
    private string _connectionPoint;

    [FirestoreProperty] public Dictionary<string, float> SpotsToAttack { get; set; }
    [FirestoreProperty] public string ConnectionPoint { get; set; }
}
