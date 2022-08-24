using TMPro;
using UnityEngine;

public class CountryDisplay : MonoBehaviour
{
    //public Country country;
    public TMP_Text CP_Title;

    public TMP_Text CP_GovernmentType;

    public TMP_Text CP_Manpower;

    public TMP_Text CP_Money;

    public CountryData CountryData;

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
            OnCountrySelected (CountryData);
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

        CountryData = countryDataFromEventParam;
        UpdateCountryData();
    }

    public void UpdateCountryData()
    {
        CP_Title.text = CountryData.name;
        CP_GovernmentType.text = CountryData.governmentType;
        CP_Money.text = CountryData.money.ToString();
    }
}
