using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Domain.Constants;
using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Days.CreateSession;

public class CreateDayCommandHandler(
	IDaysRepository daysRepository,
	IMapper mapper) : IRequestHandler<CreateDayCommand, Guid>
{
	private readonly IDaysRepository _daysRepository = daysRepository;
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

		var existDay = await _daysRepository.GetAsync(date, cancellationToken);

		if (existDay is not null)
			throw new AlreadyExistsException($"Day '{date.ToString(DateTimeConstants.DATE_FORMAT)}' already exist.");

		parsedStartTime = DateTime.SpecifyKind(parsedStartTime, DateTimeKind.Local).ToUniversalTime();
		parsedEndTime = DateTime.SpecifyKind(parsedEndTime, DateTimeKind.Local).ToUniversalTime();

		var day = new DayModel(
			Guid.NewGuid(),
			parsedStartTime,
			parsedEndTime);

		await _daysRepository.CreateAsync(_mapper.Map<DayEntity>(day), cancellationToken);

		return day.Id;
	}
}