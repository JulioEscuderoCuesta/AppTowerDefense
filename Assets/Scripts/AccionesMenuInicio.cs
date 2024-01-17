using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccionesMenuInicio : MonoBehaviour
{
    

    public GameObject menuControllers;
    public GameObject menuAjustes;
    public GameObject menuInicio;

    public void OnInicioPress()
    {
        SceneManager.LoadScene("Escena Principal", LoadSceneMode.Single);
        
    }
   public void ButtonControllers()
    {
        if (menuControllers.activeSelf == false)
        {
            menuControllers.SetActive(true);  
        }
        else {menuControllers.SetActive(false);}
        
    }
    public void ButtonAjustes()
    {
        if (menuAjustes.activeSelf == false)
        {
            menuAjustes.SetActive(true);  
        }
        else {menuAjustes.SetActive(false);}
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
