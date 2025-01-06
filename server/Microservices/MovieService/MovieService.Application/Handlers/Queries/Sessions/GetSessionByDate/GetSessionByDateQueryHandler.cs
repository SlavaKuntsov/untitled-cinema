using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Sessoins.GetSessionByDate;

public class GetSessionByDateQueryHandler(ISessionsRepository sessoinsRepository, IMapper mapper) : IRequestHandler<GetSessionByDateQuery, SessionModel?>
{
	private readonly ISessionsRepository _sessoinsRepository = sessoinsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<SessionModel?> Handle(GetSessionByDateQuery request, CancellationToken cancellationToken)
	{
		if (!request.Date.DateFormatTryParse(out DateTime parsedDate))
			throw new BadRequestException("Invalid date format.");

		var movie = await _sessoinsRepository.GetAsync(parsedDate, cancellationToken);

		return _mapper.Map<SessionModel>(movie);
	}
}