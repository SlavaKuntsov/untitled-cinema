using MediatR;

namespace MovieService.Application.Handlers.Commands.Days.DeleteDay;

public class DeleteDayCommand(Guid id) : IRequest
{
	public Guid Id { get; private set; } = id;
}