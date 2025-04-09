// <copyright file="RegisterController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Powergrid.Controllers.Register
{
    using System.Net.Mime;
    using Microsoft.AspNetCore.Mvc;
    using Powergrid.Members;
    using Powergrid.PowerGrid;

    [Tags("Powergrid")]
    public class RegisterController : PowergridController
    {
        private readonly Grid grid;
        private readonly ILogger<RegisterController> logger;

        public RegisterController(Grid grid, ILogger<RegisterController> logger)
        {
            this.grid = grid;
            this.logger = logger;
        }

        /// <summary>
        /// Lets you register to the powergrid, without it, you cant influence the energy in the powergrid.
        /// </summary>
        /// <param name="request">A <see cref="Task"/> that represents the asynchronous registration operation.</param>
        /// <returns>changes energy</returns>
        /// <remarks>
        /// Beispiel:
        ///
        ///     Input:
        ///     {
        ///        "name": "MyPowerplant",
        ///        "type": "Powerplant"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Successfully registered to the powergrid.</response>
        /// <response code="406">Couldn't register to the powergrid, might have not used "Powerplant" as type.</response>
        [HttpPost("Register")]
        [Consumes(typeof(MemberObject), MediaTypeNames.Application.Json)]
        [ProducesResponseType<string>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> RegisterAsync([FromBody] MemberObject request, CancellationToken ct)
        {
            var id = Guid.NewGuid().ToString();
            switch (request.Type)
            {
                case "Powerplant":
                    this.grid.Members.Add(id, new Powerplant(request.Name));
                    this.grid.MultiplicatorAmount.Add(id, 5);
                    break;
                default:
                {
                    return this.StatusCode(406);
                }
            }

            this.logger.LogInformation("Registered new member: {Member}", request.Name);
            var transformedMembers = this.grid.Members.ToDictionary(item => item.Key, item => $"{item.Value.Name}({item.Value.GetType()})");

            await this.grid.Clients.All.ReceiveMembersAsync(transformedMembers).ConfigureAwait(false);
            return this.Ok(id);
        }
    }
}
