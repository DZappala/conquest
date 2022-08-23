using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateDisplay : MonoBehaviour
{
    public TMP_Text DP_Date;

    public void Awake()
    {
        Time.fixedDeltaTime = 604800f; //hack is set to a literal week in seconds between ticks because for some reason 0 is not a valid value for fixedDeltaTime (well it's valid but it makes the game run at infinitely fast speed) Can't use Time.timeScale = 0 because it will make everything run at 0 speed and not just the date and time

        GameSpeedManager.Instance.OnGameSpeedChanged += OnGameSpeedChanged;
    }

    public void Start()
    {
        GameTime.daysTotal = 1; //keeps track of total days
        GameTime.days = 1; //keeps track of days in month
        GameTime.monthsTotal = 11; //keeps track of total months
        GameTime.month = 0; //keeps track of month
        GameTime.year = 1500; //keeps track of year
        GameTime.monthNames =
            new List<string> {
                "January",
                "February",
                "March",
                "April",
                "May",
                "June",
                "July",
                "August",
                "September",
                "October",
                "November",
                "December"
            }; //list of month names
        GameTime.currentMonth = GameTime.monthNames[GameTime.month];

        GameTime.HandleTime();
        DP_Date.text = GameTime.Date;
    }

    public void FixedUpdate()
    {
        if (
            Database.isRun &&
            GameSpeedManager.Instance.CurrentGameSpeed != GameSpeed.PAUSED
        )
        {
            DP_Date.text = GameTime.Date;
            GameTime.daysTotal++;
            GameTime.days++;
            GameTime.HandleTime();
        }
    }

    public void OnDestroy()
    {
        GameSpeedManager.Instance.OnGameSpeedChanged -= OnGameSpeedChanged;
    }

    private void OnGameSpeedChanged(GameSpeed newGameSpeed)
    {
        switch (newGameSpeed)
        {
            case GameSpeed.PAUSED:
                Time.fixedDeltaTime = 604800;
                break;
            case GameSpeed.SLOW:
                Time.fixedDeltaTime = 2;
                break;
            case GameSpeed.NORMAL:
                Time.fixedDeltaTime = 1;
                break;
            case GameSpeed.FAST:
                Time.fixedDeltaTime = 0.5f;
                break;
            case GameSpeed.SUPERFAST:
                Time.fixedDeltaTime = 0.25f;
                break;
            default:
                break;
        }
    }
}
