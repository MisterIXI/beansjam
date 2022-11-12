using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    private TextMeshProUGUI GUItext;
    
    private void Start() 
    {
        GUItext = GetComponentInChildren<TextMeshProUGUI>();
        ShowDialogText(5, "EPIC NEW DIALOG SCRIPT WORKS");
    }
    IEnumerator WaitSeconds(int _time) {
        {
            yield return new WaitForSeconds(_time);
            GUItext.text = "";
            gameObject.GetComponent<Image>().enabled = false;
            GUItext.enabled=false;
        }
    }
    public void ShowDialogText ( int time, string _text)
    {
        gameObject.GetComponent<Image>().enabled = true;
        GUItext.enabled=true;
        GUItext.text = _text;
        StartCoroutine("WaitSeconds", time);
    }
}

