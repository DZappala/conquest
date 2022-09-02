using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
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
    public async Task ParseCountryData(IReadOnlyDictionary<string, object> DRecord)
    {
        await Task.Run(() =>
        {
            foreach (var keyValuePair in DRecord)
            {
                FieldInfo _fieldInfo = CountryData.GetType().GetField(keyValuePair.Key);
                object _value = keyValuePair.Value;

                if (_fieldInfo != null && _fieldInfo.GetValue(CountryData) != _value)
                {
                    _fieldInfo
                        .SetValue(CountryData, _value);
                }
            }
        });
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
