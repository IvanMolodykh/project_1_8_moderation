using Microsoft.AspNetCore.Mvc;
using Paper.Application.Abstractions;
using Paper.Application.DTOs;
using Paper.Domain.Abstractions;

namespace PaperAPI.Controllers;

[ApiController]
[Route("api/users")]
[Produces("application/json")]
[Tags("Users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _users;
    private readonly IAppLogger _logger;

    public UsersController(IUserService users, IAppLogger logger)
    {
        _users = users;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<UserDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET /api/users");
        return Ok(await _users.GetAllAsync(cancellationToken));
    }

    [HttpGet("count")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetCount(CancellationToken cancellationToken)
        => Ok(await _users.CountAsync(cancellationToken));

    [HttpGet("{id:int}", Name = nameof(GetById))]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken);

        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Create(
        [FromBody] CreateUserDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await _users.CreateAsync(dto, cancellationToken);
            return CreatedAtRoute(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(exception.ParamName ?? "request", exception.Message);

            return ValidationProblem(ModelState);
        }
    }
}
