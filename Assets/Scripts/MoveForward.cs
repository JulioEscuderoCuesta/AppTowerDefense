using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Velocidad de movimiento hacia adelante")]
    private float speed;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
        //Ecuación completa. Es lo mismo que lo de arriba.
        //transform.position = transform.position + transform.forward * speed * Time.deltaTime; 
    }
}