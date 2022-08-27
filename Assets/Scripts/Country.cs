using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class Country : MonoBehaviour
{
    public CountryData CountryData;

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
    }

    public async Task<Task> ParseCountryData(IReadOnlyDictionary<string, object> record)
    {
       await Task.Run(() => { foreach (var keyValuePair in record)
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

       return Task.CompletedTask;
    }

    public void CalculateCountryData()
    {
        if (CountryData != null)
        {
            CountryData.money++;
        }
        GameControl.CalculationsComplete++;
    }
}
