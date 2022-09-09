using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CountryDisplay : MonoBehaviour
{
    //public Country country;
    [FormerlySerializedAs("CP_Title")] public TMP_Text cpTitle;

    [FormerlySerializedAs("CP_GovernmentType")] public TMP_Text cpGovernmentType;

    [FormerlySerializedAs("CP_Manpower")] public TMP_Text cpManpower;

    [FormerlySerializedAs("CP_Money")] public TMP_Text cpMoney;

    private CountryData _countryData;

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
            //HACK is there a way to invoke this through the instance instead?
            OnCountrySelected (_countryData);
        }
    }

    public void OnDestroy()
    {
        CountryDisplayManager.Instance.OnCountrySelected -= OnCountrySelected;
    }

    private void OnCountrySelected(CountryData countryDataFromEventParam)
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

        _countryData = countryDataFromEventParam;
        UpdateCountryData();
    }

    private void UpdateCountryData()
    {
        cpTitle.text = _countryData.Name;
        cpGovernmentType.text = _countryData.GovernmentType;
        cpMoney.text = _countryData.Money.ToString(CultureInfo.InvariantCulture);
    }
}
