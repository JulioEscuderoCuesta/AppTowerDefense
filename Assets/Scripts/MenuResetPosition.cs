/*  MenuResetPosition
 Autor: Ivan Sanchez
 Descripcion: Script que reposiciona el menu frente al usuario al presionar el boton start en los controles */

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MenuResetPosition : MonoBehaviour

{
    private float YFinalRot;
    [SerializeField] private GameObject oVRReference;
    
    [SerializeField] [Range(-100,100)] [Tooltip("Distancia del menu al jugador")]
    private float YOffset; //distancia del menu al jugador
    [SerializeField][Range(0,360)]
    private float XAngle;
    [SerializeField][Range(0,360)]
    private float YAngle;
    [SerializeField][Range(0,360)]
    private float ZAngle;
   
    
    
    void Update()
    {
        
       
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
          
            
            this.transform.position = oVRReference.transform.position + new Vector3(0,YOffset,0);
            this.transform.rotation = Quaternion.Euler(XAngle, YAngle, ZAngle);

            this.transform.localEulerAngles = new Vector3(0, oVRReference.transform.eulerAngles.y, 0);
            
            

        }
    }
}
