using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDisplay : MonoBehaviour
{
    [FormerlySerializedAs("PP_Title")] public TMP_Text ppTitle;

    [FormerlySerializedAs("PP_Gold")] public TMP_Text ppGold;

    [FormerlySerializedAs("PP_Manpower")] public TMP_Text ppManpower;

    public void UsePlayerCountryData(CountryData countryData)
    {
        if (countryData == null)
        {
            Debug.LogError("Country data is null");
            return;
        }

        ppTitle.text = countryData.Name;
        ppGold.text = countryData.Money.ToString(CultureInfo.InvariantCulture);
    }
}
