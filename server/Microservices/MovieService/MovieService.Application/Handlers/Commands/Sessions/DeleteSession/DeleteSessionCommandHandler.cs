using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Sessions.DeleteSession;

public class DeleteSessionCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<DeleteSessionCommand>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
	{
		var movie = await _unitOfWork.Repository<SessionEntity>().GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Session with id {request.Id} doesn't exists");

		_unitOfWork.Repository<SessionEntity>().Delete(movie);
		_unitOfWork.SeatsRepository.DeleteBySessionId(request.Id);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return;
	}
}