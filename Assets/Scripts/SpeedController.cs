using UnityEngine;

public class SpeedController : MonoBehaviour
{
    public GameSpeed CurrentGameSpeed { get; private set; }

    public GameSpeed PreviousGameSpeed { get; private set; }

    public GameSpeed NewGameSpeed { get; private set; }

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
                CurrentGameSpeed != GameSpeed.PAUSED
                    ? GameSpeed.PAUSED
                    : PreviousGameSpeed;

            GameSpeedManager.Instance.SetGameSpeed (NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            NewGameSpeed = GameSpeed.SLOW;
            GameSpeedManager.Instance.SetGameSpeed (NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            NewGameSpeed = GameSpeed.NORMAL;
            GameSpeedManager.Instance.SetGameSpeed (NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            NewGameSpeed = GameSpeed.FAST;
            GameSpeedManager.Instance.SetGameSpeed (NewGameSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            NewGameSpeed = GameSpeed.SUPERFAST;
            GameSpeedManager.Instance.SetGameSpeed (NewGameSpeed);
        }
    }
}
