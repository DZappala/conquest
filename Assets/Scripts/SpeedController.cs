using UnityEngine;

public class SpeedController : MonoBehaviour
{
    public EGameSpeed CurrentGameSpeed { get; private set; }

    public EGameSpeed PreviousGameSpeed { get; private set; }

    public EGameSpeed NewGameSpeed { get; private set; }

    public void Update()
    {
        SpeedInputHandler();
    }

    public void SpeedInputHandler()
    {
        CurrentGameSpeed = GameSpeedManager.Instance.CurrentGameSpeed;
        PreviousGameSpeed = GameSpeedManager.Instance.PreviousGameSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            NewGameSpeed =
                CurrentGameSpeed != EGameSpeed.PAUSED
                    ? EGameSpeed.PAUSED
                    : PreviousGameSpeed;

            GameSpeedManager.Instance.SetGameSpeed (NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            NewGameSpeed = EGameSpeed.SLOW;
            GameSpeedManager.Instance.SetGameSpeed (NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            NewGameSpeed = EGameSpeed.NORMAL;
            GameSpeedManager.Instance.SetGameSpeed (NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            NewGameSpeed = EGameSpeed.FAST;
            GameSpeedManager.Instance.SetGameSpeed (NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            NewGameSpeed = EGameSpeed.SUPERFAST;
            GameSpeedManager.Instance.SetGameSpeed (NewGameSpeed);
        }
    }

    public static void SwitchGameSpeed(EGameSpeed newGameSpeed)
    {
        switch (newGameSpeed)
        {
            case EGameSpeed.PAUSED:
                GameControl.DelayInSeconds = 0;
                break;
            case EGameSpeed.SLOW:
                GameControl.DelayInSeconds = 5;
                break;
            case EGameSpeed.NORMAL:
                GameControl.DelayInSeconds = 3.5f;
                break;
            case EGameSpeed.FAST:
                GameControl.DelayInSeconds = 1.75f;
                break;
            case EGameSpeed.SUPERFAST:
                GameControl.DelayInSeconds = 0.5f;
                break;
        }
    }
}
