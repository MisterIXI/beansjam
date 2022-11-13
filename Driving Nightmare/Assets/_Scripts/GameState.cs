using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    enum GameStateEnum
    {
        MainMenu,
        Playing,
        GameOver
    }

    private GameStateEnum _state;
    private CarController _car;
    public GameObject MenuUI;
    public GameObject HudUI;
    public GameObject GameOverUI;
    const float CARS_START_SPEED = 50f;

    private void Start() {
        _state = GameStateEnum.MainMenu;
        _car = GetComponent<CarController>();

    }

    public void On_Start_Click()
    {
        _state = GameStateEnum.Playing;
        _car.CarSpeed = CARS_START_SPEED;
        MenuUI.SetActive(false);
        HudUI.SetActive(true);
    }

    public void On_Credits_Click()
    {
        
    }

    public void On_Quit_Click()
    {
        Application.Quit();
    }
}
