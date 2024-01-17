using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class DAOWaves : MonoBehaviour
{
    private FirebaseApp _app;

    public static DAOWaves SharedInstance;


    private void Awake()
    {
        if(SharedInstance==null)
            SharedInstance = this;
    }
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                _app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                //Login();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public async Task<List<string>> getSpawnSpots()
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        Query collectionRef = db.Collection("MovementSpots").WhereEqualTo("Spawner", true);
        List<string> result = new List<string>();
        await collectionRef.GetSnapshotAsync().ContinueWith(task =>
                     {
                         QuerySnapshot snapshot = task.Result;
                         foreach(DocumentSnapshot documentSnapshot in snapshot.Documents)
                        {
                            result.Add(documentSnapshot.Id);
                        }
                     });
        return result;
    }
}
