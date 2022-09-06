using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver;
using UnityEngine;
using UnityEngine.Assertions;

public class GameControl : MonoBehaviour
{
    public static DateTime Date { get; private set; }

    public DateDisplay DateDisplay;

    private List<Country> CountryList;

    public static float DelayInSeconds;

    private bool CountriesAreUpdating;

    private static int DownloadsComplete = 0;

    public static int CalculationsComplete = 0;

    private static int UploadsComplete = 0;

    public void Awake()
    {
        Date = new DateTime(1500, 1, 1);
        SpeedController.SwitchGameSpeed(EGameSpeed.PAUSED);
    }

    public void Start()
    {
        //get country list
        CountryList = FindObjectsOfType<Country>().ToList();

        //set the date display text
        UpdateDateDisplay (Date);
        StartCoroutine(DatabaseSetupProcess());
    }

    //Increment the number of days passed by 1 each frame and wait for all the other functions to finish
    public void Update()
    {
        if (
            CountriesAreUpdating ||
            GameSpeedManager.Instance.CurrentGameSpeed == EGameSpeed.PAUSED
        )
        {
            return;
        }

        StartCoroutine(CountryDataProcessor(DelayInSeconds));
        CountriesAreUpdating = true;
    }

    //Every time the date is incremented, several tasks begin in sequential order.
    //1. Update the date display
    public void UpdateDateDisplay(DateTime date)
    {
        DateDisplay.UseDateDisplay (date);
    }

    //2. Download the countryData from the database
    //3. Calculate any changes to the countryData
    //4. Upload any changes to the countryData to the database
    //When all the tasks are completed, the next day begins.
    private IEnumerator DatabaseSetupProcess()
    {
        //Download the country data from the database
        foreach (Country country in CountryList)
        {
            DB
                .GetCountry(country)
                .Subscribe(countryData =>
                {
                    country.ParseCountryData (countryData);
                    DownloadsComplete++;
                });
        }
        yield return new WaitUntil(() =>
                    DownloadsComplete == CountryList.Count);
    }

    private IEnumerator CountryDataProcessor(float delay)
    {
        float enumeratorStart = Time.realtimeSinceStartup;

        Date = Date.AddDays(1);
        Debug.Log("Date was incremented to " + Date.ToString("dd MMM yyyy"));

        UpdateDateDisplay (Date);

        yield return new WaitForSeconds(.01f);

        Assert.IsTrue(CountryList.Count > 0, "Country list is empty");

        Debug.Log("Done Setting date");

        foreach (Country country in CountryList)
        {
            if (country.CountryData.tag == "IDF")
            {
                Debug.Log("Downloading country data for " + country.name);
            }

            DB
                .GetCountry(country)
                .Subscribe(record =>
                {
                    country.ParseCountryData (record);
                    DownloadsComplete++;
                });

            if (country.CountryData.tag == "IDF")
            {
                Debug
                    .Log("After DOWNLOADS completed, got Money for IDF: " +
                    country.CountryData.money);
            }
        }

        yield return new WaitUntil(() =>
                    DownloadsComplete == CountryList.Count);

        DownloadsComplete = 0;

        foreach (Country country in CountryList)
        {
            Assert
                .IsNotNull(country.CountryData.tag,
                "Country Data was null for " + country.CountryData.name);

            //calculate the changes to the countryData
            country.CalculateCountryData();

            yield return new WaitUntil(() =>
                        CalculationsComplete == CountryList.Count);

            if (country.CountryData.tag == "IDF")
            {
                Debug
                    .Log("After CALCULATIONS completed, got Money for IDF: " +
                    country.CountryData.money);
            }
        }

        CalculationsComplete = 0;

        foreach (Country country in CountryList)
        {
            //upload the changes to the database
            DB
                .SetCountry(country)
                .Subscribe(record =>
                {
                    Debug.Log(record.ToString());
                },
                () =>
                {
                    UploadsComplete++;
                });

            yield return new WaitUntil(() =>
                        UploadsComplete == CountryList.Count);

            if (country.CountryData.tag == "IDF")
            {
                Debug
                    .Log("after UPLOADS completed, got Money for IDF: " +
                    country.CountryData.money);
            }
        }

        UploadsComplete = 0;

        float timeElapsed = Time.realtimeSinceStartup - enumeratorStart;
        if (timeElapsed > delay)
        {
            yield return CountriesAreUpdating = false;
        }
        else
        {
            delay -= timeElapsed;
            yield return new WaitForSecondsRealtime(delay);
            yield return CountriesAreUpdating = false;
        }
    }
}
