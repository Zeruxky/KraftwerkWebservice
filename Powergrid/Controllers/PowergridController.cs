namespace Powergrid.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route(Route)]
    public abstract class PowergridController : ControllerBase
    {
        private const string Route = "/powergrid";
    }
}
