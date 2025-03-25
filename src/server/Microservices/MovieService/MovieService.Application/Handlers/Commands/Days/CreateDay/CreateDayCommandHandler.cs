using Domain.Constants;
using Domain.Exceptions;
using Extensions.Strings;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Days.CreateSession;

public class CreateDayCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<CreateDayCommand, Guid>
{
	public async Task<Guid> Handle(CreateDayCommand request, CancellationToken cancellationToken)
	{
		if (!request.StartTime.DateTimeFormatTryParse(out var parsedStartTime))
			throw new BadRequestException("Invalid date format.");

		if (!request.EndTime.DateTimeFormatTryParse(out var parsedEndTime))
			throw new BadRequestException("Invalid date format.");

		if (parsedStartTime.Date != parsedEndTime.Date)
			throw new UnprocessableContentException("StartTime and EndTime must be on the same day.");

		if (parsedStartTime >= parsedEndTime)
			throw new UnprocessableContentException("EndTime cannot be earlier than or equal to StartTime.");

		var date = parsedStartTime.Date;

		var existDay = await unitOfWork.DaysRepository.GetAsync(date, cancellationToken);

		if (existDay is not null)
			throw new AlreadyExistsException
				($"Day '{date.ToString(DateTimeConstants.DATE_FORMAT)}' already exist.");

		var day = new DayModel(
			Guid.NewGuid(),
			parsedStartTime,
			parsedEndTime);

		await unitOfWork.Repository<DayEntity>().CreateAsync(mapper.Map<DayEntity>(day), cancellationToken);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return day.Id;
	}
}