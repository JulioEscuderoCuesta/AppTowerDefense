using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuColocarTropas : MonoBehaviour
{
    public TextMeshProUGUI ducados;

    public void OnEnable()
    {
        //ducados.text = GameManager.SharedInstance.GetPlayerMoney.ToString();
    }

    public void Arqueros()
    {
        gameObject.SetActive(false);
        GameManager.SharedInstance.SoldierSelected = GameManager.SharedInstance.prefabArquero;
    }
    public void Infanteria()
    {
        gameObject.SetActive(false);
        GameManager.SharedInstance.SoldierSelected = GameManager.SharedInstance.prefabInfanteria;

    }
    public void Menudas()
    {
        gameObject.SetActive(false);
        GameManager.SharedInstance.SoldierSelected = GameManager.SharedInstance.prefabArtilleriaMenuda;

    }
    public void Gruesas()
    {
        gameObject.SetActive(false);
        GameManager.SharedInstance.SoldierSelected = GameManager.SharedInstance.prefabArtilleriaPesada;

    }
    public void Mozos()
    {
        gameObject.SetActive(false);
        GameManager.SharedInstance.SoldierSelected = GameManager.SharedInstance.prefabMozoMuro;

    }
}
