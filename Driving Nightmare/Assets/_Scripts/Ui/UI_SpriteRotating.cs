using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SpriteRotating : MonoBehaviour
{
    
    public RectTransform Wheel;
    
    public float progressPercent = 0f;
    private float _currAngle = 0f;
    void Update()
    {
        float lastAngle = _currAngle;
        _currAngle = 360f * progressPercent;
        float deltaAngle = _currAngle - lastAngle;
        transform.RotateAround(Wheel.transform.position, Vector3.back, deltaAngle);
    }
}
