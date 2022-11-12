using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public GameObject StreetSpawner;
    public float GroundLevel = 5f;
    public float CarSpeed = 50f;
    public float SteerStrength = 20f;
    public float raycast_offset = 5f;

    private StreetCreator _sc;
    private Rigidbody _rb;
    private EventHandler _eH;
    private float _steerInput;
    private float _gasInput;

    void Start()
    {
        _eH = ReferenceHolder.EventHandler;
        _sc = StreetSpawner.GetComponent<StreetCreator>();
        _rb = GetComponent<Rigidbody>();
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _eH.SubscribeToEvent("Steer", SteerCallback);
        _eH.SubscribeToEvent("Gas", GasCallback);
    }

    private void SteerCallback(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            _steerInput = context.ReadValue<float>();
        }
        if(context.canceled)
        {
            _steerInput = 0f;
        }
    }

    private void GasCallback(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            _gasInput = context.ReadValue<float>();
            _sc.ScrollSpeed = _gasInput * CarSpeed;
        }
        if(context.canceled)
        {
            _gasInput = 0.5f;
            _sc.ScrollSpeed = 0.5f;
        }
    }


    void FixedUpdate()
    {
        // transform.Translate(Vector3.forward * CarSpeed * Time.deltaTime);
        // _rb.velocity = new Vector3(transform.forward.x * CarSpeed * _gasInput, _rb.velocity.y, transform.forward.z * CarSpeed * _gasInput);
        // lerp vertical position to street height with raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position - (transform.up * -1f), Vector3.down, out hit, 100f))
        {
            Debug.Log("Hit: " + hit.point.y);
            float targetHeight = hit.point.y + GroundLevel;
            // float currHeight = transform.position.y;
            // float newHeight = Mathf.Lerp(currHeight, targetHeight, 0.5f);
            _rb.MovePosition(new Vector3(transform.position.x, targetHeight, transform.position.z));
            // transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
        }
        // shoot two raycasts to adjust rotation
        RaycastHit hitFront;
        RaycastHit hitBack;
        if (Physics.Raycast(transform.position + transform.forward * raycast_offset,Vector3.down, out hitFront, 50f))
        {
            if (Physics.Raycast(transform.position - transform.forward * raycast_offset, Vector3.down, out hitBack, 50f))
            {
                Vector3 targetDir = hitFront.point - hitBack.point;
                // targetDir.y = 0f;
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                _rb.MoveRotation(Quaternion.Lerp(transform.rotation, targetRot, 0.5f));
            }
        }
        Debug.Log("Velocity: " + _rb.velocity);
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        Vector3 newPos = transform.position + transform.forward * CarSpeed * _gasInput * Time.deltaTime;
        newPos.z = 0f;
        _rb.MovePosition(newPos);
        transform.Rotate(Vector3.up * _steerInput * Time.deltaTime * SteerStrength);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Vector3 posF = transform.position + transform.forward * raycast_offset;
        Vector3 posB = transform.position - transform.forward* raycast_offset;
        Gizmos.DrawSphere(posF , 0.5f);
        Gizmos.DrawRay(posF, posF + transform.up * -50f);
        Gizmos.DrawSphere(posB , 0.5f);
        Gizmos.DrawRay(posB, posB + transform.up * -50f);


    }
}
