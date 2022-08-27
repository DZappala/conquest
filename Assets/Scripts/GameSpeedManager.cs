public class GameSpeedManager
{
    private static GameSpeedManager _instance;

    public static GameSpeedManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameSpeedManager();
            }
            return _instance;
        }
    }

    public EGameSpeed CurrentGameSpeed { get; private set; }

    public EGameSpeed PreviousGameSpeed { get; private set; }

    public delegate void UseGameSpeed(EGameSpeed newGameSpeed);

    public event UseGameSpeed OnGameSpeedChanged;

    private GameSpeedManager()
    {
        CurrentGameSpeed = EGameSpeed.PAUSED;
        PreviousGameSpeed = EGameSpeed.PAUSED;
    }

    public void SetGameSpeed(EGameSpeed newGameSpeed)
    {
        if (newGameSpeed == CurrentGameSpeed)
        {
            return;
        }

        PreviousGameSpeed = CurrentGameSpeed;
        CurrentGameSpeed = newGameSpeed;
        OnGameSpeedChanged?.Invoke(newGameSpeed);
    }
}
