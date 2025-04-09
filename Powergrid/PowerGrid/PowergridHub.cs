namespace Powergrid.PowerGrid
{
    using Microsoft.AspNetCore.SignalR;
    using Powergrid.Controllers;
    using Powergrid.Members;

    public class PowergridHub : Hub<IPowergridHubClient>
    {
        private readonly Grid grid;
        private readonly ILogger<PowergridHub> logger;

        public PowergridHub(Grid grid, ILogger<PowergridHub> logger)
        {
            this.grid = grid;
            this.logger = logger;
        }

        public async Task ChangeEnergyAsync(string? id)
        {
            if (id != null && !this.grid.Members.ContainsKey(id))
            {
                await this.Clients.Caller.ReceiveMessageAsync("Not registered").ConfigureAwait(false);
                return;
            }

            this.grid.ChangeEnergy(id);
        }

        public async Task StartStopAsync()
        {
            if (this.grid.TimeInInt == 5)
            {
                //backend ist einen tick voraus, deswegen solls erst bei 5 clearn
                this.grid.Plan_User.Clear();
            }

            this.grid.Stopped = !this.grid.Stopped;
            if (!this.grid.Stopped)
            {
                this.grid.TimeLoop(this.Clients);
            }

            await this.Clients.Caller.ReceiveStopAsync(this.grid.Stopped).ConfigureAwait(false);
        }

        public async Task ChangeMultiplicatorAmountAsync(string id, int request)
        {
            this.grid.MultiplicatorAmount[id] = request;
            this.grid.InitPlanMember();

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task GetMemberDataAsync()
        {
            var transformedMembersDic = this.grid
                .Members
                .ToDictionary(item => item.Key, item => $"{item.Value.Name}({item.Value.GetType()})");

            await this.Clients.All.ReceiveMembersAsync(transformedMembersDic).ConfigureAwait(false);
            await this.Clients.Caller.ReceiveMemberDataAsync(this.grid.MultiplicatorAmount).ConfigureAwait(false);
        }

        public async Task RegisterAsync(MemberObject request)
        {
            var id = Guid.NewGuid().ToString();
            switch (request.Type)
            {
                case "Powerplant":
                    this.grid.Members.Add(id, new Powerplant(request.Name));
                    this.grid.MultiplicatorAmount.Add(id, 5);
                    break;
                case "Consumer":
                    this.grid.Members.Add(id, new Consumer(request.Name));
                    this.grid.MultiplicatorAmount.Add(id, 500);
                    this.grid.InitPlanMember();
                    break;
            }

            this.logger.LogInformation("Registered new member: {Member}", request.Name);
            var transformedMembers = this.grid
                .Members
                .ToDictionary(item => item.Key, item => $"{item.Value.Name}({item.Value.GetType()})");

            await this.Clients.All.ReceiveMembersAsync(transformedMembers).ConfigureAwait(false);
            await this.Clients.All.ReceiveMemberDataAsync(this.grid.MultiplicatorAmount).ConfigureAwait(false);
            await this.Clients.Caller.ReceiveMessageAsync(id).ConfigureAwait(false);
        }

        public async Task GetEnergyAsync() => await this.Clients.Caller.ReceiveEnergyAsync(Math.Round(this.grid.AvailableEnergy, 2)).ConfigureAwait(false);

        public async Task ResetEnergyAsync()
        {
            this.grid.Members.Clear();
            this.grid.MultiplicatorAmount.Clear();
            this.grid.Stopped = false;
            this.grid.TimeInInt = 0;
            this.grid.AvailableEnergy = 0;
            await this.Clients.All.ReceiveMembersAsync(new Dictionary<string, string>()).ConfigureAwait(false);
            await this.Clients.All.ReceiveMemberDataAsync(this.grid.MultiplicatorAmount).ConfigureAwait(false);
        }

        public async Task GetExpectedConsumeAsync() => await this.Clients.All.ReceiveExpectedConsumeAsync(this.grid.GetExpectedConsume()).ConfigureAwait(false);

        public override async Task OnConnectedAsync()
        {
            this.grid.GetClients(this.Clients);
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        public override async Task OnDisconnectedAsync(Exception? exception) => await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
    }
}