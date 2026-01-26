using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Halls.GetHallById;

public class GetHallByIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetHallByIdQuery, HallModel?>
{
	public async Task<HallModel?> Handle(GetHallByIdQuery request, CancellationToken cancellationToken)
	{
		var hall = await unitOfWork.Repository<HallEntity>().GetAsync(request.Id, cancellationToken)
					?? throw new NotFoundException($"Hall with id '{request.Id.ToString()}' not found.");

		return mapper.Map<HallModel>(hall);
	}
}