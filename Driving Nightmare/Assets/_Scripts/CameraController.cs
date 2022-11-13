using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _parent;
    private Vector3 _oldPos;
    private Vector3 _shakeTarget;
    private float _shakeTime;
    private float _shakeDurationFactor;
    [HideInInspector]
    public float TirednessFactor = 0.1f;
    private Vector3 _startPos;

    void Start()
    {
        _shakeTarget = transform.localPosition;
        _parent = transform.parent;
        _shakeDurationFactor = 1f;
        _oldPos = transform.localPosition;
        _startPos = transform.localPosition;
    }

    // private float SmoothProgress(float progress)
    // {
    //     progress = Mathf.Lerp(-Mathf.PI/2,Mathf.PI/2,progress);
    //     progress = Mathf.Sin(progress);
    //     progress = (progress / 2) ;
    //     return progress;
    // }
    private void FixedUpdate()
    {
        // add camera sway
        if (_shakeTime < 1f)
        {
            // lerp to shake target with delta time and magnitude
            _shakeTime += Time.deltaTime;
            transform.localPosition = new Vector3(Mathf.SmoothStep(_oldPos.x, _shakeTarget.x, _shakeTime),
                Mathf.SmoothStep(_oldPos.y, _shakeTarget.y, _shakeTime), Mathf.SmoothStep(_oldPos.z, _shakeTarget.z, _shakeTime));

            // decrease shake time
        }
        else
        {
            // new shake target inside sphere
            _shakeTarget = Random.insideUnitSphere * 0.1f * TirednessFactor + _startPos;
            _shakeTime = 0f;
            _oldPos = transform.localPosition;
            _shakeDurationFactor = 1f + Vector3.Distance(_oldPos, _shakeTarget);
        }
    }
}
