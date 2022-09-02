using System;
using System.Collections;
using Neo4j.Driver;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    //public Image LoadingBar;
    // public static bool GameIsLoading = true;
    //public Animator LoadingBarAnimator;
    public bool DatabaseCleared = false;

    public bool DatabaseSetup = false;

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
        //TODO create loading animation
        //start the loading bar animation
        //LoadingBarAnimator.SetTrigger("Start");
        //wait for database to clear and copy
        ClearDatabase();
        yield return new WaitWhile(() => DatabaseCleared == false);

        CopyDatabase();
        yield return new WaitWhile(() => DatabaseSetup == false);

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
            DatabaseCleared = true;
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
            new WaitForSecondsRealtime(2);
            await nationalbaselineSession.CloseAsync();
            DatabaseSetup = true;
        }
    }
}
