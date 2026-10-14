using Paper.Application.Abstractions;
using Paper.Application.DTOs;
using Paper.Application.Mapping;
using Paper.Domain.Abstractions;

namespace Paper.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IAppLogger _logger;

    public UserService(IUserRepository users, IAppLogger logger)
    {
        _users = users;
        _logger = logger;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _users.GetAllAsync(cancellationToken);
        return users.Select(u => u.ToDto()).ToArray();
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken);
        return user?.ToDto();
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        EnsureValid(dto);

        var created = await _users.AddAsync(dto.Name.Trim(), dto.Email.Trim(), cancellationToken);

        _logger.LogInformation("Создан пользователь {UserId} ({Email})", created.Id, created.Email);
        return created.ToDto();
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _users.CountAsync(cancellationToken);

    private static void EnsureValid(CreateUserDto dto)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Имя обязательно.", nameof(CreateUserDto.Name));

        if (dto.Name.Length > 100)
            throw new ArgumentException("Имя не длиннее 100 символов.", nameof(CreateUserDto.Name));

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("Email обязателен.", nameof(CreateUserDto.Email));

        if (!dto.Email.Contains('@', StringComparison.Ordinal))
            throw new ArgumentException("Некорректный email.", nameof(CreateUserDto.Email));
    }
}
