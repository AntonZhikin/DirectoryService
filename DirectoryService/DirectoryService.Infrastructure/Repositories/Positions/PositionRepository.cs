using DirectoryService.Application.Database.Repository;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories.Positions;

public class PositionRepository(ApplicationDbContext dbContext) : IPositionRepository
{
    public void Add(Position position) => dbContext.Positions.Add(position);

    public async Task<Position?> FindByIdAsync(PositionId id, CancellationToken cancellationToken)
    {
        return await dbContext.Positions.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public void Remove(Position position) => dbContext.Positions.Remove(position);

    public async Task<bool> HasDepartmentLinksAsync(PositionId id, CancellationToken cancellationToken)
    {
        return await dbContext.DepartmentPositions.AnyAsync(dp => dp.PositionId == id, cancellationToken);
    }
}
