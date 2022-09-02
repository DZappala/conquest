//Stratum.cs

using System.Collections.Generic;
using UnityEngine; //Access to Debug.Log (don't use any other UnityEngine classes, may break)

public class Stratum
{
    public Workers workers;

    public Government government;

    public Stratum()
    {
        workers = new Workers();
        government = new Government();
    }
}

public class Workers
{
    public Good Food;

    public Good Housing;

    public Strata_Base strata;

// Constructor //
    public Workers()
    {
        Food = new("Food", 10);
        Housing = new("Housing", 10);

        strata = new();

        strata.AddNeed(Food);
        strata.AddNeed(Housing);
    }
}

public class Government
{
    public Good

            Money,
            Food,
            Housing;

    public Strata_Base strata;

// Constructor //
    public Government()
    {
        Money = new("Money", 10);
        Food = new("Food", 5);
        Housing = new("Housing", 5);

        strata = new();

        strata.AddNeed(Money);

        strata.AddProduction(Food);
        strata.AddProduction(Housing);
    }
}

public class Strata_Base
{
    //TODO implement population
    //public double Amount;
    private readonly Dictionary<string, Good> Needs;

    private readonly Dictionary<string, Good> Wants;

    private readonly Dictionary<string, Good> Production;

// Constructor //
    public Strata_Base()
    {
        Needs = new Dictionary<string, Good>();
        Wants = new Dictionary<string, Good>();
        Production = new Dictionary<string, Good>();
    }

// Utility Functions //
    public void AddNeed(Good good)
    {
        Needs.Add(good.Name, good);
    }

    public void AddWant(Good good)
    {
        Wants.Add(good.Name, good);
    }

    public void AddProduction(Good good)
    {
        Production.Add(good.Name, good);
    }

    public Dictionary<string, Good> GetNeeds()
    {
        return Needs;
    }

    public Dictionary<string, Good> GetWants()
    {
        return Wants;
    }

    public Dictionary<string, Good> GetProduction()
    {
        return Production;
    }
}

public class Good
{
    public string Name { get; set; }
    public double Value { get; set; }

// Constructor //
    public Good(string _name, double _value)
    {
        Name = _name;
        Value = _value;
    }
}
