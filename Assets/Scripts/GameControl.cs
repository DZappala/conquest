using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Neo4j.Driver;
using UnityEngine;
using UnityEngine.Assertions;

public class GameControl : MonoBehaviour
{
    public static DateTime Date { get; private set; }

    public DateDisplay DateDisplay;

    private List<Country> CountryList;

    private Dictionary<string, object> CountryDataFields;

    public static float DelayInSeconds;

    private bool CountriesAreUpdating;

    private static int DownloadsComplete = 0;

    public static int CalculationsComplete = 0;

    private static int UploadsComplete = 0;

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
        while (CountriesAreUpdating ||
            GameSpeedManager.Instance.CurrentGameSpeed == EGameSpeed.PAUSED
        )
        {
            return;
        }
        StartCoroutine(CountryDataProcessor(DelayInSeconds));
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
                await session
                    .RunAsync(query, parameters)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            throw task.Exception;
                        }

                        return task.Result;
                    });

            while (await resultCursor.FetchAsync())
            {
                await country
                    .ParseCountryData(resultCursor
                        .Current[0]
                        .As<INode>()
                        .Properties);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
        }
        finally
        {
            await session
                .CloseAsync()
                .ContinueWith(task =>
                {
                    DownloadsComplete++;
                });
        }
    }

    //3. Calculate any changes to the countryData
    //4. Upload any changes to the countryData to the database
    public async void UploadCountryData(Country country)
    {
        IAsyncSession session =
            driver.AsyncSession(o => o.WithDatabase("nationalbaseline"));

        CountryDataFields = GetFields(country.CountryData);

        string query =
            "MATCH (c:Country) WHERE c.name = $CountryName SET c = $CountryData";

        IDictionary<string, object> parameters =
            new Dictionary<string, object> {
                { "CountryName", country.CountryData.name },
                { "CountryData", CountryDataFields }
            };

        try
        {
            IResultCursor resultCursor =
                await session
                    .RunAsync(query, parameters)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            throw task.Exception;
                        }

                        return task.Result;
                    });
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
        }
        finally
        {
            await session
                .CloseAsync()
                .ContinueWith(task =>
                {
                    UploadsComplete++;
                });
        }
    }

    private Dictionary<string, object> GetFields(CountryData countryData)
    {
        var type = countryData.GetType();

        var fields =
            type
                .GetFields()
                .ToDictionary(field => field.Name,
                field => field.GetValue(countryData));

        return fields;
    }

    //When all the tasks are completed, the next day begins.
    private IEnumerator DatabaseSetupProcess()
    {
        //Download the country data from the database
        foreach (Country country in CountryList)
        {
            DownloadCountryData (country);
        }
        yield return new WaitUntil(() =>
                    DownloadsComplete == CountryList.Count);
    }

    //FIXME this Enumerator is having trouble with waiting for each task to complete in sequential order.
    private IEnumerator CountryDataProcessor(float delay)
    {
        CountriesAreUpdating = true;
        float enumeratorStart = Time.realtimeSinceStartup;

        Date = Date.AddDays(1);
        Debug.Log("Date was incremented to " + Date.ToString("dd MMM yyyy"));

        UpdateDateDisplay (Date);

        yield return new WaitForSeconds(.01f);

        foreach (Country country in CountryList)
        {
            DownloadCountryData (country);
            if (country.CountryData.tag == "IDF")
            {
                Debug.Log("Got Money for IDF: " + country.CountryData.money);
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
            if (country.CountryData.tag == "IDF")
            {
                Debug.Log("Got Money for IDF: " + country.CountryData.money);
            }
        }

        yield return new WaitUntil(() =>
                    CalculationsComplete == CountryList.Count);

        CalculationsComplete = 0;

        foreach (Country country in CountryList)
        {
            //upload the changes to the database
            UploadCountryData (country);

            //FIXME somehow this is not returning the correct value
            if (country.CountryData.tag == "IDF")
            {
                Debug.Log("Got Money for IDF: " + country.CountryData.money);
            }
        }

        yield return new WaitUntil(() => UploadsComplete == CountryList.Count);
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
