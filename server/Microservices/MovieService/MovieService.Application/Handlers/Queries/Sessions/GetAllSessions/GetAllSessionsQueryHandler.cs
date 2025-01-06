using MapsterMapper;

using MediatR;

using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Sessoins.GetAllSessions;

public class GetAllSessionsQueryHandler(ISessionsRepository sessoinsRepository, IMapper mapper) : IRequestHandler<GetAllSessionsQuery, IList<SessionModel>>
{
	private readonly ISessionsRepository _sessoinsRepository = sessoinsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<SessionModel>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
	{
		var movies = await _sessoinsRepository.GetAsync(cancellationToken);

		return _mapper.Map<IList<SessionModel>>(movies);
	}
}