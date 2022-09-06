public class GameSpeedManager
{
    private static GameSpeedManager _instance;

    public static GameSpeedManager Instance
    {
        get { return _instance ??= new GameSpeedManager(); }
    }

    public EGameSpeed CurrentGameSpeed { get; private set; }

    public EGameSpeed PreviousGameSpeed { get; private set; }

    public delegate void UseGameSpeed(EGameSpeed newGameSpeed);

    public event UseGameSpeed OnGameSpeedChanged;

    private GameSpeedManager()
    {
        CurrentGameSpeed = EGameSpeed.Paused;
        PreviousGameSpeed = EGameSpeed.Paused;
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
