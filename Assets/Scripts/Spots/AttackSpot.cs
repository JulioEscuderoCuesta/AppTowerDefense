using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpot : Spot
{
    [SerializeField] private MovementSpot connectionSpot;

    public MovementSpot GetConnectionSpot => connectionSpot;

    protected override async void retrieveData()
    {
        if (gameObject.name != "Auxiliar Spot")
        {
            DTOAttackSpot dto = await DAOSpot.SharedInstance.GetAttackSpot(gameObject.name);
            GetConnectionSpotFromDTO(dto.ConnectionPoint);
            GetSpotsToAttackFromDTO(dto.SpotsToAttack);
        }   
    }

    private void GetConnectionSpotFromDTO(string dto)
    {
        connectionSpot = GameObject.Find(dto).GetComponent<MovementSpot>();
    }

    void Update()
    {
        //Si estoy en medio de una oleada
        /*if (!GameManager.SharedInstance.InPositioningStage)
        {
            if (GameManager.SharedInstance.SoldierSelected != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.name == gameObject.name)
                    {
                        GameManager.SharedInstance.AttackSpot = this;
                    }
                }
            }
            else
                GameManager.SharedInstance.AttackSpot = null;
        }*/
    }

    void OnMouseDown()
    {
        //Si selecciono un punto de ataque mientras estoy en la fase de colocación y estoy colocando
        //unidades, coloco la unidad.
        if (GameManager.SharedInstance.InPositioningStage && GameManager.SharedInstance.SoldierSelected != null)
        {
            GameManager.SharedInstance.PositioningAttackSpot(this);
            
        }
    }

    void OnMouseOver()
    {
        //TODO Mostrar características del punto
		if (!GameManager.SharedInstance.InPositioningStage && GameManager.SharedInstance.SoldierSelected != null)
        {
			GameObject soldier = GameManager.SharedInstance.SoldierSelected;
            if ((gameObject.tag == "C1" || gameObject.tag == "C2" || gameObject.tag == "T1" || gameObject.tag == "T2")
                && soldier.GetComponent<Character>().GetWeapon.WeaponValues.Type == Class.BIG_ARTILLERY)
            {
                GameManager.SharedInstance.AttackSpot = this;

            }
			else if((gameObject.tag == "B1" || gameObject.tag == "B2" || gameObject.tag == "B2") 
                    && (soldier.GetComponent<Character>().GetWeapon.WeaponValues.Type == Class.INFANTERY)
                    || (soldier.GetComponent<Character>().GetWeapon.WeaponValues.Type == Class.ARCHER))
	        	GameManager.SharedInstance.AttackSpot = this;
            else if((gameObject.tag == "S1" || gameObject.tag == "S2")
                    && soldier.GetComponent<Character>().GetWeapon.WeaponValues.Type == Class.SMALL_ARTILLERY)
                GameManager.SharedInstance.AttackSpot = this;
            else if(gameObject.tag == "M1" && soldier.GetComponent<Character>().GetWeapon.WeaponValues.Type == Class.MOZO)
                GameManager.SharedInstance.AttackSpot = this;
    	}
	}

}