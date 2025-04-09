namespace ConsumerEndpoint.Consumer;

public interface IPowergrid
{
    public string? Id { get; set; }
    
    public Task StartClientAsync(CancellationToken ct);
    
    public Task ChangeEnergyAsync(CancellationToken ct);
    
    public Task RegisterAsync(CancellationToken ct);
}