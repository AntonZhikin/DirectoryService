using System.Text.Json.Serialization;

namespace DirectoryService.Shared.ErrorManagement;

public record AppErrorMessage(string Code, string Message, string? InvalidField = null);

public record AppError
{
    public IReadOnlyList<AppErrorMessage> Messages { get; } = [];

    public ErrorType Type { get; }

    [JsonConstructor]
    private AppError(IReadOnlyList<AppErrorMessage> messages, ErrorType type)
    {
        Messages = messages.ToArray();
        Type = type;
    }

    private AppError(IEnumerable<AppErrorMessage> messages, ErrorType type)
    {
        Messages = messages.ToArray();
        Type = type;
    }

    public static AppError Validation(string code, string message, string? invalidField = null)
        => new([new AppErrorMessage(code, message, invalidField)], ErrorType.VALIDATION);

    public static AppError NotFound(string code, string message, string? invalidField = null)
        => new([new AppErrorMessage(code, message, invalidField)], ErrorType.NOT_FOUND);

    public static AppError Failure(string code, string message, string? invalidField = null)
        => new([new AppErrorMessage(code, message, invalidField)], ErrorType.FAILURE);

    public static AppError Conflict(string code, string message, string? invalidField = null)
        => new([new AppErrorMessage(code, message, invalidField)], ErrorType.CONFLICT);

    public static AppError External(string code, string message, string? invalidField = null)
        => new([new AppErrorMessage(code, message, invalidField)], ErrorType.EXTERNAL);

    public static AppError UnExpected(string code, string message, string? invalidField = null)
        => new([new AppErrorMessage(code, message, invalidField)], ErrorType.UN_EXPECTED);

    public static AppError Validation(params IEnumerable<AppErrorMessage> messages)
        => new(messages, ErrorType.VALIDATION);

    public static AppError NotFound(params IEnumerable<AppErrorMessage> messages)
        => new(messages, ErrorType.NOT_FOUND);

    public static AppError Failure(params IEnumerable<AppErrorMessage> messages)
        => new(messages, ErrorType.FAILURE);
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorType
{
    VALIDATION,
    NOT_FOUND,
    FAILURE,
    CONFLICT,
    EXTERNAL,
    UN_EXPECTED
}
