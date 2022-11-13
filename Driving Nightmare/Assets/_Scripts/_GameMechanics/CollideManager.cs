using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideManager : MonoBehaviour
{
    public bool isColide {get;private set;}
    private int CollideThreshold = 10;
    private int _collids;
    // Start is called before the first frame update
    void Start()
    {
        isColide = false;
        _collids = 0;
    }
    private void CollideHandle()
    {
        _collids ++;
        if(_collids > CollideThreshold)
            Debug.Log("GAMEOVER");
            _collids =0;
    }
    // Update is called once per frame
    private void OnTriggerEnter(Collider other) {
        Debug.Log(" COLLIDE WITH " + other.name);
        CollideHandle();
    }
}
