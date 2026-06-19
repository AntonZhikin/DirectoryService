using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Response.Location;
using DirectoryService.Shared.ErrorManagement;
using MediatR;

namespace DirectoryService.Application.Locations.Queries.GetTop;

public record GetTopLocationsQuery : IRequest<Result<List<LocationTopDto>, AppError>>;
