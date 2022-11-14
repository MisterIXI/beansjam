using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeText : MonoBehaviour
{
    private TextMeshPro text;

    private void Start()
    {
        text = gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        text.text = "" + ((int) Time.time / 60) + ":" + (int)(Time.time % 60);
    }
}
