using MediatR;

using UserService.Domain;

namespace UserService.Application.Handlers.Commands.Users.ChangeBalance;

public partial class ChangeBalanceCommand(
	Guid id,
	decimal amount,
	bool isIncrease) : IRequest<UserModel>
{
	public Guid Id { get; private set; } = id;
	public decimal Amount { get; private set; } = amount;
	public bool IsIncrease { get; private set; } = isIncrease;
}