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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentGameSpeed = GameSpeedManager.Instance.CurrentGameSpeed;
            PreviousGameSpeed = GameSpeedManager.Instance.PreviousGameSpeed;

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

        switch (GameSpeedManager.Instance.CurrentGameSpeed)
        {
            case EGameSpeed.PAUSED:
                Debug.Log("Game is paused");
                break;
            case EGameSpeed.SLOW:
                Debug.Log("Game is slow");
                break;
            case EGameSpeed.NORMAL:
                Debug.Log("Game is normal");
                break;
            case EGameSpeed.FAST:
                Debug.Log("Game is fast");
                break;
            case EGameSpeed.SUPERFAST:
                Debug.Log("Game is superfast");
                break;
        }
    }
}
