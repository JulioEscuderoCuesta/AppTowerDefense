using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControlerTest : MonoBehaviour
{
    public GameObject target;

    public NavMeshAgent agent;
    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.transform.position);
    }
}
