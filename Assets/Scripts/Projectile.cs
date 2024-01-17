using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int _damage;
    private Character _shooter;

    public int Damage
    {
        get => _damage;
        set => _damage = value;
    }

    public Character Shooter
    {
        get => _shooter;
        set => _shooter = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false); //Desactivar la bala
        Life life = other.GetComponent<Life>();
        if(life!=null)
        {
            life.Amount -= _damage;
        }
    }
}
