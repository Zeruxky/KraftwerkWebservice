// <copyright file="GetEnergyController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Powergrid.Controllers.GetEnergy
{
    using System.Net.Mime;
    using Microsoft.AspNetCore.Mvc;
    using Powergrid.PowerGrid;

    [Tags("Powergrid")]
    public class GetEnergyController : PowergridController
    {
        private readonly Grid grid;

        public GetEnergyController(Grid grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Returns the current energy in the powergrid.
        /// </summary>
        /// <returns>A <see cref="IActionResult"/> that represents the result of the operation.</returns>
        /// <remarks>
        /// Beispiel:
        ///
        ///     Output:
        ///     {
        ///        "246"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Successfully returned the current energy in the system.</response>
        [HttpGet("GetEnergy")]
        [ProducesResponseType(typeof(double), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public IActionResult GetEnergy() => this.Ok(this.grid.AvailableEnergy);
    }
}
