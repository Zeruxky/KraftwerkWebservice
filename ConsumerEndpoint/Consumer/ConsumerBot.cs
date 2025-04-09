namespace ConsumerEndpoint.Consumer;

public class ConsumerBot : BackgroundService
{
    private readonly IPowergrid powergrid;
    private readonly ILogger<ConsumerBot> logger;

    public ConsumerBot(IPowergrid powergrid, ILogger<ConsumerBot> logger)
    {
        this.powergrid = powergrid;
        this.logger = logger;
    }

    private bool IsRegistered => !string.IsNullOrEmpty(this.powergrid.Id);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await this.ProduceEnergy(stoppingToken).ConfigureAwait(false);

    private async Task ProduceEnergy(CancellationToken ct)
    {
        await this.powergrid.StartClientAsync(ct).ConfigureAwait(false);

        while (!ct.IsCancellationRequested)
        {
            if (!this.IsRegistered)
            {
                this.logger.LogInformation("Not Registered, push any button to register.");
                Console.ReadKey(true);
                await this.powergrid.RegisterAsync(ct).ConfigureAwait(false);
                await Task.Delay(1000, ct).ConfigureAwait(false);
                if (!this.IsRegistered)
                {
                    continue;
                }
            }

            await this.powergrid.ChangeEnergyAsync(ct).ConfigureAwait(false);
            this.logger.LogInformation("Has consumed");
            await Task.Delay(2000, ct).ConfigureAwait(false);
        }
    }
}