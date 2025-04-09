using Microsoft.AspNetCore.SignalR;

namespace Powergrid.PowerGrid;

using Powergrid.Members;

public class Grid
{
    public Dictionary<int, double> Plan_User { get; set; } = new();

    public Dictionary<int, double> Plan_Member { get; set; } = new();

    public Dictionary<string, IMember> Members { get; set; } = new();

    public Dictionary<string, int> MultiplicatorAmount { get; set; } = new();

    public int TimeInInt { get; set; } = 0;

    public bool Stopped { get; set; } = false;

    public bool ThreadStarted { get; set; } = false;

    public double AvailableEnergy { get; set; }

    private IHubCallerClients<IPowergridHubClient>? clients;

    public IHubCallerClients<IPowergridHubClient> Clients
    {
        get
        {
            if (this.clients == null)
            {
                throw new InvalidOperationException("Clients have not been initialized. Ensure GetClients is called before accessing this property.");
            }

            return this.clients;
        }
        set => this.clients = value;
    }

    public void GetClients(IHubCallerClients<IPowergridHubClient> incomingClients) => this.Clients = incomingClients;

    public void ChangeEnergy(string? id)
    {
        if (this.Stopped)
        {
            return;
        }

        var member = this.Members.Where(x => x.Key == id).Select(x => x.Value).FirstOrDefault();
        switch (member)
        {
            case Consumer consumer:
            {
                if (this.Plan_Member.Count == 0)
                {
                    this.InitPlanMember();
                }

                consumer.Hour = Convert.ToInt32(Math.Floor((double)((this.TimeInInt / 60) % 24)));
                this.AvailableEnergy += consumer.GetCalculatedEnergy(this.Plan_Member[consumer.Hour]);
                return;
            }

            case Powerplant powerplant:
                powerplant.Produced += member.Energy * this.MultiplicatorAmount.FirstOrDefault(x => x.Key == id).Value;
                this.AvailableEnergy += member.Energy * this.MultiplicatorAmount.FirstOrDefault(x => x.Key == id).Value;
                break;
        }
    }

    public void TimeLoop(IHubCallerClients<IPowergridHubClient> clients)
    {
        var threadTime = new Thread(
            async void () =>
            {
                if (this.ThreadStarted)
                {
                    return;
                }

                this.ThreadStarted = true;
                while (!this.Stopped)
                {
                    await clients.All.ReceiveTimeAsync(this.TimeInInt);
                    if (this.TimeInInt % 1440 == 0)
                    {
                        this.Plan_User.Clear();
                        this.Plan_Member.Clear();
                        this.TimeInInt = 0;
                        await clients.All.ReceiveExpectedConsumeAsync(this.GetExpectedConsume());
                    }

                    if (this.TimeInInt % 60 == 0)
                    {
                        var UserProduction = new Dictionary<string, MarketShare>();
                        foreach (var (key, value) in this.Members.Where(x => x.Value is Powerplant))
                        {
                            var powerplant = (Powerplant)value;
                            UserProduction.Add(key, new MarketShare { Name = value.Name, Value = powerplant.Produced, });
                            powerplant.Produced = 0;
                        }

                        await clients.All.ReceivePieChartDataAsync(UserProduction);
                    }

                    await this.Clients.All.ReceiveEnergyAsync(this.AvailableEnergy);
                    this.TimeInInt += 5;
                    if ((this.AvailableEnergy / 10000) + 50 > 52.5 || (this.AvailableEnergy / 10000) + 50 < 47.5)
                    {
                        this.Members.Clear();
                        this.Stopped = false;
                        this.TimeInInt = 0;
                        this.AvailableEnergy = 0;
                        await this.Clients.All.ReceiveBlackoutAsync();
                    }

                    Thread.Sleep(1000);
                }

                this.ThreadStarted = false;
            });
        threadTime.Start();
    }

    public void InitPlanMember()
    {
        this.Plan_Member.Clear();
        for (var i = 0; i < 24; i++)
        {
            this.Plan_Member.Add(i, 0);
            foreach (var (key, value) in this.Members.Where(x => x.Value is Consumer))
            {
                this.Plan_Member[i] -= ((value.Energy * this.MultiplicatorAmount.FirstOrDefault(x => x.Key == key).Value) + new Random().Next(100)) *
                                       new Random().Next(8, 15) /
                                       10 *
                                       ((Consumer)value).ConsumePercentDuringDayNight[i];

                ((Consumer)value).Hour = i;
            }
        }
    }

    public Dictionary<int, double> GetExpectedConsume()
    {
        if (this.Plan_Member.Count == 0)
        {
            this.InitPlanMember();
        }

        if (!this.Members.Values.OfType<Consumer>().Any())
        {
            return this.Plan_Member;
        }

        if (this.Plan_User.Count != 0)
        {
            return this.Plan_User;
        }

        foreach (var (key, value) in this.Plan_Member)
        {
            this.Plan_User.Add(key, Math.Round(value, 2));
        }

        return this.Plan_User;
    }
}