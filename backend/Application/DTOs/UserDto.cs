namespace Paper.Application.DTOs;

public sealed record UserDto(int Id, string Name, string Email, DateTimeOffset CreatedAt);
