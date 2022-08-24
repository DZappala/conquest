public class CountryData
{
    public CountryData(
        string tag,
        string name,
        string capital,
        string currency,
        string language,
        double population,
        string governmentType,
        double money,
        double impoverished,
        double workers,
        double merchants,
        double aristocrats,
        double government
    )
    {
        this.tag = tag;
        this.name = name;
        this.capital = capital;
        this.currency = currency;
        this.language = language;
        this.population = population;
        this.governmentType = governmentType;
        this.money = money;
        this.impoverished = impoverished;
        this.workers = workers;
        this.merchants = merchants;
        this.aristocrats = aristocrats;
        this.government = government;
    }

    public string tag;

    public string name;

    public string capital;

    public string currency;

    public string language;

    public double population;

    public string governmentType;

    public double money;

    //Stratum
    public double impoverished;

    public double workers;

    public double merchants;

    public double aristocrats;

    public double government;
}
