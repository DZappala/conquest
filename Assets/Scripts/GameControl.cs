using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class GameControl : MonoBehaviour
{
    private static DateTime Date { get; set; }

    [FormerlySerializedAs("DateDisplay")] public DateDisplay dateDisplay;

    private List<Country> _countryList;

    public static float DelayInSeconds;

    private bool _countriesAreUpdating;

    private static int _downloadsComplete;

    public static int CalculationsComplete;

    private static int _uploadsComplete;

    public void Awake()
    {
        Date = new DateTime(1500, 1, 1);
        SpeedController.SwitchGameSpeed(EGameSpeed.Paused);
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
        {
            return;
        }

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
    private IEnumerator DatabaseSetupProcess()
    {
        //Download the country data from the database
        foreach (var country in _countryList)
        {
            DB
                .GetCountry(country)
                .Subscribe(countryData =>
                {
                    country.ParseCountryData(countryData);
                    _downloadsComplete++;
                });
        }

        yield return new WaitUntil(() =>
            _downloadsComplete == _countryList.Count);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator CountryDataProcessor(float delay)
    {
        var enumeratorStart = Time.realtimeSinceStartup;

        Date = Date.AddDays(1);
        Debug.Log($"Date was incremented to {Date:dd MMM yyyy}");

        UpdateDateDisplay(Date);

        yield return new WaitForSeconds(.01f);

        Assert.IsTrue(_countryList.Count > 0, "Country list is empty");

        Debug.Log("Done Setting date");

        foreach (var country in _countryList)
        {
            if (country.CountryData.Tag == "IDF")
            {
                Debug.Log($"Downloading country data for {country.name}");
            }

            DB
                .GetCountry(country)
                .Subscribe(record =>
                {
                    country.ParseCountryData(record);
                    _downloadsComplete++;
                });

            if (country.CountryData.Tag == "IDF")
            {
                Debug
                    .Log($"After DOWNLOADS completed, got Money for IDF: {country.CountryData.Money}");
            }
        }

        yield return new WaitUntil(() =>
            _downloadsComplete == _countryList.Count);

        _downloadsComplete = 0;

        foreach (var country in _countryList)
        {
            Assert
                .IsNotNull(country.CountryData.Tag,
                    $"Country Data was null for {country.CountryData.Name}");

            //calculate the changes to the countryData
            country.CalculateCountryData();

            yield return new WaitUntil(() =>
                CalculationsComplete == _countryList.Count);

            if (country.CountryData.Tag == "IDF")
            {
                Debug
                    .Log($"After CALCULATIONS completed, got Money for IDF: {country.CountryData.Money}");
            }
        }

        CalculationsComplete = 0;

        foreach (var country in _countryList)
        {
            //upload the changes to the database
            DB
                .SetCountry(country)
                .Subscribe(record => { Debug.Log(record.ToString()); },
                    () => { _uploadsComplete++; });

            yield return new WaitUntil(() =>
                _uploadsComplete == _countryList.Count);

            if (country.CountryData.Tag == "IDF")
            {
                Debug
                    .Log($"after UPLOADS completed, got Money for IDF: {country.CountryData.Money}");
            }
        }

        _uploadsComplete = 0;

        var timeElapsed = Time.realtimeSinceStartup - enumeratorStart;
        if (timeElapsed > delay)
        {
            yield return _countriesAreUpdating = false;
        }
        else
        {
            delay -= timeElapsed;
            yield return new WaitForSecondsRealtime(delay);
            yield return _countriesAreUpdating = false;
        }
    }
}
