using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

[FirestoreData]
public class DTOSiegeTowerDestinationPoint
{
    private List<string> _spotsToMove;

    [FirestoreProperty] public List<string> SpotsToMove { get; set; }


}
