using System.Text.Json.Serialization;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Shared.Response;
public record Envelope
{
    [JsonPropertyName("result")]
    public object? Result { get; }

    [JsonPropertyName("errors")]
    public AppErrorList? Errors { get; }

    [JsonPropertyName("isError")]
    public bool IsError => Errors != null || Errors != null && Errors.Any();

    [JsonPropertyName("timeGenerated")]
    public DateTime TimeGenerated { get; }

    [JsonConstructor]
    private Envelope(object? result, AppErrorList? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.Now;
    }

    public static Envelope Ok(object? result = null) =>
        new(result, null);

    public static Envelope Error(AppErrorList appErrors) =>
        new(null, appErrors);
}

public record Envelope<T>
{
    [JsonPropertyName("result")]
    public T? Result { get; }

    [JsonPropertyName("errors")]
    public AppErrorList? Errors { get; }

    [JsonPropertyName("isError")]
    public bool IsError => Errors != null || Errors != null && Errors.Any();

    [JsonPropertyName("timeGenerated")]
    public DateTime TimeGenerated { get; }

    [JsonConstructor]
    private Envelope(T? result, AppErrorList? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.Now;
    }

    public static Envelope<T> Ok(T? result = default) =>
        new(result, null);

    public static Envelope<T> Error(AppErrorList appErrors) =>
        new(default, appErrors);
}
