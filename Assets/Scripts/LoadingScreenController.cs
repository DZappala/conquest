using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;

public class LoadingScreenController : MonoBehaviour
{
    //public Image LoadingBar;
    // public static bool GameIsLoading = true;
    //public Animator LoadingBarAnimator;
    private static readonly IDriver
        driver =
            GraphDatabase
                .Driver("bolt://localhost:7687",
                AuthTokens.Basic("neo4j", "glory"));

    public void Start()
    {
        StartCoroutine(StartDatabaseProcess());
    }

    public IEnumerator StartDatabaseProcess()
    {
        //start the loading bar animation
        //LoadingBarAnimator.SetTrigger("Start");
        //wait for database to clear and copy
        ClearDatabase();
        yield return null;

        CopyDatabase();
        yield return null;

        LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        StartCoroutine(Load(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public IEnumerator Load(int levelIndex)
    {
        //Load next scene
        SceneManager.LoadScene (levelIndex);
        yield return null;
    }

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
            await neo4jSession.CloseAsync();
            await nationalbaselineSession.CloseAsync();
        }
    }
}
