using System.Text.Json.Serialization;

namespace DirectoryService.Shared.ErrorManagement;
public record AppError
{
    private const string SEPARATOR = "||";

    private AppError(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public string Code { get; }
    public string Message { get; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType Type { get; }

    public static AppError Validation(string code, string message) => new(code, message, ErrorType.Validation);
    public static AppError NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
    public static AppError Failure(string code, string message) => new(code, message, ErrorType.Failure);
    public static AppError Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    public static AppError UnExpected(string code, string message) => new(code, message, ErrorType.UnExpected);
    public static AppError External(string code, string message) => new(code, message, ErrorType.External);

    public static string Serialize(AppError appError) => string.Join(SEPARATOR, appError.Code, appError.Message, appError.Type);
    public static AppError Deserialize(string serialized)
    {
        var parts = serialized.Split(SEPARATOR);

        if (parts.Length != 3)
            throw new ArgumentException("Serialized error is invalid", nameof(serialized));

        if (!Enum.TryParse<ErrorType>(parts[2], out var enumValue))
            throw new ArgumentException("Serialized error type is invalid", nameof(serialized));

        return new AppError(parts[0], parts[1], enumValue);
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