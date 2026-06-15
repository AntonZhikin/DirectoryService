namespace DirectoryService.Shared.ErrorManagement;

public static class AppErrors
{
    public static AppError ValueIsInvalid(string? name = null)
    {
        string label = name ?? "значение";
        return AppError.Validation("value.is.invalid", $"'{label}' недействительно", name);
    }
    
    public static AppError NotFound(Guid? id = null, string? name = null)
    {
        string subject = name ?? "запись";
        string forId = id is null ? string.Empty : $" по Id '{id}'";
        return AppError.NotFound("record.not.found", $"{subject} не найдена{forId}");
    }

    public static AppError AlreadyExists(string? name = null)
    {
        string subject = name ?? "запись";
        return AppError.Conflict("record.already.exists", $"{subject} уже существует");
    }

    public static AppError Failure(string? message = null)
    { 
        return AppError.Failure("server.failure", message ?? "Серверная ошибка");
    }
}