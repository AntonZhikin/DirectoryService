namespace DirectoryService.Shared.ErrorManagement;
public record AppError
{
    private const string SEPARATOR = "||";

    private AppError(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public string? InvalidField { get; }

    public static AppError Validation(string code, string message, string? invalidField = null) =>
        new(code, message, ErrorType.Validation, invalidField);
    public static AppError NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
    public static AppError Failure(string code, string message) => new(code, message, ErrorType.Failure);
    public static AppError Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    public static AppError UnExpected(string code, string message) => new(code, message, ErrorType.UnExpected);
    public static AppError External(string code, string message) => new(code, message, ErrorType.External);

    public string Serialize()
    {
        return string.Join(SEPARATOR, Code, Message, Type);
    }

    public static AppError Deserialize(string serialized)
    {
        string[] parts = serialized.Split(SEPARATOR);

        if (parts.Length < 3)
        {
            throw new ArgumentException("Invalid serialized format");
        }

        if (Enum.TryParse<ErrorType>(parts[2], out var type) == false)
        {
            throw new ArgumentException("Invalid serialized format");
        }

        return new AppError(parts[0], parts[1], type);
    }

    public AppErrorList ToErrorList() => new([this]);
}
public enum ErrorType
{
    Validation,
    NotFound,
    Failure,
    Conflict,
    External,
    UnExpected
}