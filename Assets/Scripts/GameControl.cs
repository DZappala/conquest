using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class GameControl : MonoBehaviour
{
    public static float DelayInSeconds;

    private static int _downloadsComplete;

    public static int CalculationsComplete;

    private static int _uploadsComplete;

    private static DB _db;

    [FormerlySerializedAs("DateDisplay")] public DateDisplay dateDisplay;

    private bool _countriesAreUpdating;

    private List<Country> _countryList;
    private static DateTime Date { get; set; }

    public void Awake()
    {
        Date = new DateTime(1500, 1, 1);
        SpeedController.SwitchGameSpeed(EGameSpeed.Paused);

        _db = new DB();
        _downloadsComplete = 0;
        _uploadsComplete = 0;
        CalculationsComplete = 0;
    }

    public void Start()
    {
        //get country list
        _countryList = FindObjectsOfType<Country>().ToList();

        //set the date display text
        UpdateDateDisplay(Date);
        StartCoroutine(DatabaseSetupProcess());
    }

    //Increment the number of days passed by 1 each frame and wait for all the other functions to finish
    public void Update()
    {
        if (
            _countriesAreUpdating ||
            GameSpeedManager.Instance.CurrentGameSpeed == EGameSpeed.Paused
        )
            return;

        StartCoroutine(CountryDataProcessor(DelayInSeconds));
        _countriesAreUpdating = true;
    }

    //Every time the date is incremented, several tasks begin in sequential order.
    //1. Update the date display
    private void UpdateDateDisplay(DateTime date)
    {
        dateDisplay.UseDateDisplay(date);
    }

    //2. Download the countryData from the database
    //3. Calculate any changes to the countryData
    //4. Upload any changes to the countryData to the database
    //When all the tasks are completed, the next day begins.
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator DatabaseSetupProcess()
    {
        //Download the country data from the database
        foreach (var country in _countryList)
            _db
                .GetCountry(country)
                .Subscribe(countryData =>
                {
                    country.ParseCountryData(countryData);
                    _downloadsComplete++;
                });

        yield return new WaitUntil(() => _downloadsComplete == _countryList.Count);
        _downloadsComplete = 0;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator CountryDataProcessor(float delay)
    {
        var enumeratorStart = Time.realtimeSinceStartup;

        Date = Date.AddDays(1);

        UpdateDateDisplay(Date);

        yield return new WaitForSeconds(.01f);

        Assert.IsTrue(_countryList.Count > 0, "Country list is empty");

        _downloadsComplete = 0;

        Assert.IsTrue(_downloadsComplete == 0, "Download counter was not reset to 0");

        foreach (var country in _countryList)
            _db
                .GetCountry(country)
                .Subscribe(record =>
                {
                    country.ParseCountryData(record);
                    _downloadsComplete++;
                });

        yield return new WaitUntil(() =>
            _downloadsComplete == _countryList.Count);

        _downloadsComplete = 0;

        CalculationsComplete = 0;
        Assert.IsTrue(CalculationsComplete == 0, "Calculations counter was not reset to 0");

        foreach (var country in _countryList)
        {
            Assert
                .IsNotNull(country.CountryData.Tag,
                    $"Country Data was null for {country.CountryData.Name}");

            //calculate the changes to the countryData
            country.CalculateCountryData();
        }

        yield return new WaitUntil(() =>
            CalculationsComplete == _countryList.Count);

        CalculationsComplete = 0;

        _uploadsComplete = 0;
        Assert.IsTrue(_uploadsComplete == 0, "Upload counter was not reset to 0");
        foreach (var country in _countryList)
            //upload the changes to the database
            _db
                .SetCountry(country)
                .Subscribe(_ => { _uploadsComplete++; });

        yield return new WaitUntil(() =>
            _uploadsComplete == _countryList.Count);

        _uploadsComplete = 0;

        if (_downloadsComplete != 0 || _uploadsComplete != 0 || CalculationsComplete != 0)
            Debug.LogError("Counters were never properly reset, something went wrong");

        var timeElapsed = Time.realtimeSinceStartup - enumeratorStart;
        if (timeElapsed > delay)
        {
            _countriesAreUpdating = false;
        }
        else
        {
            delay -= timeElapsed;
            yield return new WaitForSecondsRealtime(delay);
            _countriesAreUpdating = false;
        }
    }
}
