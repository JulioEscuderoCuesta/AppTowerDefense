using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class Consumer : MonoBehaviour
{
    public static Consumer SharedInstance;
    private FirebaseApp _app;

    private void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Firebase
            .FirebaseApp
            .CheckAndFixDependenciesAsync()
            .ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    _app = Firebase.FirebaseApp.DefaultInstance;
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    //AddData();
                }
                else
                {
                    UnityEngine
                        .Debug
                        .LogError(System
                            .String
                            .Format("Could not resolve all Firebase dependencies: {0}",
                            dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }

            });
    }

    public void AddDataMovementSpot(string spot, List<string> nearest3Spots, string right, string left, Dictionary<string, float> spotsToAttack, string closestWallToClimb,
                Dictionary<string, float> wallsToAttack, bool spawner, Dictionary<string,float> jumpPoint, string doorString, string towerSpotString)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        DocumentReference docRef = db.Collection("MovementSpots").Document(spot);
        DTOMovementSpot dto = new DTOMovementSpot
        {
            TargetWalls = wallsToAttack,
            SpotsToMoveForward = nearest3Spots,
            Right = right,
            Left = left,
            SpotsToAttack = spotsToAttack,
            ClosestWallToClimb = closestWallToClimb,
            Door = doorString,
            TowerSpot = towerSpotString,
            Spawner = spawner,
            JumpPoint = jumpPoint,
        };

        docRef.SetAsync(dto).ContinueWithOnMainThread(task =>
            {
                Debug
                    .Log("Added data to the MovementSpot document in the users collection.");
            });
    }

    public void AddDataMovementSpotForDefenders(string spot, Dictionary<string, float> keySpots, bool keySpot)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        DocumentReference docRef = db.Collection("MovementSpotsForDefenders").Document(spot);
        DTOMovementSpotsForDefenders dto = new DTOMovementSpotsForDefenders
        {
            KeySpots = keySpots,
            IsKeySpot = keySpot,
        };

        docRef.SetAsync(dto).ContinueWithOnMainThread(task =>
            {
                Debug
                    .Log("Added data to the MovementSpot document in the users collection.");
            });
    }

    public void AddDataAttackSpot(string spot, Dictionary<string, float> spotsToAttack, string connectionPoint)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        DocumentReference docRef = db.Collection("AttackSpots").Document(spot);
        DTOAttackSpot dto = new DTOAttackSpot
        {
            SpotsToAttack = spotsToAttack,
            ConnectionPoint = connectionPoint,
        };

        docRef.SetAsync(dto).ContinueWithOnMainThread(task =>
            {
                Debug
                    .Log("Added data to the AttackSpots document in the users collection.");
            });
    }
    
    public void AddDataTowerSpot(string spot, List<string> spotsToAttack, string connectionPoint)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        DocumentReference docRef = db.Collection("TowerSpots").Document(spot);
        DTOTowerSpot dto = new DTOTowerSpot
        {
            SpotsToAttack = spotsToAttack,
            ConnectionPoint = connectionPoint,
        };

        docRef.SetAsync(dto).ContinueWithOnMainThread(task =>
        {
            Debug
                .Log("Added data to the TowerSpots document in the users collection.");
        });
    }

    public void AddDataSiegeTowerDestinationPointData(string spot, List<string> spotsToMove)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        DocumentReference docRef = db.Collection("SiegeTowerDestinationPoint").Document(spot);
        DTOSiegeTowerDestinationPoint dto = new DTOSiegeTowerDestinationPoint
        {
            SpotsToMove = spotsToMove,
        };

        docRef.SetAsync(dto).ContinueWithOnMainThread(task =>
            {
                Debug
                    .Log("Added data os siege tower destination point to the document in the users collection.");
            });
    }

}
