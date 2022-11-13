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
    public GameObject CreditsUI;
    public GameObject HudUI;
    public GameObject WinObject;
    public GameObject LoseObject;
    public GameObject GameOverUI;
    [HideInInspector]
    public SleepManager SleepManager;
    [HideInInspector]
    public CanManager CanManager;
    [HideInInspector]
    public UI_SpriteRotating CarProgressRotator;
    [HideInInspector]
    public UI_Sprite_Animation CarAnimation;

    const float CARS_START_SPEED = 150f;
    private void Awake() {
        SleepManager = HudUI.GetComponentInChildren<SleepManager>();
        CanManager = HudUI.GetComponentInChildren<CanManager>();
        CarProgressRotator = HudUI.GetComponentInChildren<UI_SpriteRotating>();
        CarAnimation = HudUI.GetComponentInChildren<UI_Sprite_Animation>();
    }
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

    public void Win()
    {
        _state = GameStateEnum.GameOver;
        HudUI.SetActive(false);
        GameOverUI.SetActive(true);
        WinObject.SetActive(true);
        _car.StopCar();
    }

    public void Lose()
    {
        _state = GameStateEnum.GameOver;
        _car.CarSpeed = 0f;
        HudUI.SetActive(false);
        GameOverUI.SetActive(true);
        LoseObject.SetActive(true);
        _car.StopCar();
    }

    public void On_Restart_Click()
    {
        // reload scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void On_Credits_Click()
    {
        CreditsUI.SetActive(true);
    }

    public void On_CreditsBack_Click()
    {
        CreditsUI.SetActive(false);
    }

    public void On_Quit_Click()
    {
        Application.Quit();
    }
}
