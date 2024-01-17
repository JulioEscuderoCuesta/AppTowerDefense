using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenúAjustes : MonoBehaviour
{
   // public GameObject ModoLibre;
   // public GameObject ModoLibreVertical;
   // public GameObject ModoAnillos;
    public GameObject Player;
    public GameObject PlayerCenterPivot;
    
    public void ModoAnillosButton()
    {
        
        Player.transform.localPosition = new Vector3(0, 0, -3);
        Player.transform.eulerAngles = new Vector3(0, 0, 0);
    }
    public void ModoLibreButton()
    {
      
        Player.transform.localPosition = new Vector3(0, 0, -3);
        Player.transform.eulerAngles = new Vector3(0, 0, 0);
    }
    public void ModoBloqueado()
    {
      
        Player.transform.localPosition = new Vector3(0, 0, 0);
        Player.transform.eulerAngles = new Vector3(0, 0, 0);
    }
    void Update()
    {
        
    }
}
