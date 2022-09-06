using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using static Utilities;

public class Country : MonoBehaviour
{
    public CountryData CountryData;

    public bool IsPlayerCountry;

    public Stratum stratum;

    public void Awake()
    {
        gameObject.tag = "Country";
        CountryData = new()
        {
            name = gameObject.name
        };

        gameObject.AddComponent<PolygonCollider2D>();


    }

    public void Start()
    {
        stratum = new Stratum();
    }

    public void Update()
    {
        if (IsPlayerCountry)
        {
            GameObject.FindWithTag("PlayerPanel").GetComponent<PlayerDisplay>().UsePlayerCountryData(CountryData);
        }
    }

    //TODO does this need to be an async function?
    public void ParseCountryData(IReadOnlyDictionary<string, object> DRecord)
    {
        Assert.IsNotNull(DRecord["tag"], "Country tag is null for " + name);

        Debug.Log("Parsing country data for " + name);

            foreach (var keyValuePair in DRecord)
            {

                FieldInfo _fieldInfo = CountryData.GetType().GetField(keyValuePair.Key);

                object _value = keyValuePair.Value;

                if (_fieldInfo != null)
                {
                    _fieldInfo
                        .SetValue(CountryData, _value);
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

        var totalWorkerNeeds = GetTotalNeeds(stratum.workers.strata);
        var totalGovernmentNeeds = GetTotalNeeds(stratum.government.strata);
        var sumTotalNeeds = totalWorkerNeeds + totalGovernmentNeeds;

        CountryData.money = CalculateGrowthRate(CountryData.money, sumTotalNeeds);

        GameControl.CalculationsComplete++;

    }

    public double GetTotalNeeds(Strata_Base strata)
    {
        double totalNeeds = 0;

        foreach (var need in strata.GetNeeds())
        {
            totalNeeds += need.Value.Value;
        }

        return totalNeeds;
    }
}
