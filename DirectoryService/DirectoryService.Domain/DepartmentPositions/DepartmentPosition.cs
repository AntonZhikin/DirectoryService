using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;

namespace DirectoryService.Domain.DepartmentPositions;

public record DepartmentPosition
{
    public DepartmentPositionId Id { get; init; }
    
    public required DepartmentId DepartmentId { get; init; }
    
    public required PositionId PositionId { get; init; }
}