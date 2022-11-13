using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public long FinishLine = 10;

    public float FogDensity = 0.02f;
    const float DEFAULT_SPEED = 0.75f;
    const float MIN_SPEED = 0.4f;
    public AudioClip CrashSound;
    public GameObject StreetSpawner;
    public Transform SteeringWheel;
    public float GroundLevel = 5f;
    public float CarSpeed = 0f;
    public float SteerStrength = 20f;
    public float raycast_offset = 5f;
    public float maxAngle = 80f;
    private StreetCreator _sc;
    private Rigidbody _rb;
    private EventHandler _eH;
    private float _steerInput;
    private float _actualSteer;
    private float _gasInput;
    private float _actualSpeed;
    private float _finishProgress;
    private UI_SpriteRotating _progressRotator;
    private SleepManager _sleepManager;
    private AudioSource _audioSource;
    void Start()
    {
        _eH = ReferenceHolder.EventHandler;
        _sc = StreetSpawner.GetComponent<StreetCreator>();
        _rb = GetComponent<Rigidbody>();
        SubscribeToEvents();
        _gasInput = DEFAULT_SPEED;
        _sc.ScrollSpeed = _gasInput * CarSpeed;
        _finishProgress = 0f;
        _progressRotator = ReferenceHolder.GameState.CarProgressRotator;
        _audioSource = GetComponent<AudioSource>();
        _sleepManager = ReferenceHolder.SleepManager;
    }

    private void SubscribeToEvents()
    {
        _eH.SubscribeToEvent("Steer", SteerCallback);
        _eH.SubscribeToEvent("Gas", GasCallback);
        _eH.SubscribeToEvent("DebugButton", DebugCallback);
    }

    private void DebugCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RenderSettings.fogDensity = FogDensity;
            RenderSettings.fogColor = new Color(Random.Range(0f, 0.1f), 0f, 0f, 1f);
        }
    }
    private float _desiredSteer = 0f;
    private void SteerCallback(InputAction.CallbackContext context)
    {
        if (!_sleepManager.IsSleeping)
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
        else
        {
            if (context.performed)
            {
                _desiredSteer = context.ReadValue<float>();
            }
            if (context.canceled)
            {
                _desiredSteer = 0f;
            }
        }

    }
    public void StopCar()
    {
        CarSpeed = 0f;
        _actualSpeed = 0f;
        _sc.ScrollSpeed = 0f;
        _audioSource.Stop();
        Time.timeScale = 0f;
    }
    public void OnSleep()
    {
        _steerInput = 0f;
        // _gasInput = DEFAULT_SPEED;
    }
    private float _desiredGas = DEFAULT_SPEED;
    private void GasCallback(InputAction.CallbackContext context)
    {
        if (!_sleepManager.IsSleeping)
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
        else
        {
            if (context.performed)
            {
                _desiredGas = context.ReadValue<float>();
                if (_desiredGas < MIN_SPEED)
                {
                    _desiredGas = MIN_SPEED;
                }
            }
            if (context.canceled)
            {
                _desiredGas = DEFAULT_SPEED;
            }
        }
    }

    public void WakeUp()
    {
        // _steerInput = _desiredSteer;
        // _gasInput = _desiredGas;
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
    float progressPerSecond = 0f;
    float lastTime;
    int totalCounts = 0;
    private void FixedUpdate()
    {
        // lerp speed to desired input
        _actualSpeed = Mathf.Lerp(_actualSpeed, _gasInput * CarSpeed, Time.deltaTime);
        _actualSteer = Mathf.Lerp(_actualSteer, _steerInput * SteerStrength, Time.deltaTime);
        _sc.ScrollSpeed = _actualSpeed;

        _finishProgress += _actualSpeed;
        // Debug.Log("Progress: " + _finishProgress + " FinishLine: " + FinishLine* 10 + " ProgressPerSecond: " + _finishProgress/Time.time);
        _progressRotator.progressPercent = _finishProgress / FinishLine / 10;
        if (_finishProgress >= FinishLine * 10)
        {
            GetComponent<GameState>().Win();
        }
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
                float angle = _actualSteer * Time.deltaTime;
                float eulerAngle = transform.rotation.eulerAngles.y + maxAngle;
                float combination = Mathf.Abs(angle + eulerAngle);
                combination %= 360f;
                // Debug.Log("Angle: " + angle + " EulerAngles: " + eulerAngle + " Combination: " + combination);
                if (combination > maxAngle * 2)
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
        LerpSteeringWheel();
        SoundChange();
    }

    private float _wheelAngle = 0f;
    private void LerpSteeringWheel()
    {
        float change = _actualSteer - Mathf.Lerp(_wheelAngle, _actualSteer, Time.deltaTime);
        _wheelAngle += change;
        // lerp rotate steering wheel
        // SteeringWheel.eulerAngles
        SteeringWheel.RotateAround(SteeringWheel.position, SteeringWheel.up, change * 3f);
    }
    private void SoundChange()
    {
        if (CarSpeed == 0)
        {
            if (_audioSource.isPlaying)
                _audioSource.Pause();
        }
        else
        {
            if (!_audioSource.isPlaying)
                _audioSource.Play();
            _audioSource.pitch = (_actualSpeed / (CarSpeed * DEFAULT_SPEED)) * 0.5f;
            // Debug.Log(_audioSource.pitch);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            Debug.Log("Trigger with obstacle");
            GetComponent<GameState>().Lose();
            Camera.main.GetComponent<AudioSource>().PlayOneShot(CrashSound, 0.6f);
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
