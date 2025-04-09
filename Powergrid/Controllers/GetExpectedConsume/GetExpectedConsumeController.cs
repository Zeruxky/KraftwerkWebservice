// <copyright file="GetExpectedConsumeController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Powergrid.Controllers.GetExpectedConsume
{
    using System.Net.Mime;
    using Microsoft.AspNetCore.Mvc;
    using Powergrid.PowerGrid;

    [Tags("Powergrid")]
    public class GetExpectedConsumeController : PowergridController
    {
        private readonly Grid grid;

        public GetExpectedConsumeController(Grid grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Get the expected consumption of all participating consumers.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/> that represents the result of the operation.</returns>
        /// <remarks>
        /// Beispiel:
        ///
        ///    Output:
        ///    {
        ///         "0": 0,
        ///         "1": 0,
        ///         "2": 0,
        ///         "3": 0,
        ///         "4": 0,
        ///         "5": 0,
        ///         "6": 0,
        ///         "7": 0,
        ///         "8": 0,
        ///         "9": 0,
        ///         "10": 0,
        ///         "11": 0,
        ///         "12": 0,
        ///         "13": 0,
        ///         "14": 0,
        ///         "15": 0,
        ///         "16": 0,
        ///         "17": 0,
        ///         "18": 0,
        ///         "19": 0,
        ///         "20": 0,
        ///         "21": 0,
        ///         "22": 0,
        ///         "23": 0
        ///    }
        ///
        /// </remarks>
        /// <response code="200">The expected consumption for the current day.</response>
        [HttpGet("GetExpectedConsume")]
        [ProducesResponseType<IDictionary<int, double>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public IActionResult Handle() => this.Ok(this.grid.GetExpectedConsume());
    }
}
