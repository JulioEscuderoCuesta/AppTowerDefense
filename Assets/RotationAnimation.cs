using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnimation : MonoBehaviour

{

    public float zspeed;
    private float zangle;
    
    void Update()
    {
        zangle = zspeed * Time.deltaTime;
     this.transform.Rotate(0,0,zangle);   
    }
}
