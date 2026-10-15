namespace Domain.Exceptions;

/// <summary>
/// Данные уже заняты (например, email уже зарегистрирован) → 409 Conflict.
/// ArgumentException остаётся для ошибок валидации → 400 Bad Request.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}