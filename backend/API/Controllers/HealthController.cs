using Microsoft.AspNetCore.Mvc;
using Paper.Infrastructure.Configuration;
using Paper.Infrastructure.Persistence;

namespace PaperAPI.Controllers;

[ApiController]
[Route("api/health")]
[Produces("application/json")]
[Tags("Health")]
public sealed class HealthController : ControllerBase
{
    private readonly DatabaseHealthProbe _probe;
    private readonly DatabaseOptions _options;

    public HealthController(DatabaseHealthProbe probe, DatabaseOptions options)
    {
        _probe = probe;
        _options = options;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _probe.CheckAsync(cancellationToken);

        var payload = new
        {
            status = result.IsHealthy ? "healthy" : "unhealthy",
            database = result.IsHealthy ? result.ServerVersion : result.Error,
            target = _options.BuildRedactedDescription(),
        };

        return result.IsHealthy
            ? Ok(payload)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, payload);
    }
}
