using System.Globalization;

using Mapster;

using MediatR;

using UserService.Domain;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Users;

public class UpdateUserCommand(
	Guid id,
	string firstName,
	string lastName,
	string dateOfBirth) : IRequest<UserModel>
{
	public Guid Id { get; private set; } = id;
	public string FirstName { get; private set; } = firstName;
	public string LastName { get; private set; } = lastName;
	public string DateOfBirth { get; private set; } = dateOfBirth;

	public class UpdateParticipantCommandHandler(IUsersRepository usersRepository) : IRequestHandler<UpdateUserCommand, UserModel>
	{
		private readonly IUsersRepository _usersRepository = usersRepository;

		public async Task<UserModel> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
		{
			if (!DateTime.TryParseExact(
				request.DateOfBirth,
				Domain.Constants.DateTimeConstants.DATE_TIME_FORMAT,
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out DateTime parsedDateTime))
				throw new BadRequestException("Invalid date format.");

			var existUser = await _usersRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"User with id {request.Id} doesn't exists");

			request.Adapt(existUser);

			return await _usersRepository.UpdateAsync(existUser, cancellationToken);
		}
	}
}