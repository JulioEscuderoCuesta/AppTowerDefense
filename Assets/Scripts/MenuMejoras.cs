/*Menu Mejoras
 Script que controla la navegacion por las pestañas del menu de mejoras*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Weapons;

public class MenuMejoras : MonoBehaviour

{
    public GameObject arqueros;

    public GameObject infanteria;

    public GameObject PM;

    public GameObject PG;

    public GameObject castillo;
    
    public TextMeshProUGUI ducados;

    private List<Weapon> arcos;
    private GameObject previousGameObject;
    private GameObject previousButtonGameObject;
    private Weapon[] arrayWeapons;

    public void OnEnable()
    {
        ducados.text = GameManager.SharedInstance.GetPlayerMoney.ToString();
    }

    public void arquerosButton()
    {
        arqueros.SetActive(true);
        infanteria.SetActive(false);
        PM.SetActive(false);
        PG.SetActive(false);
        castillo.SetActive(false);
    }
    public void infanteriaButton()
    {
        arqueros.SetActive(false);
        infanteria.SetActive(true);
        PM.SetActive(false);
        PG.SetActive(false);
        castillo.SetActive(false);
    }
    public void pmButton()
    {
        arqueros.SetActive(false);
        infanteria.SetActive(false);
        PM.SetActive(true);
        PG.SetActive(false);
        castillo.SetActive(false);
    }
    public void pgButton()
    {
        arqueros.SetActive(false);
        infanteria.SetActive(false);
        PM.SetActive(false);
        PG.SetActive(true);
        castillo.SetActive(false);
    }
    public void castilloButton()
    {
        arqueros.SetActive(false);
        infanteria.SetActive(false);
        PM.SetActive(false);
        PG.SetActive(false);
        castillo.SetActive(true);
    }

    public void ShowCard(GameObject card)
    {
        previousGameObject.SetActive(true);
        card.SetActive(false);
        previousGameObject = card;
    }

    public void ShowButton(GameObject buttonCard)
    {
        previousButtonGameObject.SetActive(false);
        buttonCard.SetActive(true);
        previousButtonGameObject = buttonCard;
    }
    
    public void Brazal()
    {
        Debug.Log($"brazal");
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.ARCHER)
            {
                weapon.WeaponValues.FireRate += 5;
            }
        }

    }
    public void ArcoCompuesto()
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.ARCHER)
            {
                weapon.WeaponValues.Damage += 2;
                weapon.WeaponValues.Range += 5;
            }
        }
    }
    
    public void Dactilera()
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.ARCHER)
            {
                weapon.WeaponValues.Range += 5;
            }
        }
    }
    public void FlechasIncendiarias()
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.ARCHER)
            {
                weapon.WeaponValues.Damage += 5;
                weapon.WeaponValues.Range += 5;
            }
        }
    }
    public void Ballesta()
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.ARCHER)
            {
                weapon.WeaponValues.Damage += 5;
                weapon.WeaponValues.Range += 5;
                weapon.WeaponValues.FireRate -= 40;
                weapon.WeaponValues.Accuracy += 5;

            }
        }
    }
    public void Gafa()
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.ARCHER)
            {
                weapon.WeaponValues.FireRate += 20;
            }
        }
    }

    public void CanonDeMano(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.INFANTERY)
            {
                weapon.WeaponValues.Damage += 2;
                weapon.WeaponValues.Range += 2;
                weapon.WeaponValues.FireRate += 5;
                weapon.WeaponValues.Accuracy += 5;
            }
        }
    }
    public void Espingarda(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.INFANTERY)
            {
                weapon.WeaponValues.Damage += 3;
                weapon.WeaponValues.Range += 2;
                weapon.WeaponValues.FireRate += 5;
                weapon.WeaponValues.Accuracy += 5;

            }
        }
    }
    public void CubreCazoletas(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.INFANTERY)
            {
                weapon.WeaponValues.FireRate += 5;
                weapon.WeaponValues.Accuracy += 5;
            }
        }
    }
    public void Arcabuz(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.INFANTERY)
            {
                weapon.WeaponValues.Damage += 3;
                weapon.WeaponValues.Range += 2;
                weapon.WeaponValues.FireRate += 5;
                weapon.WeaponValues.Accuracy += 5;
            }
        }
    }
    public void BalasDePlomo(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.INFANTERY)
            {
                weapon.WeaponValues.Damage += 3;
            }
        }
    }
    public void Mosquete(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.INFANTERY)
            {
                weapon.WeaponValues.Damage += 4;
                weapon.WeaponValues.Range += 3;
                weapon.WeaponValues.FireRate += 5;
                weapon.WeaponValues.Accuracy += 15;
            }
        }
    }
    public void Horquilla(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.INFANTERY)
            {
                weapon.WeaponValues.Accuracy += 100;
            }
        }
    }

    public void MedioRibadoquin(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.SMALL_ARTILLERY)
            {
                weapon.WeaponValues.Damage += 5;
                weapon.WeaponValues.Range += 10;
                weapon.WeaponValues.Accuracy += 10;
            }
        }
    }
    
    public void Falconete(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.SMALL_ARTILLERY)
            {
                weapon.WeaponValues.Damage += 10;
            }
        }
    }
    
    public void SacreDeFonseca(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.SMALL_ARTILLERY)
            {
                weapon.WeaponValues.Damage += 10;
                weapon.WeaponValues.Range += 20;
                weapon.WeaponValues.Accuracy -= 10;
                weapon.WeaponValues.Speed -= 10;

            }
        }
    }
    
    public void MediaCulebrilla(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.SMALL_ARTILLERY)
            {
                weapon.WeaponValues.Damage += 10;
                weapon.WeaponValues.FireRate += 10;
                weapon.WeaponValues.Accuracy += 10;
            }
        }
    }

    public void Bombarda(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.BIG_ARTILLERY)
            {
                weapon.WeaponValues.Damage += 20;
                weapon.WeaponValues.Accuracy -= 10;
                weapon.WeaponValues.Speed -= 10;
            }
        }
    }
    
    public void CulebrinaDeJuanaI(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.BIG_ARTILLERY)
            {
                weapon.WeaponValues.Damage -= 10;
                weapon.WeaponValues.Range += 30;
                weapon.WeaponValues.FireRate += 30;
                weapon.WeaponValues.Accuracy += 20;
                weapon.WeaponValues.Speed += 20;
            }
        }
    }
    
    public void CanonFernandoElCatolico(GameObject g)
    {
        arrayWeapons = FindObjectsOfType(typeof(Weapon)) as Weapon[];
        foreach (Weapon weapon in arrayWeapons)
        {
            if (weapon.WeaponValues.Type == Class.BIG_ARTILLERY)
            {
                weapon.WeaponValues.Damage += 20;
                weapon.WeaponValues.Speed -= 10;
            }
        }
    }
    
    public void Calabozo()
    {
        GameManager.SharedInstance.moneyMultiplier += 0.2f;
    }
    
    public void Rastrillo()
    {
        
    }
    
    public void PozoDeSuministros()
    {
        
    }
    
    public void Botica()
    {
        GameManager.SharedInstance.recoveryMultiplier += 0.2f;
    }
    
    public void Enfermeria()
    {
        GameManager.SharedInstance.moneyMultiplier += 0.2f;
    }
    
    public void Matacanes()
    {
        GameObject[] matacanes = GameObject.FindGameObjectsWithTag("M1");
        foreach (var m in matacanes)
            m.SetActive(true);
    }
    
    public void Buhedera()
    {
        
    }

    public void ReconstuirMuralla1()
    {
        GameManager.SharedInstance.wallsRecoveryMultiplier = 0.1f;
    }
    public void ReconstuirMuralla2()
    {
        GameManager.SharedInstance.wallsRecoveryMultiplier = 0.2f;
    }
    public void ReconstuirMuralla3()
    {
        GameManager.SharedInstance.wallsRecoveryMultiplier = 0.25f;
    }
    
    public void ReconstruirArcos1()
    {
        
    }
    
    public void ReconstruirArcos2()
    {
        
    }
    
    public void ReconstruirArcos3()
    {
        GameManager.SharedInstance.UnlockTrabuquete();
    }
}
