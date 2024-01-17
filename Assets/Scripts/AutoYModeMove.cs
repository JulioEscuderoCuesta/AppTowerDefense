using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Script que controla el movimiento discreto al rededor del castillo y el movimiento discreto vertical y radial.
public class AutoYModeMove : MonoBehaviour
{
    public GameObject PlayerRef;
    [SerializeField][Range(0,365)]
    private float AngularIncrement;
    
    [SerializeField][Range(0,300)]
    private float RadialIncrement;
    
    [SerializeField][Range(0,500)]
    private float RadioMax;
    
    
    
    [SerializeField][Range(-100,200)]
    private float altura0;
    [SerializeField][Range(-100,200)]
    private float altura1;
    [SerializeField][Range(-100,200)]
    private float altura2;
    [SerializeField][Range(-100,200)]
    private float altura3;
    
    [SerializeField][Range(-100,300)]
    private float Radio1;
    [SerializeField][Range(-100,300)]
    private float radio2;
    [SerializeField][Range(-100,300)]
    private float radio3;
    
    private float space;
    private Vector2 thumbstickRigthPos;
    private Vector2 thumbstickLeftPos;

    private bool ReadyToSnapTurn;
   
    private bool ReadytoRadialMov;

    
    void Update()
    {
        thumbstickRigthPos = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        thumbstickLeftPos = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        
        //-------------Movimiento en vertical Automático----------
        
        if (PlayerRef.transform.localPosition.z > -Radio1)
        {
            this.transform.position= new Vector3(transform.position.x, altura0, transform.position.z);
        }
        
        if (PlayerRef.transform.localPosition.z < -Radio1 && PlayerRef.transform.localPosition.z > -radio2)
        {
            this.transform.position= new Vector3(transform.position.x, altura1, transform.position.z);
        }
        
        if (PlayerRef.transform.localPosition.z < -radio2 && PlayerRef.transform.localPosition.z > -radio3)
        {
            this.transform.position= new Vector3(transform.position.x, altura2, transform.position.z);
        }
        
        if (PlayerRef.transform.localPosition.z < -radio3)
        {
            this.transform.position= new Vector3(transform.position.x, altura3, transform.position.z);
        }

        //-----------Movimiento rotación--------------
        
        
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight)  )
        {
            if (ReadyToSnapTurn)
            {
                ReadyToSnapTurn = false;
                this.transform.Rotate(0,-AngularIncrement,0);
            }
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft)) 
                
        {
            if (ReadyToSnapTurn)
            {
                ReadyToSnapTurn = false;
                this.transform.Rotate(0,AngularIncrement,0);
            }
        }
        else
        {
            ReadyToSnapTurn = true;
        }
        
        //----Movimiento radial Discreto-------
        
        
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp) && PlayerRef.transform.localPosition.z<-0.001f )
        {
            if (ReadytoRadialMov)
            {
                ReadytoRadialMov = false;
                PlayerRef.transform.Translate(0,0,RadialIncrement);
            }
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown)&& PlayerRef.transform.localPosition.z>-RadioMax) 
                
        {
            if (ReadytoRadialMov)
            {
                ReadytoRadialMov = false;
                PlayerRef.transform.Translate(0,0,-RadialIncrement);
            }
        }
        else
        {
            ReadytoRadialMov = true;
        }

        
        
    }
}
