public class CountryDisplayManager
{
    public static CountryDisplayManager _instance;

    public static CountryDisplayManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CountryDisplayManager();
            }
            return _instance;
        }
    }

    private CountryDisplayManager()
    {
        IsCountrySelected = false;
    }

    public bool IsCountrySelected { get; private set; }

    public delegate void UseCountryDisplay(CountryData countryData);

    public event UseCountryDisplay OnCountrySelected;

    public void SetIsCountrySelected(CountryData countryData)
    {
        IsCountrySelected = true;
        OnCountrySelected?.Invoke(countryData);
    }
}
