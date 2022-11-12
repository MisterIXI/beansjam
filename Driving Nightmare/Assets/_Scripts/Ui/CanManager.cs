using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanManager : MonoBehaviour
{
    public int CanUses{get; set;}
    private int Maxuses=3;
    private int tempuse=0;
    public GameObject[] canObjects;
    // Start is called before the first frame update
    void Start()
    {
        CanUses = 3;
        for (int i = 0; i < CanUses; i++)
        {
            canObjects[i].SetActive(true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (tempuse != CanUses)
        {
            for (int i = 0; i < Maxuses; i++)
            {
                if(i <CanUses)
                    canObjects[i].SetActive(true);
                else
                    canObjects[i].SetActive(false);
            }
            tempuse = CanUses;
        }
    }
}
