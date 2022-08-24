using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Neo4j.Driver;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public static DateTime Date { get; private set; }

    public DateDisplay DateDisplay;

    public List<Country> CountryList;

    private Coroutine _useCountryData;

    private Coroutine _initializeGame;

    private static readonly IDriver
        driver =
            GraphDatabase
                .Driver("bolt://localhost:7687",
                AuthTokens.Basic("neo4j", "glory"));

    public void Awake()
    {
        DateDisplay = FindObjectOfType<DateDisplay>();
        Date = new DateTime(1500, 1, 1);
    }

    public void Start()
    {
        //get country list
        CountryList = FindObjectsOfType<Country>().ToList();

        //set the date display text
        UpdateDateDisplay();
        _initializeGame = StartCoroutine(DatabaseSetupProcess());
    }

    //Increment the number of days passed by 1 each frame and wait for all the other functions to finish
    public void Update()
    {
        Date = Date.AddDays(1);
        UpdateDateDisplay();

        _useCountryData = StartCoroutine(CountryDataProcessor());
    }

    //Every time the date is incremented, several tasks begin in sequential order.
    //1. Update the date display
    private void UpdateDateDisplay()
    {
        DateDisplay.UseDateDisplay();
    }

    //2. Download the countryData from the database
    private async void DownloadCountryData(Country country)
    {
        IAsyncSession session =
            driver.AsyncSession(o => o.WithDatabase("nationalbaseline"));
        try
        {
            //get the country data from the database matching the country's name
            IResultCursor cursor =
                await session
                    .RunAsync("MATCH (n:Country {name:'$CountryName'}) RETURN n",
                    new Dictionary<string, object> {
                        { "CountryName", country.gameObject.name }
                    });

            while (await cursor.FetchAsync())
            {
                Debug.Log("Found country data for " + country.gameObject.name);
                country
                    .ParseCountryData(cursor
                        .Current["n"]
                        .As<INode>()
                        .Properties);
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

    //3. Calculate any changes to the countryData
    private void UseCalculateCountryData()
    {
        //foreach country in the country list
        foreach (Country country in CountryList)
        {
            //calculate the changes to the countryData
            country.CalculateCountryData();
        }
    }

    //4. Upload any changes to the countryData to the database
    private async void UploadCountryData(Country country)
    {
        if (country.CountryData == null)
        {
            Debug
                .Log("Upload: countryData is null for " +
                country.gameObject.name);
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
                            { "CountryName", country.gameObject.name },
                            { "prop", prop.Key },
                            { "value", prop.Value }
                        });
                while (await cursor.FetchAsync())
                {
                    Debug
                        .Log("UL: " +
                        country.gameObject.name +
                        ": " +
                        prop.Key +
                        " = " +
                        prop.Value);
                }
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
    public async void ClearDatabase()
    {
        var nationalbaselineSession =
            driver.AsyncSession(o => o.WithDatabase("nationalbaseline"));
        try
        {
            IResultCursor nationalbaselineCursor =
                await nationalbaselineSession
                    .RunAsync("MATCH(a) -[r]-> (), (b) DELETE a, r, b");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            Debug.Log("database was successfully cleared!");
            await nationalbaselineSession.CloseAsync();
        }
    }

    public async void CopyDatabase()
    {
        var neo4jSession = driver.AsyncSession(o => o.WithDatabase("neo4j"));
        var nationalbaselineSession =
            driver.AsyncSession(o => o.WithDatabase("nationalbaseline"));
        try
        {
            IResultCursor neo4JCursor =
                await neo4jSession
                    .RunAsync("CALL apoc.export.graphml.all('countryData.graphml', {readLabels: true, useTypes: true})");
            IResultCursor nationalbaselineCursor =
                await nationalbaselineSession
                    .RunAsync("CALL apoc.import.graphml('countryData.graphml', {readLabels: true, storeNodeIds: true})");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            Debug.Log("database was successfully copied!");
            await neo4jSession.CloseAsync();
            await nationalbaselineSession.CloseAsync();
        }
    }

    private IEnumerator DatabaseSetupProcess()
    {
        Debug.Log("Database setup process started");

        ClearDatabase();
        CopyDatabase();

        for (int i = 0; i < CountryList.Count; i++)
        {
            Debug
                .Log("Downloading country data for " +
                CountryList[i].gameObject.name);

            DownloadCountryData(CountryList[i]);
            yield return null;
        }
    }

    private IEnumerator CountryDataProcessor()
    {
        Debug.Log("Country data processor started");

        for (int i = 0; i < CountryList.Count; i++)
        {
            Debug
                .Log("Downloading country data for " +
                CountryList[i].gameObject.name);
            DownloadCountryData(CountryList[i]);

            Debug
                .Log("Calculating country data for " +
                CountryList[i].gameObject.name);
            UseCalculateCountryData();

            Debug
                .Log("Uploading country data for " +
                CountryList[i].gameObject.name);
            UploadCountryData(CountryList[i]);
            yield return null;
        }
    }
}
