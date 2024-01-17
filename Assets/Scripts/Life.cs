using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour
{
    [Tooltip("Vida del objeto")]
    [SerializeField]private float amount;

    [Tooltip("Vida máxima del objeto")]
    private float maximumLife;
    public UnityEvent onDeath;

    private void Awake()
    {
        amount = maximumLife;
    }

    public float MaximumLife => maximumLife;

    public float Amount
    {
        get => amount;
        set
        {
            amount = value;
            if (amount <= 0) 
                onDeath.Invoke();
        } 
    }
}