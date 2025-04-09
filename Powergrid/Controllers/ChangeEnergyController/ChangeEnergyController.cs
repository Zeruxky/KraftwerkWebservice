// <copyright file="ChangeEnergyController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Powergrid.Controllers.ChangeEnergyController
{
    using System.Net.Mime;
    using Microsoft.AspNetCore.Mvc;
    using Powergrid.PowerGrid;

    [Tags("Powergrid")]
    public class ChangeEnergyController : PowergridController
    {
        private readonly Grid grid;

        public ChangeEnergyController(Grid grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Produces a predefined amount of energy.
        /// </summary>
        /// <param name="id">The identifier of the participant.</param>
        /// <remarks>
        /// Beispiel:
        ///
        ///     Input:
        ///     {
        ///        "0BC0E7F2-EE32-4DA1-89F9-0DBD8B558026"
        ///     }
        ///
        /// </remarks>
        /// <returns>A <see cref="Task"/> that represents the asynchronous handle operation.</returns>
        /// <response code="204">The energy was successfully registered by the power grid.</response>
        /// <response code="401">You are not authorized to produce energy. Please register first at the Registration endpoint.</response>
        [HttpPost("ChangeEnergy")]
        [Consumes(typeof(string), MediaTypeNames.Application.Json)]
        [ProducesResponseType<string>(StatusCodes.Status204NoContent, MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Handle([FromBody] string id)
        {
            if (!this.grid.Members.ContainsKey(id))
            {
                return this.Unauthorized();
            }

            if (this.grid.Stopped)
            {
                return this.Ok("Not started yet.");
            }

            this.grid.ChangeEnergy(id);
            return this.Ok("Registered");
        }
    }
}
