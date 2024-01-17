using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDownMovement : MonoBehaviour
{
    [SerializeField][Range(0,500)]
    private float speed;
    
    [SerializeField][Range(0,0.5f)]
    private float deathZone;
    [SerializeField][Range(-100,200)]
    private float yMin;
    [SerializeField][Range(-100,200)]
    private float yMax;
    private float space;
    private Vector2 ThumbstickPos;

    // Update is called once per frame
    void Update()
    {
        ThumbstickPos = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        
            if (ThumbstickPos.y > deathZone && this.transform.position.y<yMax)
        {
            space = speed * ThumbstickPos.y * Time.deltaTime;
            this.transform.Translate(0,space,0);
        }
            
            if (ThumbstickPos.y < -deathZone && this.transform.position.y>yMin)
            {
                space = speed * ThumbstickPos.y * Time.deltaTime;
                this.transform.Translate(0,space,0);
            }
        
    }
}
