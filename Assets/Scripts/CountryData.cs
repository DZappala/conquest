using System.Collections.Generic;
using UnityEngine;

public class CountryData
{
    //TODO this list is extremely lacking in total data points
    public string tag;

    public string title;

    public string capital;

    public string currency;

    public string language;

    public double population;

    public string geography;

    public string governmentType;

    public double money;

    public double unemploymentRate;

    public double inflationRate;

    public double gdpPerCapita;

    public double militarySize;

    public double militaryStrength;

    public double militaryDefense;

    public double militaryArmy;

    public double militaryNavy;

    public double militaryReserves;

    /* --- STRATUM --- */
    public double impoverished;

    public double workers;

    public double merchants;

    public double aristocrats;

    public double government;

    public List<Organization> religionList;

    public List<Organization> partyList;

    public List<History> historyList;
}
