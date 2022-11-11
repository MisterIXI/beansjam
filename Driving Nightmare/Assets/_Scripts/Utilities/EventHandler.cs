using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class EventHandler : MonoBehaviour
{
    private InputActionMap _actionMap;
    // Start is called before the first frame update
    private void Awake() {
        _actionMap = GetComponent<PlayerInput>().currentActionMap;
    }
    void Start()
    {
    }

    public void SubscribeToEvent(string ActionName, System.Action<InputAction.CallbackContext> callback)
    {
        InputAction action = _actionMap.FindAction(ActionName,true);
        action.started += callback;
        action.performed += callback;
        action.canceled += callback;
    }

    public void UnsubscribeFromEvent(string ActionName, System.Action<InputAction.CallbackContext> callback)
    {
        InputAction action = _actionMap.FindAction(ActionName,true);
        action.started -= callback;
        action.performed -= callback;
        action.canceled -= callback;
    }
    
}
