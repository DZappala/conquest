using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class Country : MonoBehaviour
{
    [FormerlySerializedAs("IsPlayerCountry")]
    public bool isPlayerCountry;

    public double initialPopulation;
    private Growth _growth;
    private Stratum _stratum;
    private UIMain _uiMain;
    public CountryData CountryData;

    public void Awake()
    {
        gameObject.tag = "Country";
        CountryData = new()
        {
            Name = name
        };

        gameObject.AddComponent<PolygonCollider2D>();
        _growth = new();
    }

    public void Start()
    {
        _uiMain = GameObject.Find("UIMain").GetComponent<UIMain>();
        _stratum = new();
    }

    public void Update()
    {
        if (isPlayerCountry) _uiMain.UsePlayerCountryData(CountryData);
    }

    public void ParseCountryData(IReadOnlyDictionary<string, object> record)
    {
        Assert.IsNotNull(record["tag"], $"Country tag is null for {CountryData.Name}");

        foreach (var (key, value) in record)
        {
            var keyUpper = key.FirstCharacterToUpper();
            var fieldInfo = CountryData.GetType().GetField(keyUpper);

            if (fieldInfo != null)
                fieldInfo
                    .SetValue(CountryData, value);
        }
    }


    public Dictionary<string, object> GetCountryDataFields()
    {
        var type = CountryData.GetType();

        return type.GetFields().ToDictionary(field => char.ToLowerInvariant(field.Name[0]) + field.Name[1..]
            , field => field.GetValue(CountryData));
    }


    public void CalculateCountryData()
    {
        CountryData.Population = _growth.CalculateGrowth(CountryData, initialPopulation);
        GameControl.CalculationsComplete++;
    }

    private static double GetTotalNeeds(StrataBase strata)
    {
        return strata.GetNeeds().Sum(need => need.Value.Value);
    }
}
