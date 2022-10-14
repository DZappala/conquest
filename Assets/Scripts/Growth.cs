using System;

public class Growth
{
    private readonly double _e;
    private readonly double _relativeGrowthRate;
    private double _capacityCoefficient;
    private double _capacityCoefficientX;
    private double _carryingCapacity;

    public Growth()
    {
        _relativeGrowthRate = 0.05;
    }

    public double CalculateGrowth(CountryData countryData, double initialPopulation)
    {
        _carryingCapacity = Math.Min(Math.Min(countryData.Water, countryData.Food), countryData.Housing);
        _capacityCoefficient = (_carryingCapacity - countryData.Population) / countryData.Population;
        var newPopulation = _carryingCapacity /
                            (1 + _capacityCoefficient *
                                Math.Exp(-_relativeGrowthRate / GameControl.DaysPassed));

        return newPopulation;
    }
}


// Basic Needs Class
// Food
// Water
// Shelter

// 1 unit of population consumes 1 unit of Food, Water, and Shelter, per day

// **** public class Basics
// {
// }


// Wealth Class => Money
// Trade and Taxes

//

// Culture Class => Happiness
// Religion and Entertainment


// Renown Class => Prestige
// Conquest and Diplomacy


// Population Growth Model
/*
 ## General Population Growth Formula:
 dN/dT = rmax((K-N)/K) * N

 **************

## The formula above solved for P after time T has passed:
 P(t) = K/(1+Ae^-rt)
    - P(t) = Population after time T
    - K = Carrying Capacity
    - r = relative growth rate coefficient
    - t = time a population grows
 A = (K-P0/P0)

 eg.
    (1000K - 500[P0])/500[P0] = A
    1000K/1 + 1*e^-.65*1 = [Pt]

    P[t] = 641.02

## Carrying Capacity:
- Determined by the availability of resources in the Area
    - Fresh Water
    - Food
    - Shelter (land area and rent)
=> 1000 units of water, food and shelter can support 1000 individuals

*/
