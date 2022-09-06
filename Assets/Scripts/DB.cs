using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Neo4j.Driver;
using UnityEngine;

public class DB : IDisposable
{
    private static readonly IDriver Driver;
    private bool _disposed;

    ~DB() => Dispose(false);

    static DB()
    {
        Driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "glory"));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static IObservable<IReadOnlyDictionary<string, object>> GetCountry(Country country)
    {
        if (country.CountryData.Tag == "IDF")
        {
            Debug.Log("GetCountry observable was called");
        }

        var session = Driver.RxSession(o => o.WithDatabase("nationalbaseline"));

        if (country.CountryData.Tag == "IDF")
        {
            Debug.Log($"Session was created with {session.SessionConfig.Database}");
        }

        Query query = new("MATCH (c:Country {name: $CountryName}) RETURN c", new { CountryName = country.CountryData.Name });

        return session.ReadTransaction(tx =>
        {
            if (country.CountryData.Tag == "IDF")
            {
                Debug.Log($"ReadTransaction was called. Reading {country.CountryData.Name}");
            }

            return tx.Run(query).Records().Select(record => record[0].As<INode>().Properties);

        }).OnErrorResumeNext(session.Close<IReadOnlyDictionary<string, object>>());
    }

    public static IObservable<IRecord> SetCountry(Country country)
    {
        var session = Driver.RxSession(o => o.WithDatabase("nationalbaseline"));

        var countryDataFields = country.GetCountryDataFields();

        Query query = new(text: "MATCH(c:Country{name:$CountryName}) SET c = $CountryData", parameters: new { CountryName = country.CountryData.Name, CountryData = countryDataFields });

        return session.Run(query).Records();
    }

    public void Clear()
    {
        Query query = new("MATCH(a)->[r]->(), (b) RETURN a, r, b");
        if (query == null) throw new ArgumentNullException(nameof(query));

        using var session = Driver.AsyncSession(o => o.WithDatabase("nationalbaseline").WithDefaultAccessMode(AccessMode.Write));

        session.WriteTransactionAsync(tx => tx.RunAsync(query));
    }

    public void Copy()
    {
        Query copy = new("CALL apoc.export.graphml.all('countryDAta.graphml', {readLabels: true, useTypes: true})");
        if (copy == null) throw new ArgumentNullException(nameof(copy));

        Query paste = new("CALL apoc.import.graphml('countryData.graphml', {readLabels: true, useTypes: true})");

        using var session = Driver.AsyncSession(o => o.WithDatabase("neo4j").WithDefaultAccessMode(AccessMode.Read));

        session.ReadTransactionAsync(tx => tx.RunAsync(copy));

        using var session2 = Driver.AsyncSession(o => o.WithDatabase("nationalbaseline").WithDefaultAccessMode(AccessMode.Write));

        session2.WriteTransactionAsync(tx => tx.RunAsync(paste));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Driver?.Dispose();
        }

        _disposed = true;
    }

}
