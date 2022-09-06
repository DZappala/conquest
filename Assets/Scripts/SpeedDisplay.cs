using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SpeedDisplay : MonoBehaviour
{
    [FormerlySerializedAs("SD_Pause")] public Image sdPause;

    [FormerlySerializedAs("SD_Slow")] public Image sdSlow;

    [FormerlySerializedAs("SD_Normal")] public Image sdNormal;

    [FormerlySerializedAs("SD_Fast")] public Image sdFast;

    [FormerlySerializedAs("SD_SuperFast")] public Image sdSuperFast;

    private Color _sdPauseColor;

    private Color _sdSlowColor;

    private Color _sdNormalColor;

    private Color _sdFastColor;

    private Color _sdSuperFastColor;

    public void Awake()
    {
        GameSpeedManager.Instance.OnGameSpeedChanged += OnGameSpeedChanged;
    }

    public void Start()
    {
        _sdPauseColor = sdPause.color;
        _sdSlowColor = sdSlow.color;
        _sdNormalColor = sdNormal.color;
        _sdFastColor = sdFast.color;
        _sdSuperFastColor = sdSuperFast.color;
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
            case EGameSpeed.Paused:
                sdPause.color =
                    new Color(_sdPauseColor.r,
                        _sdPauseColor.g,
                        _sdPauseColor.b,
                        1);
                image = sdPause;
                break;
            case EGameSpeed.Slow:
                sdSlow.color =
                    new Color(_sdSlowColor.r,
                        _sdSlowColor.g,
                        _sdSlowColor.b,
                        1);
                image = sdSlow;
                break;
            case EGameSpeed.Normal:
                sdNormal.color =
                    new Color(_sdNormalColor.r,
                        _sdNormalColor.g,
                        _sdNormalColor.b,
                        1);
                image = sdNormal;
                break;
            case EGameSpeed.Fast:
                sdFast.color =
                    new Color(_sdFastColor.r,
                        _sdFastColor.g,
                        _sdFastColor.b,
                        1);
                image = sdFast;
                break;
            case EGameSpeed.Superfast:
                sdSuperFast.color =
                    new Color(_sdSuperFastColor.r,
                        _sdSuperFastColor.g,
                        _sdSuperFastColor.b,
                        1);
                image = sdSuperFast;
                break;
        }

        //set the opacity of all the other images to .5
        foreach (var otherImage in GetComponentsInChildren<Image>())
        {
            if (image == null) continue;
            if (otherImage.name != image.name)
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
