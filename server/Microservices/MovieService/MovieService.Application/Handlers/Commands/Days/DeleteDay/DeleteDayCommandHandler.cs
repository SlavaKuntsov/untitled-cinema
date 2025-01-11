using MapsterMapper;

using MediatR;

using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Application.Handlers.Commands.Days.DeleteDay;

public class DeleteDayCommandHandler(
	IDaysRepository daysRepository,
	IMapper mapper) : IRequestHandler<DeleteDayCommand>
{
	private readonly IDaysRepository _daysRepository = daysRepository;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(DeleteDayCommand request, CancellationToken cancellationToken)
	{
		var hall = await daysRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Day with id {request.Id} doesn't exists");

		daysRepository.Delete(hall);

		return;
	}
}