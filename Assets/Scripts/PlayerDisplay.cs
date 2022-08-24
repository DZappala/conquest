using TMPro;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    public TMP_Text PP_Title;

    public TMP_Text PP_Gold;

    public TMP_Text PP_Manpower;

    public void UsePlayerCountryData(CountryData countryData)
    {
        if (countryData == null)
        {
            Debug.LogError("Country data is null");
            return;
        }

        PP_Title.text = countryData.name;
        PP_Gold.text = countryData.money.ToString();
    }
}
