using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public static ObjectPool SharedInstance;
    
    [SerializeField] [Tooltip("Qué disparará el jugador")]
    private GameObject prefab;
    
    [SerializeField] [Tooltip("Lista de balas")] 
    private List<GameObject> pooledObjets;

    [SerializeField] [Tooltip("Numero de objeos a colocar en la pool")]
    private int amountToPool;
    
    public GameObject Prefab => prefab;

    private void Awake()
    {
        if (SharedInstance == null)
            SharedInstance = this;
    }

    private void Start()
    {
        //Crear las balas antes de usarlas. Se desactivan para que no se vean en pantalla y se guardan en el pool.
        pooledObjets = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(prefab);
            tmp.SetActive(false);
            pooledObjets.Add(tmp);
        }
    }
    
    public GameObject getFirstPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjets[i].activeInHierarchy)
            {
                return pooledObjets[i]; 
            }
        }

        return null; 
    }
}


