namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record Path
{
    private const char SEPARATOR = '.';

    public string Value { get; }

    private Path(string value)
    {
        Value = value;
    }

    public static Path CreateParent(Identifier identifier)
    {
        return new Path(identifier.Value);
    }

    public static Path Create(string value)
    {
        return new Path(value);
    }

    public Path CreateChild(Identifier childIdentifier)
    {
        return new Path(Value + SEPARATOR + childIdentifier.Value);
    }
}