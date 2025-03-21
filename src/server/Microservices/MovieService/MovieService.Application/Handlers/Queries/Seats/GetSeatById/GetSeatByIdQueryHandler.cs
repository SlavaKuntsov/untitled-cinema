using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetSeatById;

public class GetSeatByIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetSeatByIdQuery, SeatModel>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<SeatModel> Handle(GetSeatByIdQuery request, CancellationToken cancellationToken)
	{
		var seat = await _unitOfWork.Repository<SeatEntity>().GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Seat with id '{request.Id}' not found.");

		return _mapper.Map<SeatModel>(seat);
	}
}