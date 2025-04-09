// <copyright file="GetTimeController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Powergrid.Controllers.GetTime
{
    using System.Net.Mime;
    using Microsoft.AspNetCore.Mvc;
    using Powergrid.PowerGrid;

    [Tags("Powergrid")]
    public class GetTimeController : PowergridController
    {
        private readonly Grid grid;

        public GetTimeController(Grid grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Returns the current time in hours of the day.
        /// </summary>
        /// <returns>A <see cref="IActionResult"/> that represents the result of the operation.</returns>
        /// <remarks>
        /// Beispiel:
        ///
        ///     Output:
        ///     {
        ///        "21"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Successfully returned the current time of the system.</response>
        [HttpGet("GetTime")]
        [ProducesResponseType<int>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public IActionResult GetTime() => this.Ok(this.grid.TimeInInt / 60);
    }
}
