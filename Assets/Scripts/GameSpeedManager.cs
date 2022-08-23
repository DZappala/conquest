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

    public GameSpeed CurrentGameSpeed { get; private set; }

    public GameSpeed PreviousGameSpeed { get; private set; }

    public delegate void UseGameSpeed(GameSpeed newGameSpeed);

    public event UseGameSpeed OnGameSpeedChanged;

    private GameSpeedManager()
    {
        CurrentGameSpeed = GameSpeed.PAUSED;
        PreviousGameSpeed = GameSpeed.PAUSED;
    }

    public void SetGameSpeed(GameSpeed newGameSpeed)
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
