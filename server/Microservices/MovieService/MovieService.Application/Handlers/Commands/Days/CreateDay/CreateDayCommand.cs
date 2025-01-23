using MediatR;

namespace MovieService.Application.Handlers.Commands.Days.CreateSession;

public class CreateDayCommand(
    string startTime,
    string endTime) : IRequest<Guid>
{
    public string StartTime { get; private set; } = startTime;
    public string EndTime { get; private set; } = endTime;
}