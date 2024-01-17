using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autodestroy : MonoBehaviour
{
    [SerializeField] [Tooltip("Tiempo después del cual se destruye el objeto")]
    private float hideDelay;
    void OnEnable()
    {
        //Destroy(gameObject, destructionDelay);
        Invoke(nameof(HideObject), hideDelay);
    }

    private void HideObject()
    {
        gameObject.SetActive(false);
    }

}