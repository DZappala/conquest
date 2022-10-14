using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIMain : MonoBehaviour
{
    private Stack<GroupBox> _activeWindows;
    private CountryData _countryData;
    private GroupBox _countryPanel;
    private Label _cpMoney;
    private Label _cpName;
    private Label _cpPopulation;
    private Label _dspDate;
    private Label _ppMoney;
    private Label _ppName;
    private Label _ppPopulation;
    private VisualElement _spFast;
    private VisualElement _spNormal;
    private VisualElement _spPaused;
    private VisualElement _spSlow;
    private VisualElement _spSuperFast;
    private VisualElement[] _visualElements;

    private void Awake()
    {
        CountryDisplayManager.Instance.OnCountrySelected += OnCountrySelected;
        GameSpeedManager.Instance.OnGameSpeedChanged += OnGameSpeedChanged;

        var root = GetComponent<UIDocument>().rootVisualElement;
        _ppMoney = root.Q<Label>("PPMoney");
        _ppName = root.Q<Label>("PPName");
        _ppPopulation = root.Q<Label>("PPPopulation");
        _cpName = root.Q<Label>("CPName");
        _cpMoney = root.Q<Label>("CPMoney");
        _cpPopulation = root.Q<Label>("CPPopulation");
        _countryPanel = root.Q<GroupBox>("CountryPanel");
        _spPaused = root.Q<VisualElement>("SPPaused");
        _spSlow = root.Q<VisualElement>("SPSlow");
        _spNormal = root.Q<VisualElement>("SPNormal");
        _spFast = root.Q<VisualElement>("SPFast");
        _spSuperFast = root.Q<VisualElement>("SPSuperFast");
        _dspDate = root.Q<Label>("DSPDate");
    }

    private void Start()
    {
        _visualElements = new[] { _spPaused, _spSlow, _spNormal, _spFast, _spSuperFast };
        _activeWindows = new();
        _countryPanel.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if (CountryDisplayManager.Instance.IsCountrySelected)
            //HACK is there a way to invoke this through the instance instead?
            OnCountrySelected(_countryData);
    }

    public void OnDestroy()
    {
        CountryDisplayManager.Instance.OnCountrySelected -= OnCountrySelected;
        GameSpeedManager.Instance.OnGameSpeedChanged -= OnGameSpeedChanged;
    }

    private void OnGameSpeedChanged(EGameSpeed newGameSpeed)
    {
        UseSpeedDisplay();
        SpeedController.SwitchGameSpeed(newGameSpeed);
    }


    public void UsePlayerCountryData(CountryData countryData)
    {
        if (countryData == null)
        {
            Debug.LogError("Country data is null");
            return;
        }

        _ppName.text = countryData.Name;
        _ppMoney.text = countryData.Money.ToString("F1");
        _ppPopulation.text = countryData.Population.ToString("F1");
    }

    private void OnCountrySelected(CountryData countryDataFromEventParam)
    {
        if (countryDataFromEventParam == null)
        {
            Debug.LogError("Country data is null");
            return;
        }

        if (_countryPanel.style.display == DisplayStyle.None)
        {
            _countryPanel.style.display = DisplayStyle.Flex;
            _activeWindows.Push(_countryPanel);
        }

        _countryData = countryDataFromEventParam;
        UpdateCountryData();
    }

    private void UpdateCountryData()
    {
        _cpName.text = _countryData.Name;
        _cpMoney.text = _countryData.Money.ToString("F1");
        _cpPopulation.text = _countryData.Population.ToString("F1");
    }

    private void UseSpeedDisplay()
    {
        foreach (var speedDisplay in _visualElements)
            speedDisplay.style.opacity = 0.5f;

        //set the opacity of the image whose name matches the current game speed to 1
        switch (GameSpeedManager.Instance.CurrentGameSpeed)
        {
            case EGameSpeed.Paused:
                _spPaused.style.opacity = 1;
                break;
            case EGameSpeed.Slow:
                _spSlow.style.opacity = 1;
                break;
            case EGameSpeed.Normal:
                _spNormal.style.opacity = 1;
                break;
            case EGameSpeed.Fast:
                _spFast.style.opacity = 1;
                break;
            case EGameSpeed.Superfast:
                _spSuperFast.style.opacity = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void UseDateDisplay(DateTime date)
    {
        _dspDate.text = date.ToString("dd MMM yyyy");
    }

    public void CloseLastWindow()
    {
        if (_activeWindows.Count == 0) return;
        var prevWindow = _activeWindows.Pop();
        prevWindow.style.display = DisplayStyle.None;
        if (prevWindow == _countryPanel)
            CountryDisplayManager.Instance.IsCountrySelected = false;
    }
}
