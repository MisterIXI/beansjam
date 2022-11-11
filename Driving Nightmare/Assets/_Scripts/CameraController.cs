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
    public float TirednessFactor = 1f;

    void Start()
    {
        _shakeTarget = new Vector3(0, 0, 0);
        _parent = transform.parent;
        _shakeDurationFactor = 1f;
        _oldPos = transform.localPosition;
    }

    private float SmoothProgress(float progress)
    {
        progress = Mathf.Lerp(-Mathf.PI/2,Mathf.PI/2,progress);
        progress = Mathf.Sin(progress);
        progress = (progress / 2) + 0.5f;
        return progress;
    }
    private void FixedUpdate()
    {
        // add camera sway
        if (_shakeTime < 1f)
        {
            // lerp to shake target with delta time and magnitude
            _shakeTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(_oldPos, _shakeTarget, SmoothProgress(_shakeTime/_shakeDurationFactor));

            // decrease shake time
        }
        else
        {
            // new shake target inside sphere
            _shakeTarget = Random.insideUnitSphere * 0.3f;
            _shakeTime = 0f;
            _oldPos = transform.localPosition;
            _shakeDurationFactor = 1f + Vector3.Distance(_oldPos, _shakeTarget);
        }
    }
}
