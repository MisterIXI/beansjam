using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Cuby_Rotation : MonoBehaviour
{
    // Start is called before the first frame update
    

    // Update is called once per frame
    void FixedUpdate()
    {
            transform.RotateAround(transform.position, transform.forward, Time.deltaTime * 90f);
    }
}
