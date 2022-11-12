using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Sprite_Animation : MonoBehaviour
{
    public Image image;
    public Sprite[] spriteArray;
    float animationSpeed =0.33f;
    private int index;
    bool isPlaying = false;
    // Update is called once per frame
    IEnumerator PlayAnimUI()
    {
        isPlaying = true;
        yield return new WaitForSeconds(animationSpeed);

        if(index >= spriteArray.Length)
            index = 0;
        
        image.sprite = spriteArray[index];
        index ++;
        isPlaying =false;
    }
    void FixedUpdate()
    {
        if(isPlaying== false )
        {
            StartCoroutine("PlayAnimUI");
        }
        
    }
}
