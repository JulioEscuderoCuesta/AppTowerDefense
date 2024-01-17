using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class AccionesMenuPrincipal : MonoBehaviour
{
    public GameObject Menu;
    public GameObject MenuTropas;
    public GameObject MenuTropasMinimizado;
    public GameObject MenuMejoras;
    public GameObject MenuAjustes;
    public GameObject MenuSalir;
    
    public TextMeshProUGUI ducados;
    public TextMeshProUGUI oleada;

    

    public void OnEnable()
    {
        /*ducados.text = GameManager.SharedInstance.GetPlayerMoney.ToString();
        oleada.text = GameManager.SharedInstance.GetNextWave.ToString();*/
    }
    
    public void tropas()
    {
        
        Menu.SetActive(false);
        MenuTropas.SetActive(true);
        MenuTropasMinimizado.SetActive(false);
        MenuMejoras.SetActive(false);
        MenuAjustes.SetActive(false);
        MenuSalir.SetActive(false);
        
    }
    public void tropasMin()
    {
        Menu.SetActive(false);
        MenuTropas.SetActive(false);
        MenuTropasMinimizado.SetActive(true);
        MenuMejoras.SetActive(false);
        MenuAjustes.SetActive(false);
        MenuSalir.SetActive(false);
        
    }
    public void Mejoras()
    {
        Menu.SetActive(false);
        MenuTropas.SetActive(false);
        MenuTropasMinimizado.SetActive(false);
        MenuMejoras.SetActive(true);
        MenuAjustes.SetActive(false);
        MenuSalir.SetActive(false);
    }
    public void Ajustes()
    {
        Menu.SetActive(false);
        MenuTropas.SetActive(false);
        MenuTropasMinimizado.SetActive(false);
        MenuMejoras.SetActive(false);
        MenuAjustes.SetActive(true);
        MenuSalir.SetActive(false);
    }
    public void Salir()
    {
        Menu.SetActive(false);
        MenuTropas.SetActive(false);
        MenuTropasMinimizado.SetActive(false);
        MenuMejoras.SetActive(false);
        MenuAjustes.SetActive(false);
        MenuSalir.SetActive(true);
    }
    public void VolverAMenu()
    {
        Menu.SetActive(true);
        MenuTropas.SetActive(false);
        MenuTropasMinimizado.SetActive(false);
        MenuMejoras.SetActive(false);
        MenuAjustes.SetActive(false);
        MenuSalir.SetActive(false);
    }

    public void CargarSiguienteOleada()
    {
        Menu.SetActive(false);
        GameManager.SharedInstance.BattleStage();
    }

    public void exitToInicio()
    {
        SceneManager.LoadScene("Escena Inicio", LoadSceneMode.Single);
    }
    void Update()
    {
        
    }
}
