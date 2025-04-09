namespace Powergrid.Utilities;

public class SunCycle
{
    private SineSquaredCurve sineSquaredCurve;
    private Random random;
    private List<double> cycleValues = new List<double>();

    public SunCycle(int repeatAfterSteps)
    {
        this.sineSquaredCurve = new SineSquaredCurve(repeatAfterSteps);
        this.random = new Random();
        this.GenerateValues();
    }

    private double SampleRandomDouble(double minVal, double maxVal)
    {
        return this.random.NextDouble() * (maxVal - minVal) + minVal;
    }

    private void GenerateValues()
    {
        for (int i = 0; i < 24 * 60; i++)
        {
            this.cycleValues.Add(this.GetNextNoisyPvValue(10));
        }
    }

    private double GetNextNoisyPvValue(double peakPower)
    {
        double noise = this.SampleRandomDouble(0, 1) > 0.85 ? this.SampleRandomDouble(-0.8, 0.0) : 0;
        double sineSquaredValue = this.sineSquaredCurve.NextValue();
        double noisySineSquaredValue = Math.Max(sineSquaredValue + noise, 0);
        double noisyPvValue = peakPower * noisySineSquaredValue;

        return noisyPvValue;
    }

    public double GetValueOfTime(int minutesPassed)
    {
        return this.cycleValues[minutesPassed];
    }
}