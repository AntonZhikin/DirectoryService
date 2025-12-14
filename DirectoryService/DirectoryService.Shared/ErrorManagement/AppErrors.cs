namespace DirectoryService.Shared.ErrorManagement;

public static class AppErrors
{
    public static AppError ValueIsInvalid(string name) =>
        AppError.Validation("value.is.invalid", $"'{name}' is invalid");

    public static AppError NotFound(Guid id) =>
        AppError.NotFound("record.not.found", $"Record not found for id '{id}'");

    public static AppError NotFound(string name) =>
        AppError.NotFound("record.not.found", $"Record not found with '{name}'");

    public static AppError AlreadyExists(string name) =>
        AppError.Conflict("record.already.exists", $"Record already exists with '{name}'");

    public static AppError Failure(string message) =>
        AppError.Failure("record.failure", message);
    
    public static AppError Failure(string code, string message) => AppError.Failure(code, message);
    
    public static AppError UnExpected(string message) =>
        AppError.UnExpected("unexpected.error", message);

    public static AppError External(string message) =>
        AppError.External("external.error", message);
}