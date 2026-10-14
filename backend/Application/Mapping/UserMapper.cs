using Paper.Application.DTOs;
using Paper.Domain.Entities;

namespace Paper.Application.Mapping;

public static class UserMapper
{
    public static UserDto ToDto(this User user) => new(user.Id, user.Name, user.Email, user.CreatedAt);
}
