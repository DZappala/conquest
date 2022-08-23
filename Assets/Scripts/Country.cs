/* 
Workers --- produce --> goods --- sold by  --> Merchants --- to --> Exporters & Workers.
The Government --- collects ---> Taxes & Tariffs --- from --> Workers, Merchants, Exports and Imports

// classes //
Impoverished:
    - Costs money for Government
    - Lower happiness
Workers:
    - Can cost some money for Government
    - Provides money in taxes
    - Provides demand for goods
    - Raises but typically mostly lowers happiness 
Merchants:
    - Provides money in tariffs
    - Provides goods for workers
    - Raises and lowers happiness 
Landlords & Nobles:
    - Provides money in taxes
    - Provides demand for luxury goods
    - Raises happiness
Government:
    - Provides boons and banes for Impoverished, Workers, and Merchants        
*/
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Country : MonoBehaviour
{
    public CountryData countryData;

    private GameObject countryDisplay;

    private GameObject playerDisplay;

    public List<byte> Workers;

    public double PopulationWorkerModifier;

    public double PopulationGrowthModifier;

    public double AverageSkill;

    public double taxRate;

    public double TaxIncome;

    //TODO allow player to select their country before the start function is called
    public bool isPlayerCountry;

    public Slider taxRateSlider;

    public Database db;

    public void Awake()
    {
        db = new Database();
        if (!Database.isRun)
        {
            Database.DatabaseConstruct();
        }

        countryData = new CountryData { title = name };
        countryDisplay = GameObject.FindWithTag("CountryPanel");
        playerDisplay = GameObject.FindWithTag("PlayerPanel");
        taxRateSlider = countryDisplay.GetComponentInChildren<Slider>();

        gameObject.tag = "Country";
        gameObject.AddComponent<PolygonCollider2D>();
    }

    public void Start()
    {
        //HACK numbers are currently static and not programmatic.
        PopulationWorkerModifier = 0.02;
        PopulationGrowthModifier = 0.02;

        if (Database.isRun)
        {
            db.DownloadCountryData(countryData, this);
        }

        if (isPlayerCountry)
        {
            playerDisplay
                .GetComponent<PlayerDisplay>()
                .UsePlayerCountryData(countryData);
        }
    }

    public void Update()
    {
        if (taxRateSlider != null)
        {
            if (taxRateSlider.value != taxRate)
            {
                UseTaxRate();
            }
        }
    }

    public void FixedUpdate()
    {
        //HACK right now this data is only stored locally and not reflected on the server, this will need to be implemented when transitioning to local database files.
        //downloads all the country data from Neo4j graph and parses from a dictionary, the neo4j return type, into a CountryData object
        if (
            Database.isRun &&
            GameSpeedManager.Instance.CurrentGameSpeed != GameSpeed.PAUSED
        )
        {

            db.DownloadCountryData(countryData, this); //this refers to this specific country instance.
            Debug.Log("DL: Downloading country data for " + countryData.title);

            //calculates statistics about each country
            CalculateCountryData();

            //if the country is the player, update the player display
            if (isPlayerCountry)
            {
                playerDisplay
                    .GetComponent<PlayerDisplay>()
                    .UsePlayerCountryData(countryData);
            }

            //uploads the country data to the database
            db.UploadCountryData (countryData);
            Debug.Log("UL: Uploading data for " + countryData.title);
        }
    }

    public void CalculateCountryData()
    {
        //HACK population growth is currently fixed.
        //TODO find a way to do population and growth on the macro scale that doesn't involve keeping track of individual people in a population ie. can this be abstracted to a macro scale?
        countryData.population +=
            countryData.population * PopulationGrowthModifier;

        //TODO verify this tax calculation is correct
        //This should find the current tax income by multiplying the total portion of money owned by the workers by the tax rate.
        //FIXME assumes the total liquid cash of the entire government is equivalent to the entire GDP of the nation. Money and GDP should be different numbers!!
        TaxIncome =
            taxRate *
            (countryData.workers * countryData.money / countryData.population);

        countryData.money += TaxIncome;
    }

    //HACK relies on there only being 1 player!
    public void UseTaxRate()
    {
        taxRate = isPlayerCountry ? taxRateSlider.value * 0.01 : 0.07;
    }

    public CountryData
    ParseCountryData(IReadOnlyDictionary<string, object> dict)
    {
        if (dict == null)
        {
            Debug.Log("Prase: dict is null");
            return null;
        }
        Debug
            .Log("Parse: Parsing country data from dict: " +
            dict["title"] +
            " for: " +
            countryData.title);

        //TODO implement all fields
        //FIXME is this the correct way of assigning values to the countryData object? this seems like a resource intensive way to do this
        countryData.tag = dict["tag"].ToString();
        countryData.title = dict["title"].ToString();
        countryData.capital = dict["capital"].ToString();
        countryData.population = double.Parse(dict["population"].ToString());
        countryData.money = double.Parse(dict["money"].ToString());
        countryData.workers = double.Parse(dict["workers"].ToString());

        return countryData;
    }
}
