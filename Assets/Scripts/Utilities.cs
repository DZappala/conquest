public static class Utilities
{
    public static double CalculateGrowthRate(double currentValue, double ratio)
    {
        var rate = ratio / 365;
        return currentValue * (1 + rate);
    }
}
