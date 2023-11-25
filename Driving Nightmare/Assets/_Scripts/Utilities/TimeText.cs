using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeText : MonoBehaviour
{
    private TextMeshPro text;
    private bool RadioRuns= false;
    private float _starttime= 0.0f;
    private void Start()
    {
        text = gameObject.GetComponent<TextMeshPro>();
    }
    private string GetTimeFormat()
    {
        string _text;
        int _minute = (int)((Time.time-_starttime) / 60);
        if (_minute < 10)
        {
            _text = "0" + _minute; 
        }
        else
        {
             _text = "" +  _minute;
        }
        int _second = (int) (Time.time-_starttime) % 60;
        if (_second < 10)
        {
            _text += ":0" + _second; 
        }
        else
        {
             _text += ":" +  _second;
        }
        
        return _text;
    }
    // Update is called once per frame
    public void RadioTimeRun( bool _RadioTimeRuns)
    {
        _starttime = Time.time;
        RadioRuns = _RadioTimeRuns;

    }
   
    void FixedUpdate()
    {   if(RadioRuns)
        {
            text.text = GetTimeFormat();
        }
    }
}
