using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class SleepManager : MonoBehaviour
{
    private CanManager _canManager;

    public Image sleepPanel;
    public AudioClip CanOpen;
    public AudioClip CanDrink;
    public float Sleep { get; private set; }
    public float CanReduceValue = 1f;
    public float CanDrinkDuration = 1f;
    private float _sleepTick = 0.1f;
    private int whity;
    public bool IsSleeping;
    private bool hit = false; // GLOBAL COLLISION BOOL IF player hits Obstacle
    private float _startedSleeping = 0f;
    private CarController _car;
    ///////////// UI //////////////
    public Slider _slider;
    public Image _image;
    // Start is called before the first frame update
    private void Awake()
    {
        ReferenceHolder.SleepManager = this;
        transform.parent.gameObject.SetActive(false);
    }

    void Start()
    {
        whity = 0;
        Sleep = 0.01f;
        _car = ReferenceHolder.Player.GetComponent<CarController>();
        _canManager = ReferenceHolder.CanManager;
        ReferenceHolder.EventHandler.SubscribeToEvent("EnergyDrink", UseCan);
    }
    private void AdjustFog()
    {
        float baseline = 0.02f;

        float change = Sleep * 0.01f;
        // change = Mathf.Sqrt(change);

        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, baseline + change, 0.4f);
        if (Sleep > 0.9f)
        {
            float colorFactor = (Sleep - 0.9f) / 0.1f;
            sleepPanel.color = new Color(0f, 0f, 0f, colorFactor);
        }
    }
    private void FixedUpdate()
    {
        HandleSleep();
        AdjustFog();
        // Debug.Log("Sleep: " + Sleep + " IsSleeping: " + IsSleeping);
    }
    IEnumerator PulseImageEffect()
    {
        if (Sleep > 0.75f && whity <= 0)
        {
            _image.color = new Color(1, 1, 1);
            whity = 10;
        }
        else
        {
            _image.color = new Color(Sleep, 0, 0);
            whity--;

        }
        yield return new WaitForSeconds(0.01f);



    }
    public void UseCan(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_canManager.gameObject.activeSelf && _canManager.UseCan())
            {
                StartCoroutine("ReduceSleep", CanReduceValue);
                if (IsSleeping)
                {
                    IsSleeping = false;
                    _car.WakeUp();
                }
                AudioSource audioSource = Camera.main.GetComponent<AudioSource>();
                audioSource.PlayOneShot(CanOpen, 0.7f);
                audioSource.PlayOneShot(CanDrink, 1f);
            }
        }
    }

    private void HandleSleep()
    {
        if (Sleep > 1 && IsSleeping == false)
        {   //Debug.Log(" I SLEEP");
            IsSleeping = true;
            _startedSleeping = Time.time;
            SleepEffect();
        }
        else if (Sleep <= 0.7f && IsSleeping == true)
        {
            IsSleeping = false;
            _car.WakeUp();
        }

        else if (hit)
        {   //Debug.Log("HIT");
            Sleep += _sleepTick;
        }
        if (!IsSleeping)
        {

            Sleep += _sleepTick * Time.deltaTime;
        }
        if (IsSleeping)
        {
            if (Time.time - _startedSleeping > 1f)
            {
                Sleep -= _sleepTick * 15 * Time.deltaTime;
            }
        }
        // if obstacles hit increase sleep
        // else sleep + Deltatime
        _slider.value = Sleep;
        StartCoroutine("PulseImageEffect");
    }

    IEnumerator ReduceSleep(float value)
    {
        float sleepDuration = CanDrinkDuration / 100f;
        float sleepStep = value / 100f;
        for (int i = 0; i < 100; i++)
        {
            if (Sleep - _sleepTick > 0)
                Sleep -= sleepStep;
            yield return new WaitForSeconds(sleepDuration);
        }
    }
    private void SleepEffect()
    {

        // for seconds hard Darkness and no Control
        _car.OnSleep();

    }

}
