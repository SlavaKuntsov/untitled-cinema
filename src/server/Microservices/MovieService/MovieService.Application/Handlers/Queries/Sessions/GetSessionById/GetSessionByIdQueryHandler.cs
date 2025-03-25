using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Sessions.GetSessionById;

public class GetSessionByIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetSessionByIdQuery, SessionModel>
{
	public async Task<SessionModel> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
	{
		var session = await unitOfWork.Repository<SessionEntity>()
						.GetAsync(request.Id, cancellationToken)
					?? throw new NotFoundException($"Session with id '{request.Id}' not found.");

		return mapper.Map<SessionModel>(session);
	}
}