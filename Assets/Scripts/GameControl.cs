using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Neo4j.Driver;
using UnityEngine;
using Unity.VisualScripting;

public class GameControl : MonoBehaviour
{
    public static DateTime Date { get; private set; }

    public DateDisplay DateDisplay;

    public List<Country> CountryList;

    public static int DelayInSeconds;

    public bool CountryUpdating;

    private static readonly IDriver
        driver =
            GraphDatabase
                .Driver("bolt://localhost:7687",
                AuthTokens.Basic("neo4j", "glory"));

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
            !CountryUpdating &&
            GameSpeedManager.Instance.CurrentGameSpeed != EGameSpeed.PAUSED
        )
        {
            StartCoroutine(CountryDataProcessor(DelayInSeconds));
        }
    }

    //Every time the date is incremented, several tasks begin in sequential order.
    //1. Update the date display
    public void UpdateDateDisplay(DateTime date)
    {
        DateDisplay.UseDateDisplay (date);
    }

    //2. Download the countryData from the database
    public async void DownloadCountryData(Country country)
    {
        string query =
            @"MATCH (c:Country) WHERE c.name = $CountryName
            RETURN c";

        IDictionary<string, object> parameters =
            new Dictionary<string, object> {
                { "CountryName", country.CountryData.name }
            };

        IAsyncSession session =
            driver.AsyncSession(o => o.WithDatabase("nationalbaseline"));

        try
        {
            IResultCursor resultCursor =
                await session.RunAsync(query, parameters);

            while (await resultCursor.FetchAsync())
            {
                Debug.Log($"{resultCursor.Current[0].As<INode>()["tag"]}");
                await country
                    .ParseCountryData(resultCursor
                        .Current[0]
                        .As<INode>()
                        .Properties);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + e.StackTrace);
        }
        finally
        {
            await session.CloseAsync();
        }
    }

    //3. Calculate any changes to the countryData
    //4. Upload any changes to the countryData to the database
    public async Task UploadCountryData(Country country)
    {
        if (country.CountryData == null)
        {
            Debug
                .Log("Upload: countryData is null for " +
                country.CountryData.name);
            return;
        }

        Dictionary<string, object> properties =
            country
                .CountryData
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(prop => prop.Name,
                prop => prop.GetValue(country.CountryData));

        IAsyncSession session =
            driver.AsyncSession(o => o.WithDatabase("nationalbaseline"));

        try
        {
            foreach (var prop in properties)
            {
                IResultCursor cursor =
                    await session
                        .RunAsync("MATCH (n:Country {name: '$CountryName'}) SET n.$prop = $value",
                        new Dictionary<string, object> {
                            { "CountryName", country.CountryData.name },
                            { "prop", prop.Key },
                            { "value", prop.Value }
                        });
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            await session.CloseAsync();
        }
    }

    //When all the tasks are completed, the next day begins.
    private IEnumerator DatabaseSetupProcess()
    {
        Debug.Log("Country setup process has started");

        //TODO, copy database download process from below
        // foreach (Country country in CountryList)
        // {
        //     Task download = DownloadCountryData(country);
        // }
        // yield return Task.WhenAll(CountryList.Select(DownloadCountryData));
        yield return null; //DEBUG
    }

    private IEnumerator CountryDataProcessor(int delay)
    {
        //FIXME: this isn't downloading or uploading data. it's just incrementing the date. It needs to perform these actions synchronously.
        Debug.Log("Country data processor started");
        CountryUpdating = true;

        Date = Date.AddDays(1);
        UpdateDateDisplay (Date);

        yield return new WaitForSeconds(.01f);

        foreach (Country country in CountryList)
        {
            DownloadCountryData (country);
            Debug
                .Assert(country.CountryData.tag != null,
                "Download was not completed successfully");
            yield return null;
        }

        yield return null;

        foreach (Country country in CountryList)
        {
            //calculate the changes to the countryData
            Debug.Log("Calculating changes for " + country.CountryData.name);

            country.CalculateCountryData();
        }

        foreach (Country country in CountryList)
        {
            Debug.Log("Uploading changes for " + country.CountryData.name);
            Task upload = UploadCountryData(country);
        }

        yield return Task
                .WhenAll(CountryList
                    .Select(country => UploadCountryData(country)));

        yield return new WaitForSecondsRealtime(delay);

        CountryUpdating = false;
    }
}
