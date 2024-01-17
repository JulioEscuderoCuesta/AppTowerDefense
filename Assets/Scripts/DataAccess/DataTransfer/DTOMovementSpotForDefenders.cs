using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

[FirestoreData]
public class DTOMovementSpotsForDefenders
{

    [FirestoreProperty] public Dictionary<string, float> KeySpots { get; set; }
    [FirestoreProperty] public List<string> ClosestStairs { get; set; }
    [FirestoreProperty] public bool IsKeySpot { get; set; }

}
