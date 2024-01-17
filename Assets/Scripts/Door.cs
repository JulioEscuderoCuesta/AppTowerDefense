using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Life life;

    private void Start()
    {
        life = GetComponent<Life>();
        life.onDeath.AddListener(DestroyDoor);
    }

    private void DestroyDoor()
    {
        GetComponent<MeshRenderer>().enabled = false;
        life.onDeath.RemoveListener(DestroyDoor);
    }

    public bool IsWallDestroyed()
    {
        if(GetComponent<MeshRenderer>().enabled)
            return false;
        return true;
    }
}
