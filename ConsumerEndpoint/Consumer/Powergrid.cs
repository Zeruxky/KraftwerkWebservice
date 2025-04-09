using Microsoft.AspNetCore.SignalR.Client;

namespace ConsumerEndpoint.Consumer;

public class Powergrid : IPowergrid
{
    private readonly ILogger<Powergrid> logger;
    private readonly HubConnection hub;
    private bool started;
    
    public Powergrid(HubConnection hub, ILogger<Powergrid> logger)
    {
        this.logger = logger;
        this.hub = hub;
    }
    
    public string? Id { get; set; }

    public async Task StartClientAsync(CancellationToken ct)
    {
        if (!this.started)
        {
            this.started = true;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task RegisterAsync(CancellationToken ct)
    {
        if (this.hub.State == HubConnectionState.Disconnected)
        {
            try
            {
                await this.hub.StartAsync(ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Server not running or not possible to connect...\nWait a short moment and try again.");
                return;
            }
        }

        await this.hub.SendAsync("RegisterAsync", new MemberObject() { Name = "Household", Type = "Consumer", }, ct).ConfigureAwait(false);
        this.hub.On<string>("ReceiveMessageAsync", message => this.Id = message);
    }

    public async Task ChangeEnergyAsync(CancellationToken ct)
    {
        await this.hub.SendAsync("ChangeEnergyAsync", this.Id, ct).ConfigureAwait(false);
        this.hub.On<string>(
            "ReceiveMessageAsync",
            message =>
            {
                if (message != "Registered")
                {
                    this.Id = null;
                }
            });
    }

    private sealed record MemberObject
    {
        public string Name { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
    }
}