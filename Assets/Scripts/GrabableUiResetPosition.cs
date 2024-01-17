/*  Grababbable UI ResetPosition
 Autor: Ivan Sanchez
 Descripcion: Script que reposiciona el menu frente al usuario (tras haberlo reposicionado manualmente agarrandolo) al presionar el boton start en los controles */
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GrabableUiResetPosition : MonoBehaviour

{
   [Range(0,200)]
    public float FrontMenuOffset;
    [SerializeField]
    private float RotationX;
    [SerializeField]
    private float RotationY;
    [SerializeField]
    private float RotationZ;
    // Update is called once per frame
    void Update()
    {
       // transform.rotation= Quaternion.Euler(0,0,0);
        if (OVRInput.Get(OVRInput.Button.Start))
        {
            this.transform.localPosition = new Vector3(0, 0,FrontMenuOffset);
            transform.rotation = new Quaternion(RotationX,RotationY ,RotationZ,RotationX );

        }  
    }
}
