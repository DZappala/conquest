using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using static Utilities;

public class Country : MonoBehaviour
{
    [FormerlySerializedAs("IsPlayerCountry")]
    public bool isPlayerCountry;

    private PlayerDisplay _playerDisplay;
    private GameObject _playerPanel;

    private Stratum _stratum;
    public CountryData CountryData;

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
        if (isPlayerCountry) _playerDisplay.UsePlayerCountryData(CountryData);
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
