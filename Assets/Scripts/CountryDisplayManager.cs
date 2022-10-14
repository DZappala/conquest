public class CountryDisplayManager
{
    public delegate void UseCountryDisplay(CountryData countryData);

    private static CountryDisplayManager _instance;

    private CountryDisplayManager()
    {
        IsCountrySelected = false;
    }

    public static CountryDisplayManager Instance
    {
        get { return _instance ??= new(); }
    }

    public bool IsCountrySelected { get; set; }

    public event UseCountryDisplay OnCountrySelected; // ReSharper disable Unity.PerformanceAnalysis

    public void SetIsCountrySelected(CountryData countryData)
    {
        IsCountrySelected = true;
        OnCountrySelected?.Invoke(countryData);
    }
}
