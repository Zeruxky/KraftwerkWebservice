namespace Powergrid.Utilities
{
    public class SineSquaredCurve
    {
        private double currentAngle;
        private readonly double stepAngle;

        public SineSquaredCurve(int repeatAfterSteps)
        {
            this.currentAngle = 0;
            this.stepAngle = Math.PI / repeatAfterSteps;
        }

        public double NextValue()
        {
            double value = Math.Pow(Math.Sin(this.currentAngle), 2);
            this.currentAngle = (this.currentAngle + this.stepAngle) % (2 * Math.PI);
            return value;
        }
    }
}