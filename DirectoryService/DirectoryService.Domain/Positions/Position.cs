using CSharpFunctionalExtensions;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Domain.Positions;

public class Position
{
    //EF Core
    private Position() { }

    public Position(PositionName name, PositionDescription description)
    {
        Id = new PositionId(Guid.NewGuid());
        Name = name;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public PositionId Id { get; private set; }
    public PositionName Name { get; private set; }
    public PositionDescription Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public UnitResult<AppError> Update(string name, string description)
    {
        var nameResult = PositionName.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var descriptionResult = PositionDescription.Create(description);
        if (descriptionResult.IsFailure)
            return descriptionResult.Error;

        Name = nameResult.Value;
        Description = descriptionResult.Value;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<AppError>();
    }
}
