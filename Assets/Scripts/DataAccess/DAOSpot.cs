using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class DAOSpot : MonoBehaviour
{
    private FirebaseApp _app;

    public static DAOSpot SharedInstance;


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

    private void Login()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null)
        {
            Debug.Log("Usuario ya registrado");
            return;
        }
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    public async Task<DTOMovementSpot> GetMovementSpot(string spot)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("MovementSpots").Document(spot);
        DTOMovementSpot dto = null;

        await docRef.GetSnapshotAsync().ContinueWith(task =>
                     {
                         var snapshot = task.Result;
                         if (snapshot.Exists)
                         {
                             dto = snapshot.ConvertTo<DTOMovementSpot>();
                         }
                         else
                         {
                             Debug.Log("Something happened... Error");
                         }

                     });
        return dto;
    }

    public async Task<DTOMovementSpotsForDefenders> GetMovementSpotForDefenders(string spot)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("MovementSpotsForDefenders").Document(spot);
        DTOMovementSpotsForDefenders dto = null;

        await docRef.GetSnapshotAsync().ContinueWith(task =>
                     {
                         var snapshot = task.Result;
                         if (snapshot.Exists)
                         {
                             //Debug.Log("Succesfull Read SpotDAO");
                             dto = snapshot.ConvertTo<DTOMovementSpotsForDefenders>();
                         }
                         else
                         {
                             Debug.Log("Something happened... Error");
                         }

                     });
        return dto;
    }

    public async Task<DTOAttackSpot> GetAttackSpot(string spot)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("AttackSpots").Document(spot);
        DTOAttackSpot dto = null;

        await docRef.GetSnapshotAsync().ContinueWith(task =>
                     {
                         var snapshot = task.Result;
                         if (snapshot.Exists)
                         {
                             //Debug.Log("Succesfull Read SpotDAO");
                             dto = snapshot.ConvertTo<DTOAttackSpot>();
                         }
                         else
                         {
                             Debug.Log("Something happened... Error");
                         }

                     });
        return dto;
    }
    
    public async Task<DTOTowerSpot> GetTowerSpot(string spot)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("TowerSpots").Document(spot);
        DTOTowerSpot dto = null;

        await docRef.GetSnapshotAsync().ContinueWith(task =>
        {
            var snapshot = task.Result;
            if (snapshot.Exists)
            {
                //Debug.Log("Succesfull Read SpotDAO");
                dto = snapshot.ConvertTo<DTOTowerSpot>();
            }
            else
            {
                Debug.Log("Something happened... Error");
            }

        });
        return dto;
    }

    public async Task<DTOSiegeTowerDestinationPoint> GetSiegeTowerDestinationPoint(string spot)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("SiegeTowerDestinationPoint").Document(spot);
        DTOSiegeTowerDestinationPoint dto = null;

        await docRef.GetSnapshotAsync().ContinueWith(task =>
                     {
                         var snapshot = task.Result;
                         if (snapshot.Exists)
                         {
                             //Debug.Log("Succesfull Read SpotDAO SiegeTowerDestinationPoint");
                             dto = snapshot.ConvertTo<DTOSiegeTowerDestinationPoint>();
                         }
                         else
                         {
                             Debug.Log("Something happened... Error");
                         }

                     });
        return dto;
    }
}
