using DirectoryService.Domain.Positions.ValueObjects;

namespace DirectoryService.Domain.Positions;

public class Position
{
    //EF Core
    private Position() { }
    
    public Position(PositionId id, PositionName name, PositionDescription description, bool isActive, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public PositionId Id { get; private set; }
    public PositionName Name { get; private set; }
    public PositionDescription Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
}