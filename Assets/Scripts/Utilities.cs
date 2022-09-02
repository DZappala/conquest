public static class Utilities
{
    public static double CalculateGrowthRate(double currentValue, double ratio)
    {
        double rate = ratio / 365;
        return currentValue * (1 + rate);
    }
}
