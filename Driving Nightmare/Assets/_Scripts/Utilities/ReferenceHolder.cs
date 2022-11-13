using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceHolder : MonoBehaviour
{
    public static GameObject Player;
    public static EventHandler EventHandler;
    public static GameState GameState;
    public static SleepManager SleepManager;
    // Start is called before the first frame update
    private void Awake() {
        Player = gameObject;
        EventHandler = GetComponent<EventHandler>();
        GameState = GetComponent<GameState>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
