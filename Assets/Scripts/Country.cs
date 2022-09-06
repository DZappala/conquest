using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using static Utilities;

public class Country : MonoBehaviour
{
    public CountryData CountryData;

    [FormerlySerializedAs("IsPlayerCountry")]
    public bool isPlayerCountry;

    private Stratum _stratum;
    private GameObject _playerPanel;
    private PlayerDisplay _playerDisplay;

    public void Awake()
    {
        gameObject.tag = "Country";
        CountryData = new CountryData
        {
            Name = name
        };

        gameObject.AddComponent<PolygonCollider2D>();
    }

    public void Start()
    {
        _playerPanel = GameObject.FindWithTag("PlayerPanel");
        _playerDisplay = _playerPanel.GetComponent<PlayerDisplay>();

        _stratum = new Stratum();
    }

    public void Update()
    {
        if (isPlayerCountry)
        {
            _playerDisplay.UsePlayerCountryData(CountryData);
        }
    }

    //TODO does this need to be an async function?
    public void ParseCountryData(IReadOnlyDictionary<string, object> dRecord)
    {
        Assert.IsNotNull(dRecord["tag"], $"Country tag is null for {name}");

        Debug.Log($"Parsing country data for {name}");

        foreach (var keyValuePair in dRecord)
        {
            var fieldInfo = CountryData.GetType().GetField(keyValuePair.Key);

            var value = keyValuePair.Value;

            if (fieldInfo != null)
            {
                fieldInfo
                    .SetValue(CountryData, value);
            }
        }
    }

    public Dictionary<string, object> GetCountryDataFields()
    {
        var type = CountryData.GetType();

        var fields =
            type
                .GetFields()
                .ToDictionary(field => field.Name,
                    field => field.GetValue(CountryData));

        return fields;
    }


    public void CalculateCountryData()
    {
        var totalWorkerNeeds = GetTotalNeeds(_stratum.Workers.Strata);
        var totalGovernmentNeeds = GetTotalNeeds(_stratum.Government.Strata);
        var sumTotalNeeds = totalWorkerNeeds + totalGovernmentNeeds;

        CountryData.Money = CalculateGrowthRate(CountryData.Money, sumTotalNeeds);

        GameControl.CalculationsComplete++;
    }

    private static double GetTotalNeeds(StrataBase strata)
    {
        return strata.GetNeeds().Sum(need => need.Value.Value);
    }
}
