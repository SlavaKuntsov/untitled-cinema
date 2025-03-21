using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Sessions.GetSessionById;

public class GetSessionByIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetSessionByIdQuery, SessionModel>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<SessionModel> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
	{
		var session = await _unitOfWork.Repository<SessionEntity>()
			.GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Session with id '{request.Id}' not found.");

		return _mapper.Map<SessionModel>(session);
	}
}