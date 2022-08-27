using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SpeedDisplay : MonoBehaviour
{
    public Image SD_Pause;

    public Image SD_Slow;

    public Image SD_Normal;

    public Image SD_Fast;

    public Image SD_SuperFast;

    private Color SD_PauseColor;

    private Color SD_SlowColor;

    private Color SD_NormalColor;

    private Color SD_FastColor;

    private Color SD_SuperFastColor;

    public void Awake()
    {
        GameSpeedManager.Instance.OnGameSpeedChanged += OnGameSpeedChanged;
    }

    public void Start()
    {
        SD_PauseColor = SD_Pause.color;
        SD_SlowColor = SD_Slow.color;
        SD_NormalColor = SD_Normal.color;
        SD_FastColor = SD_Fast.color;
        SD_SuperFastColor = SD_SuperFast.color;
    }

    public void OnDestroy()
    {
        GameSpeedManager.Instance.OnGameSpeedChanged -= OnGameSpeedChanged;
    }

    public void OnGameSpeedChanged(EGameSpeed newGameSpeed)
    {
        UseSpeedDisplay();
        SpeedController.SwitchGameSpeed(newGameSpeed);
    }

    public void UseSpeedDisplay()
    {
        //get the current game speed
        Image image = null;

        //set the opacity of the image whose name matches the current game speed to 1
        switch (GameSpeedManager.Instance.CurrentGameSpeed)
        {
            case EGameSpeed.PAUSED:
                SD_Pause.color =
                    new Color(SD_PauseColor.r,
                        SD_PauseColor.g,
                        SD_PauseColor.b,
                        1);
                image = SD_Pause;
                break;
            case EGameSpeed.SLOW:
                SD_Slow.color =
                    new Color(SD_SlowColor.r,
                        SD_SlowColor.g,
                        SD_SlowColor.b,
                        1);
                image = SD_Slow;
                break;
            case EGameSpeed.NORMAL:
                SD_Normal.color =
                    new Color(SD_NormalColor.r,
                        SD_NormalColor.g,
                        SD_NormalColor.b,
                        1);
                image = SD_Normal;
                break;
            case EGameSpeed.FAST:
                SD_Fast.color =
                    new Color(SD_FastColor.r,
                        SD_FastColor.g,
                        SD_FastColor.b,
                        1);
                image = SD_Fast;
                break;
            case EGameSpeed.SUPERFAST:
                SD_SuperFast.color =
                    new Color(SD_SuperFastColor.r,
                        SD_SuperFastColor.g,
                        SD_SuperFastColor.b,
                        1);
                image = SD_SuperFast;
                break;
        }

        //set the opacity of all the other images to .5
        foreach (Image otherImage in GetComponentsInChildren<Image>())
        {
            if (otherImage.name != image.name && image != null)
            {
                otherImage.color =
                    new Color(otherImage.color.r,
                        otherImage.color.g,
                        otherImage.color.b,
                        0.5f);
            }
        }
    }
}
