//Stratum.cs

using System.Collections.Generic;

//Access to Debug.Log (don't use any other UnityEngine classes, may break)

public class Stratum
{
    public readonly Workers Workers;

    public readonly Government Government;

    public Stratum()
    {
        Workers = new Workers();
        Government = new Government();
    }
}

public class Workers
{
    public readonly StrataBase Strata;

// Constructor //
    public Workers()
    {
        Good food = new("Food", 10);
        Good housing = new("Housing", 10);

        Strata = new StrataBase();

        Strata.AddNeed(food);
        Strata.AddNeed(housing);
    }
}

public class Government
{
    public readonly StrataBase Strata;

// Constructor //
    public Government()
    {
        Good money = new("Money", 10);
        Good food = new("Food", 5);
        Good housing = new("Housing", 5);

        Strata = new StrataBase();

        Strata.AddNeed(money);

        Strata.AddProduction(food);
        Strata.AddProduction(housing);
    }
}

public class StrataBase
{
    //TODO implement population
    //public double Amount;
    private readonly Dictionary<string, Good> _needs;

    private readonly Dictionary<string, Good> _wants;

    private readonly Dictionary<string, Good> _production;

// Constructor //
    public StrataBase()
    {
        _needs = new Dictionary<string, Good>();
        _wants = new Dictionary<string, Good>();
        _production = new Dictionary<string, Good>();
    }

// Utility Functions //
    public void AddNeed(Good good)
    {
        _needs.Add(good.Name, good);
    }

    public void AddWant(Good good)
    {
        _wants.Add(good.Name, good);
    }

    public void AddProduction(Good good)
    {
        _production.Add(good.Name, good);
    }

    public Dictionary<string, Good> GetNeeds()
    {
        return _needs;
    }

    public Dictionary<string, Good> GetWants()
    {
        return _wants;
    }

    public Dictionary<string, Good> GetProduction()
    {
        return _production;
    }
}

public class Good
{
    public string Name { get; }
    public double Value { get; }

// Constructor //
    public Good(string name, double value)
    {
        Name = name;
        Value = value;
    }
}
