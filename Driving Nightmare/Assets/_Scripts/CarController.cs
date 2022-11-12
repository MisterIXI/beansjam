using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    const float DEFAULT_SPEED = 0.5f;
    const float MIN_SPEED = 0.2f;
    public GameObject StreetSpawner;
    public float GroundLevel = 5f;
    public float CarSpeed = 50f;
    public float SteerStrength = 20f;
    public float raycast_offset = 5f;
    public float maxAngle = 80f;
    private StreetCreator _sc;
    private Rigidbody _rb;
    private EventHandler _eH;
    private float _steerInput;
    private float _gasInput;
    private float _actualSpeed;

    void Start()
    {
        _eH = ReferenceHolder.EventHandler;
        _sc = StreetSpawner.GetComponent<StreetCreator>();
        _rb = GetComponent<Rigidbody>();
        SubscribeToEvents();
        _gasInput = DEFAULT_SPEED;
        _sc.ScrollSpeed = _gasInput * CarSpeed;

    }

    private void SubscribeToEvents()
    {
        _eH.SubscribeToEvent("Steer", SteerCallback);
        _eH.SubscribeToEvent("Gas", GasCallback);
    }

    private void SteerCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _steerInput = context.ReadValue<float>();
        }
        if (context.canceled)
        {
            _steerInput = 0f;
        }
    }

    private void GasCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _gasInput = context.ReadValue<float>();
            if (_gasInput < MIN_SPEED)
            {
                _gasInput = MIN_SPEED;
            }
        }
        if (context.canceled)
        {
            _gasInput = DEFAULT_SPEED;
        }
    }


    private void Update()
    {


        RaycastHit hit;
        // raycast straight down to street
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 50f))
        {
            float height = hit.point.y;
            // Debug.Log("Height: " + height + " GroundLevel: " + GroundLevel);
            // if street is below ground level, move car up
            Vector3 lerpedPos = Vector3.Lerp(transform.position, new Vector3(transform.position.x, height + GroundLevel, transform.position.z), 0.5f);
            transform.position = lerpedPos;
        }
    }

    private void FixedUpdate()
    {
        // lerp speed to desired input
        _actualSpeed = Mathf.Lerp(_actualSpeed, _gasInput * CarSpeed, Time.deltaTime);
        _sc.ScrollSpeed = _actualSpeed;
        // sample two points on the street with raycasts front and back with offset
        Vector3 front = transform.position + transform.forward * raycast_offset;
        Vector3 back = transform.position - transform.forward * raycast_offset;
        RaycastHit hitFront;
        RaycastHit hitBack;

        if (Physics.Raycast(front, Vector3.down, out hitFront, 50f))
        {
            if (Physics.Raycast(back, Vector3.down, out hitBack, 50f))
            {
                Vector3 direction = hitFront.point - hitBack.point;
                // direction += Vector3.right * _steerInput * SteerStrength;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.5f);
                float angle = _steerInput * SteerStrength * Time.deltaTime;
                float eulerAngle = transform.rotation.eulerAngles.y + maxAngle;
                float combination = Mathf.Abs(angle + eulerAngle);
                combination %= 360f;
                Debug.Log("Angle: " + angle + " EulerAngles: " + eulerAngle + " Combination: " + combination);
                if(combination > maxAngle*2)
                {
                    angle = 0f;
                }

                transform.RotateAround(transform.position, Vector3.up, angle);
                // get speed forwards
                Vector3 moveVec = transform.forward * _actualSpeed * Time.deltaTime;
                moveVec.z = 0f;
                transform.Translate(moveVec, Space.World);
                _rb.MovePosition(moveVec);

            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 posF = transform.position + transform.forward * raycast_offset;
        Vector3 posB = transform.position - transform.forward * raycast_offset;
        Gizmos.DrawSphere(posF, 0.5f);
        Gizmos.DrawRay(posF, posF + transform.up * -50f);
        Gizmos.DrawSphere(posB, 0.5f);
        Gizmos.DrawRay(posB, posB + transform.up * -50f);


    }
}
