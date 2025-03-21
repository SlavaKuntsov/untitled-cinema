using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Domain.Constants;
using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Days.CreateSession;

public class CreateDayCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<CreateDayCommand, Guid>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(CreateDayCommand request, CancellationToken cancellationToken)
	{
		if (!request.StartTime.DateTimeFormatTryParse(out DateTime parsedStartTime))
			throw new BadRequestException("Invalid date format.");

		if (!request.EndTime.DateTimeFormatTryParse(out DateTime parsedEndTime))
			throw new BadRequestException("Invalid date format.");

		if (parsedStartTime.Date != parsedEndTime.Date)
			throw new UnprocessableContentException("StartTime and EndTime must be on the same day.");

		if (parsedStartTime >= parsedEndTime)
			throw new UnprocessableContentException("EndTime cannot be earlier than or equal to StartTime.");

		var date = parsedStartTime.Date;

		var existDay = await _unitOfWork.DaysRepository.GetAsync(date, cancellationToken);

		if (existDay is not null)
			throw new AlreadyExistsException
				($"Day '{date.ToString(DateTimeConstants.DATE_FORMAT)}' already exist.");

		var day = new DayModel(
			Guid.NewGuid(),
			parsedStartTime,
			parsedEndTime);

		await _unitOfWork.Repository<DayEntity>().CreateAsync(_mapper.Map<DayEntity>(day), cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return day.Id;
	}
}