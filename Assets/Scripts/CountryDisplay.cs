using TMPro;
using UnityEngine;

public class CountryDisplay : MonoBehaviour
{
    //public Country country;
    public TMP_Text CP_Title;

    public TMP_Text CP_GovernmentType;

    public TMP_Text CP_Manpower;

    public TMP_Text CP_Money;

    public CountryData countryData;

    public void Awake()
    {
        CountryDisplayManager.Instance.OnCountrySelected += OnCountrySelected;
    }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void FixedUpdate()
    {
        if (CountryDisplayManager.Instance.IsCountrySelected)
        {
            //Hack is there a way to invoke this through the instance instead?
            OnCountrySelected (countryData);
        }
    }

    public void OnDestroy()
    {
        CountryDisplayManager.Instance.OnCountrySelected -= OnCountrySelected;
    }

    public void OnCountrySelected(CountryData countryDataFromEventParam)
    {
        if (countryDataFromEventParam == null)
        {
            Debug.LogError("Country data is null");
            return;
        }
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        countryData = countryDataFromEventParam;
        UpdateCountryData();
    }

    public void UpdateCountryData()
    {
        CP_Title.text = countryData.title;
        CP_GovernmentType.text = countryData.governmentType;
        CP_Manpower.text = countryData.militarySize.ToString();
        CP_Money.text = countryData.money.ToString();
    }
}
