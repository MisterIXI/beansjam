using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public float CarSpeed = 3f;
    private EventHandler _eH;
    private Vector2 _input;

    void Start()
    {
        _eH = ReferenceHolder.EventHandler;
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _eH.SubscribeToEvent("Steer", SteerCallback);
    }

    private void SteerCallback(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            _input = context.ReadValue<Vector2>();
        }
        if(context.canceled)
        {
            _input = Vector2.zero;
        }
    }


    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * CarSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * _input.x * Time.deltaTime * 100f);
    }
}
