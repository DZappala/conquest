using System;
using UnityEngine;

public class SpeedController : MonoBehaviour
{
    private EGameSpeed CurrentGameSpeed { get; set; }

    private EGameSpeed PreviousGameSpeed { get; set; }

    private EGameSpeed NewGameSpeed { get; set; }

    public void Update()
    {
        SpeedInputHandler();
    }

    private void SpeedInputHandler()
    {
        CurrentGameSpeed = GameSpeedManager.Instance.CurrentGameSpeed;
        PreviousGameSpeed = GameSpeedManager.Instance.PreviousGameSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            NewGameSpeed =
                CurrentGameSpeed != EGameSpeed.Paused
                    ? EGameSpeed.Paused
                    : PreviousGameSpeed;

            GameSpeedManager.Instance.SetGameSpeed(NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            NewGameSpeed = EGameSpeed.Slow;
            GameSpeedManager.Instance.SetGameSpeed(NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            NewGameSpeed = EGameSpeed.Normal;
            GameSpeedManager.Instance.SetGameSpeed(NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            NewGameSpeed = EGameSpeed.Fast;
            GameSpeedManager.Instance.SetGameSpeed(NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            NewGameSpeed = EGameSpeed.Superfast;
            GameSpeedManager.Instance.SetGameSpeed(NewGameSpeed);
        }
    }

    //TODO integrate the SpeedDisplay function into this one, they use the same switch statement
    public static void SwitchGameSpeed(EGameSpeed newGameSpeed)
    {
        GameControl.DelayInSeconds = newGameSpeed switch
        {
            EGameSpeed.Paused => 0,
            EGameSpeed.Slow => 3.5f,
            EGameSpeed.Normal => 1f,
            EGameSpeed.Fast => 0.5f,
            EGameSpeed.Superfast => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(newGameSpeed), newGameSpeed, null)
        };
    }
}
