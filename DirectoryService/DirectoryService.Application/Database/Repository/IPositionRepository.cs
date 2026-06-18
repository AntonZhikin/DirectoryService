using DirectoryService.Domain.Positions;

namespace DirectoryService.Application.Database.Repository;

public interface IPositionRepository
{
    void Add(Position position);
    Task<Position?> FindByIdAsync(PositionId id, CancellationToken cancellationToken);
    void Remove(Position position);
    Task<bool> HasDepartmentLinksAsync(PositionId id, CancellationToken cancellationToken);
}
