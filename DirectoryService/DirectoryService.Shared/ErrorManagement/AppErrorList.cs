using System.Collections;

namespace DirectoryService.Shared.ErrorManagement;
public class AppErrorList(IEnumerable<AppError> errors) : IEnumerable<AppError>
{
    private readonly List<AppError> _errors = [.. errors];

    public IEnumerator<AppError> GetEnumerator()
    {
        return _errors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static implicit operator AppErrorList(List<AppError> errors)
        => new(errors);

    public static implicit operator AppErrorList(AppError appError)
        => new([appError]);
}