using MapsterMapper;

using MediatR;

using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Application.Handlers.Commands.Sessions.DeleteSession;

public class DeleteSessionCommandHandler(
	ISessionsRepository sessionsRepository,
	IMapper mapper) : IRequestHandler<DeleteSessionCommand>
{
	private readonly ISessionsRepository _sessionsRepository = sessionsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
	{
		var movie = await _sessionsRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Session with id {request.Id} doesn't exists");

		_sessionsRepository.Delete(movie);

		return;
	}
}