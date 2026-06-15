namespace DirectoryService.Shared.ErrorManagement;

public record AppErrorMessage(string Code, string Message, string? InvalidField = null);

public record AppError
{
    public IReadOnlyList<AppErrorMessage> Messages { get; } = [];

    public ErrorType Type { get; }

    private const string SEPARATOR = "||";

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

    public static AppError Validation(params AppErrorMessage[] messages)
        => new(messages, ErrorType.VALIDATION);

    public static AppError NotFound(params AppErrorMessage[] messages)
        => new(messages, ErrorType.NOT_FOUND);

    public static AppError Failure(params AppErrorMessage[] messages)
        => new(messages, ErrorType.FAILURE);
    public string Serialize()
    {
        var first = Messages[0];
        return string.Join(SEPARATOR, first.Code, first.Message);
    }

    public static AppErrorMessage DeserializeToMessage(string serialized, string? invalidField = null)
    {
        string[] parts = serialized.Split(SEPARATOR);

        return parts.Length < 2 
            ? throw new ArgumentException("Invalid serialized format") 
            : new AppErrorMessage(parts[0], parts[1], invalidField);
    }
}

public enum ErrorType
{
    VALIDATION,
    NOT_FOUND,
    FAILURE,
    CONFLICT,
    EXTERNAL,
    UN_EXPECTED
}
