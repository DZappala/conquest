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

    public List<Country> CountryList;

    public static float DelayInSeconds;

    public bool CountryUpdating;

    public static int DownloadsComplete = 0;

    public static int CalculationsComplete = 0;

    public static int UploadsComplete = 0;

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
            await session.CloseAsync();
            DownloadsComplete++;
        }
    }

    //3. Calculate any changes to the countryData
    //4. Upload any changes to the countryData to the database
    public async void UploadCountryData(Country country)
    {
        //FIXME is not uploading data to the database by prop. Seems to be running the upload method but skipping any uploads
        //convert the CountryData of a country to a dictionary for uploading
        Dictionary<string, object> countryDataProps =
            country
                .CountryData
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(prop => prop.Name,
                prop => prop.GetValue(country.CountryData));

        IAsyncSession session =
            driver.AsyncSession(o => o.WithDatabase("nationalbaseline"));

        Assert
            .IsNotNull(country.CountryData,
            "Country Data was null for " + country.name);

        try
        {
            await session
                .WriteTransactionAsync(async tx =>
                {
                    await tx
                        .RunAsync(@"MERGE (c:Country {name: $CountryName})
                    SET c = $CountryData",
                        new Dictionary<string, object> {
                            { "CountryName", country.CountryData.name },
                            { "CountryData", countryDataProps }
                        });
                });
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
        }
        finally
        {
            await session.CloseAsync();
            UploadsComplete++;
        }
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

    private IEnumerator CountryDataProcessor(float delay)
    {
        var enumeratorStart = Time.realtimeSinceStartup;

        //FIXME: this isn't downloading or uploading data. it's just incrementing the date. It needs to perform these actions synchronously.
        CountryUpdating = true;

        Date = Date.AddDays(1);
        UpdateDateDisplay (Date);

        yield return new WaitForSeconds(.01f);

        foreach (Country country in CountryList)
        {
            DownloadCountryData (country);
        }

        yield return new WaitUntil(() =>
                    DownloadsComplete == CountryList.Count);
        DownloadsComplete = 0;

        foreach (Country country in CountryList)
        {
            Assert
                .IsNotNull(country.CountryData.tag,
                "Country Data was null for " + country.name);

            //calculate the changes to the countryData
            country.CalculateCountryData();
        }

        yield return new WaitUntil(() =>
                    CalculationsComplete == CountryList.Count);
        CalculationsComplete = 0;

        foreach (Country country in CountryList)
        {
            //upload the changes to the database
            UploadCountryData (country);
        }

        yield return new WaitUntil(() => UploadsComplete == CountryList.Count);
        UploadsComplete = 0;

        if ((float)(Time.realtimeSinceStartup - enumeratorStart) > delay)
        {
            yield return null;
        }
        else
        {
            delay -= (float)(Time.realtimeSinceStartup - enumeratorStart);
            yield return new WaitForSecondsRealtime(delay);
        }

        CountryUpdating = false;
    }
}
