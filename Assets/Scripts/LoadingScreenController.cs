using System;
using System.Collections;
using Neo4j.Driver;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LoadingScreenController : MonoBehaviour
{
    private static readonly IDriver
        Driver =
            GraphDatabase
                .Driver("bolt://localhost:7687",
                    AuthTokens.Basic("neo4j", "glory"));

    //public Image LoadingBar;
    // public static bool GameIsLoading = true;
    //public Animator LoadingBarAnimator;
    [FormerlySerializedAs("DatabaseCleared")]
    public bool databaseCleared;

    [FormerlySerializedAs("DatabaseSetup")]
    public bool databaseSetup;

    public void Start()
    {
        StartCoroutine(StartDatabaseProcess());
    }

    private IEnumerator StartDatabaseProcess()
    {
        //TODO create loading animation
        //start the loading bar animation
        //LoadingBarAnimator.SetTrigger("Start");
        //wait for database to clear and copy
        ClearDatabase();
        yield return new WaitWhile(() => databaseCleared == false);

        CopyDatabase();
        yield return new WaitWhile(() => databaseSetup == false);

        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        StartCoroutine(Load(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private IEnumerator Load(int levelIndex)
    {
        //Load next scene
        SceneManager.LoadScene(levelIndex);
        yield return null;
    }

    private async void ClearDatabase()
    {
        var nationalbaselineSession =
            Driver.AsyncSession(o => o.WithDatabase("nationalbaseline"));
        try
        {
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
            databaseCleared = true;
        }
    }

    private async void CopyDatabase()
    {
        var neo4JSession = Driver.AsyncSession(o => o.WithDatabase("neo4j"));
        var nationalbaselineSession =
            Driver.AsyncSession(o => o.WithDatabase("nationalbaseline"));
        try
        {
            await neo4JSession
                .RunAsync("CALL apoc.export.graphml.all('countryData.graphml', {readLabels: true, useTypes: true})");

            await nationalbaselineSession
                .RunAsync("CALL apoc.import.graphml('countryData.graphml', {readLabels: true, storeNodeIds: true})");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            await neo4JSession.CloseAsync();
            await nationalbaselineSession.CloseAsync();
            databaseSetup = true;
        }
    }
}
