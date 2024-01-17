using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModoAnillosButton : MonoBehaviour
{
    public GameObject PlayerRef;

    public void reset()
    {
        PlayerRef.transform.localPosition = new Vector3(0, 0, -3);
        PlayerRef.transform.eulerAngles = new Vector3(0, 0, 0);
    }
    // Update is called once per frame
    void Update()
    {
       
    }
}
