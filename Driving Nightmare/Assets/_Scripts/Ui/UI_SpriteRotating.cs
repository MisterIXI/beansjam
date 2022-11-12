using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SpriteRotating : MonoBehaviour
{
    
    public RectTransform Wheel;
    // Start is called before the first frame update
    
    void Update()
    {
        transform.RotateAround(Wheel.transform.position, Vector3.back, 20 * Time.deltaTime);
    }
}
