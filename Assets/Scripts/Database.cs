using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Neo4j.Driver;
using UnityEngine;

public class Database
{
    //create a link to the local neo4j database
    private static readonly IDriver
        driver =
            GraphDatabase
                .Driver("bolt://localhost:7687",
                AuthTokens.Basic("neo4j", "glory"));

    //copies the database from the immutable neo4j db to a mutable one
    private static Task CopyDatabase()
    {
        Debug.Log("Copying database...");
        Task
            .Run(async () =>
            {
                var neo4jSession =
                    driver.AsyncSession(o => o.WithDatabase("neo4j"));
                var nationalbaselineSession =
                    driver
                        .AsyncSession(o => o.WithDatabase("nationalbaseline"));
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
                    await neo4jSession.CloseAsync();
                    await nationalbaselineSession.CloseAsync();
                }
            });
        return Task.CompletedTask;
    }

    private static Task ClearDatabase()
    {
        Debug.Log("Clearing database...");
        Task
            .Run(async () =>
            {
                var nationalbaselineSession =
                    driver
                        .AsyncSession(o => o.WithDatabase("nationalbaseline"));
                try
                {
                    IResultCursor nationalbaselineCursor =
                        await nationalbaselineSession
                            .RunAsync("MATCH(a) -[r]-> (), (b) DELETE a, r, b");
                    Debug.Log("database was successfully cleared!");
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                finally
                {
                    await nationalbaselineSession.CloseAsync();
                }
            });
        return Task.CompletedTask;
    }

    public static bool isRun = false;

    public static Task DatabaseConstruct()
    {
        isRun = true;
        Task
            .Run(async () =>
            {
                //constructor
                await ClearDatabase(); // DEBUG TEMP FUNCTION TO CLEAR DATABASE
                await CopyDatabase();
            });
        return Task.CompletedTask;
    }

    //downloads all the data from the Neo4j and parses it into a CountryData object
    public Task DownloadCountryData(CountryData countryData, Country country)
    {
        Task
            .Run(async () =>
            {
                if (countryData == null)
                {
                    Debug
                        .Log("DL: countryData is null for " +
                        countryData.title);
                    return;
                }

                IAsyncSession nationalbaselineSession =
                    driver
                        .AsyncSession(o => o.WithDatabase("nationalbaseline"));
                try
                {
                    IResultCursor cursor =
                        await nationalbaselineSession
                            .RunAsync("MATCH (n:Country {name: '$countryName'}) RETURN n",
                            new Dictionary<string, object> {
                                { "countryName", countryData.title }
                            });
                    while (await cursor.FetchAsync())
                    {
                        countryData =
                            country
                                .ParseCountryData(cursor
                                    .Current
                                    .As<INode>()
                                    .Properties);
                        Debug.Log("DL: " + countryData.title);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                finally
                {
                    await nationalbaselineSession.CloseAsync();
                }
            });
        return Task.CompletedTask;
    }

    //uploads all the data from the CountryData object to the Neo4j database
    public Task UploadCountryData(CountryData countryData)
    {
        Task
            .Run(async () =>
            {
                //get the countryData as a dictionary
                Dictionary<string, object> Dict =
                    countryData
                        .GetType()
                        .GetProperties(BindingFlags.Public |
                        BindingFlags.Instance)
                        .ToDictionary(prop => prop.Name,
                        prop => prop.GetValue(countryData));

                //for each of the properties in the dictionary upload the data to the database if that property is not null
                foreach (var property in Dict)
                {
                    if (property.Value != null)
                    {
                        Debug
                            .Log("UL: key = " +
                            property.Key.ToString() +
                            ": value = " +
                            property.Value.ToString() +
                            "Country: " +
                            countryData.title); //DEBUG

                        var session =
                            driver
                                .AsyncSession(o =>
                                    o.WithDatabase("nationalbaseline"));
                        try
                        {
                            IResultCursor cursor =
                                await session
                                    .RunAsync("MATCH (n:Country {name: '$countryName'}) SET n.$property = '$value'",
                                    new Dictionary<string, object> {
                                        { "countryName", countryData.title },
                                        { "property", property.Key },
                                        { "value", property.Value }
                                    });
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
                }
            });
        return Task.CompletedTask;
    }
}
