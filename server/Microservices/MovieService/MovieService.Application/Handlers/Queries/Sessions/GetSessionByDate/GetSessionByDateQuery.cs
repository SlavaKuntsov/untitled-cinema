using MediatR;

using MovieService.Domain;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Sessoins.GetSessionByDate;

public partial class GetSessionByDateQuery(string date) : IRequest<SessionModel>
{
	public string Date { get; private set; } = date;
}