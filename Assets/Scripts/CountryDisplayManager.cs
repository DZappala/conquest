public class CountryDisplayManager
{
    private static CountryDisplayManager _instance;

    public static CountryDisplayManager Instance
    {
        get { return _instance ??= new CountryDisplayManager(); }
    }

    private CountryDisplayManager()
    {
        IsCountrySelected = false;
    }

    public bool IsCountrySelected { get; private set; }

    public delegate void UseCountryDisplay(CountryData countryData);

    public event UseCountryDisplay OnCountrySelected;

    // ReSharper disable Unity.PerformanceAnalysis
    public void SetIsCountrySelected(CountryData countryData)
    {
        IsCountrySelected = true;
        OnCountrySelected?.Invoke(countryData);
    }
}
