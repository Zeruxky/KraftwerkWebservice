namespace Powergrid.PowerGrid
{
    public interface IPowergridHubClient
    {
        public Task ReceiveMessageAsync(string message);

        public Task ReceiveMembersAsync(IReadOnlyDictionary<string, string> message);

        public Task ReceiveMemberDataAsync(IReadOnlyDictionary<string, int> data);

        public Task ReceiveTimeAsync(int hours);

        public Task ReceiveStopAsync(bool stopped);

        public Task ReceiveEnergyAsync(double energy);

        public Task ReceiveExpectedConsumeAsync(IReadOnlyDictionary<int, double> plan);

        public Task ReceiveBlackoutAsync();

        public Task ReceivePieChartDataAsync(IReadOnlyDictionary<string, MarketShare> userProduction);
    }
}
