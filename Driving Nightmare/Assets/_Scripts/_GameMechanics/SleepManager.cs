using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SleepManager : MonoBehaviour
{
    [SerializeField]private CanManager canManager;


    
    public float Sleep {get; private set;}
    private float _sleepTick= 0.005f;
    public bool usesCan {get;set;}
    private int whity;
    private bool isSleeping;
    private bool hit = false; // GLOBAL COLLISION BOOL IF player hits Obstacle
    ///////////// UI //////////////
    public Slider _slider;
    public Image _image;
    // Start is called before the first frame update
    void Start()
    {
        whity = 0;
        Sleep = 0.01f;
        usesCan = false;
    }
    private void FixedUpdate() {
        HandleSleep();
        
    }
    IEnumerator PulseImageEffect()
    {
            if(Sleep>  0.75f && whity <= 0)
            {   
                _image.color = new Color(1, 1,1);
                whity=10;
            }
            else{
                _image.color = new Color(Sleep, 0,0);
                whity--;

            }
            yield return new WaitForSeconds(0.01f);
        
        
        
    }
    private void HandleSleep()
    {
        if(Sleep >1 && isSleeping == false)
        {   //Debug.Log(" I SLEEP");
            isSleeping =true;
            SleepEffect();
        }
        else if (Sleep <=0 && isSleeping == true)
        {
            isSleeping=false;
        }
        if(usesCan)
        {
           // Debug.Log("Using EnergyDrink");
            usesCan = false;
            StartCoroutine("ReduceSleep",1);
        }
        else if(hit)
        {   //Debug.Log("HIT");
            Sleep += _sleepTick;
        }
        if(!isSleeping)
        {
            
            Sleep += _sleepTick;
        }
        if(isSleeping)
        {
            Sleep-= _sleepTick;
        }
        // if obstacles hit increase sleep
        // else sleep + Deltatime
        _slider.value = Sleep;
        StartCoroutine("PulseImageEffect");
    }
 
    IEnumerator ReduceSleep(int value)
    {
        for (int i = 0; i < 1000; i++)
        {
            if(Sleep-_sleepTick >0)
                Sleep -= _sleepTick;
            yield return new WaitForSeconds(0.01f);
        }
    }
    private void SleepEffect()
    {
        
        // for seconds hard Darkness and no Control
        
        
    }
    
}
