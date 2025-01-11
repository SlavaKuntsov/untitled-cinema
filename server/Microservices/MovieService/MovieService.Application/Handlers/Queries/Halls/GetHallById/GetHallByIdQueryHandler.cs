using MapsterMapper;

using MediatR;

using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Halls.GetHallById;

public class GetHallByIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetHallByIdQuery, HallModel?>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<HallModel?> Handle(GetHallByIdQuery request, CancellationToken cancellationToken)
	{
		var hall = await _unitOfWork.HallsRepository.GetAsync(request.Id, cancellationToken);

		return _mapper.Map<HallModel>(hall);
	}
}