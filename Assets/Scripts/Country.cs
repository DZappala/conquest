using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour
{
    public CountryData CountryData;

    public void Awake()
    {
        gameObject.tag = "Country";
        gameObject.AddComponent<PolygonCollider2D>();
    }

    public void Start()
    {
        CountryData =
            new CountryData(" ", " ", " ", " ", " ", 0, " ", 0, 0, 0, 0, 0, 0);
    }

    public void ParseCountryData(IReadOnlyDictionary<string, object> record)
    {
        CountryData =
            new CountryData(record["tag"].ToString(),
                record["name"].ToString(),
                record["capital"].ToString(),
                record["currency"].ToString(),
                record["language"].ToString(),
                double.Parse(record["population"].ToString()),
                record["governmentType"].ToString(),
                double.Parse(record["money"].ToString()),
                double.Parse(record["impoverished"].ToString()),
                double.Parse(record["workers"].ToString()),
                double.Parse(record["merchants"].ToString()),
                double.Parse(record["aristocrats"].ToString()),
                double.Parse(record["government"].ToString()));
    }

    public void CalculateCountryData()
    {
        if (CountryData != null)
        {
            CountryData.money++;
        }
    }
}
