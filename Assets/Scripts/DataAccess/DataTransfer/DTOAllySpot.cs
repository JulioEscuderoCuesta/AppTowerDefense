using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

[FirestoreData]
public class DTOAllySpot
{
    private Dictionary<string, float> _spotsToAttack;

    [FirestoreProperty] public Dictionary<string, float> SpotsToAttack { get; set; }

}
