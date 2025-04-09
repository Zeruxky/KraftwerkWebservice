namespace Powergrid.Members;

using Powergrid.PowerGrid;

public class Consumer : IMember
{
    public double[] ConsumePercentDuringDayNight { get; } =
    [
        0.125, 0.1875, 0.25, 0.1875, 0.375, 0.5, 0.75, 0.875, 0.8125, 0.875, 1, 1, 0.625, 0.6875, 0.5, 0.375, 0.375, 0.75, 0.5,
        0.5, 0.625, 0.3125, 0.1875, 0.1875,
    ];

    public int Hour { get; set; }

    public virtual double Energy { get; set; }

    public double GetCalculatedEnergy(double plannedEnergy) => plannedEnergy * new Random().Next(9, 11) / 10 * -1;

    public string Name { get; set; }

    public Consumer(string name)
    {
        this.Energy = -3;
        this.Name = name;
    }
}