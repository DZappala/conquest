using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Neo4j.Driver;

public class DB : IDisposable
{
    private readonly IDriver _driver;
    private bool _disposed;

    public DB()
    {
        _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "glory"));
    }

    ~DB()
    {
        Dispose(false);
    } // ReSharper disable Unity.PerformanceAnalysis
    public IObservable<IReadOnlyDictionary<string, object>> GetCountry(Country country)
    {
        var session = _driver.RxSession(o => o.WithDatabase("nationalbaseline"));

        Query query = new("MATCH (c:Country {name: $CountryName}) RETURN c",
            new { CountryName = country.CountryData.Name });

        return session.ReadTransaction(tx =>
        {
            return tx.Run(query).Records().Select(record => record[0].As<INode>().Properties);
        }).OnErrorResumeNext(session.Close<IReadOnlyDictionary<string, object>>());
    }

    public IObservable<int> SetCountry(Country country)
    {
        var session = _driver.RxSession(o => o.WithDatabase("nationalbaseline"));

        var countryDataFields = country.GetCountryDataFields();

        Query query = new("MATCH(c:Country{name: $CountryName}) SET c = $CountryData",
            new { CountryName = country.CountryData.Name, CountryData = countryDataFields });

        return session.WriteTransaction(tx =>
            {
                return tx.Run(query).Consume().Select(result => result.Counters.PropertiesSet);
            })
            .OnErrorResumeNext(session.Close<int>());
    }

    public void Clear()
    {
        Query query = new("MATCH(a)->[r]->(), (b) RETURN a, r, b");
        if (query == null) throw new ArgumentNullException(nameof(query));

        using var session =
            _driver.AsyncSession(o => o.WithDatabase("nationalbaseline").WithDefaultAccessMode(AccessMode.Write));

        session.WriteTransactionAsync(tx => tx.RunAsync(query));
    }

    public void Copy()
    {
        Query copy = new("CALL apoc.export.graphml.all('countryDAta.graphml', {readLabels: true, useTypes: true})");
        if (copy == null) throw new ArgumentNullException(nameof(copy));

        Query paste = new("CALL apoc.import.graphml('countryData.graphml', {readLabels: true, useTypes: true})");

        using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j").WithDefaultAccessMode(AccessMode.Read));

        session.ReadTransactionAsync(tx => tx.RunAsync(copy));

        using var session2 =
            _driver.AsyncSession(o => o.WithDatabase("nationalbaseline").WithDefaultAccessMode(AccessMode.Write));

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

        if (disposing) _driver?.Dispose();

        _disposed = true;
    }
}
