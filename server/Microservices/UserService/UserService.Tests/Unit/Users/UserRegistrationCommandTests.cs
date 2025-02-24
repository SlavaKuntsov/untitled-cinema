using Bogus;

using FluentAssertions;

using MapsterMapper;

using Moq;

using UserService.Application.DTOs;
using UserService.Application.Exceptions;
using UserService.Application.Handlers.Commands.Users.UserRegistration;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

using Xunit;

namespace UserService.Tests.Unit.Users;

public class UserRegistrationCommandTests
{
	private readonly Mock<IUsersRepository> _usersRepositoryMock = new();
	private readonly Mock<IPasswordHash> _passwordHashMock = new();
	private readonly Mock<IJwt> _jwtMock = new();
	private readonly Mock<IMapper> _mapperMock = new();
	private readonly UserRegistrationCommandHandler _handler;
	private readonly Faker _faker = new();

	public UserRegistrationCommandTests()
	{
		_handler = new UserRegistrationCommandHandler(
			_usersRepositoryMock.Object,
			_passwordHashMock.Object,
			_jwtMock.Object,
			_mapperMock.Object
		);
	}

	[Fact]
	public async Task Handle_ValidRequest_ReturnsAuthDto()
	{
		// Arrange
		var command = GetValidCommand();
		var userId = Guid.NewGuid();
		var expectedAuthDto = GetAuthDto();
		var userModel = GetUserModel(command, userId);
		var userEntity = new UserEntity { Id = userId };
		var refreshTokenEntity = new RefreshTokenEntity { UserId = userId };

		SetupMocksForSuccessScenario(command, userModel, userEntity, refreshTokenEntity, expectedAuthDto);

		// Act
		_jwtMock.Setup(x => x.GenerateAccessToken(userModel.Id, It.IsAny<Role>()))
			.Returns(expectedAuthDto.AccessToken);

		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.Should().BeEquivalentTo(expectedAuthDto);
		VerifySuccessScenarioCalls(command, userModel, userEntity, refreshTokenEntity);
	}

	[Fact]
	public async Task Handle_InvalidDateFormat_ThrowsBadRequestException()
	{
		// Arrange
		var command = GetCommandWithInvalidDate();

		// Act & Assert
		await Assert.ThrowsAsync<BadRequestException>(() =>
			_handler.Handle(command, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_ExistingUser_ThrowsAlreadyExistsException()
	{
		// Arrange
		var command = GetValidCommand();
		SetupExistingUserScenario(command);

		// Act & Assert
		await Assert.ThrowsAsync<AlreadyExistsException>(() =>
			_handler.Handle(command, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_PasswordHashing_ShouldBeCalled()
	{
		// Arrange
		var command = GetValidCommand();
		var userId = Guid.NewGuid();
		var userModel = GetUserModel(command, userId);
		var userEntity = new UserEntity { Id = userId };
		var refreshTokenEntity = new RefreshTokenEntity { UserId = userId };
		var authDto = GetAuthDto();

		SetupMocksForSuccessScenario(command, userModel, userEntity, refreshTokenEntity, authDto);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_passwordHashMock.Verify(
			x => x.Generate(command.Password),
			Times.Once);
	}

	private void SetupMocksForSuccessScenario(
		UserRegistrationCommand command,
		UserModel userModel = null,
		UserEntity userEntity = null,
		RefreshTokenEntity refreshTokenEntity = null,
		AuthDto authDto = null)
	{
		userModel ??= GetUserModel(command, userModel?.Id ?? Guid.NewGuid());
		userEntity ??= new UserEntity { Id = userModel.Id };
		refreshTokenEntity ??= new RefreshTokenEntity { UserId = userModel.Id };
		authDto ??= GetAuthDto();

		_usersRepositoryMock
			.Setup(x => x.GetIdAsync(command.Email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(Guid.Empty);

		_passwordHashMock
			.Setup(x => x.Generate(It.IsAny<string>()))
			.Returns("hashed_password");

		_jwtMock.Setup(x => x.GenerateAccessToken(userModel.Id, It.IsAny<Role>()))
			.Returns(authDto.AccessToken);

		_jwtMock.Setup(x => x.GenerateRefreshToken())
			.Returns(authDto.RefreshToken);

		_mapperMock.Setup(x => x.Map<UserEntity>(It.IsAny<UserModel>()))
			.Returns(userEntity);

		_mapperMock.Setup(x => x.Map<RefreshTokenEntity>(It.IsAny<RefreshTokenModel>()))
			.Returns(refreshTokenEntity);
	}

	private void SetupExistingUserScenario(UserRegistrationCommand command)
	{
		_usersRepositoryMock
			.Setup(x => x.GetIdAsync(command.Email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(Guid.NewGuid());
	}

	private void VerifySuccessScenarioCalls(
		UserRegistrationCommand command,
		UserModel userModel,
		UserEntity userEntity,
		RefreshTokenEntity refreshTokenEntity)
	{
		_usersRepositoryMock.Verify(
			x => x.CreateAsync(
				userEntity,
				refreshTokenEntity,
				It.IsAny<CancellationToken>()),
			Times.Once);

		_jwtMock.Verify(
			x => x.GenerateAccessToken(userModel.Id, Role.User),
			Times.Once);

		_jwtMock.Verify(
			x => x.GenerateRefreshToken(),
			Times.Once);
	}

	private UserRegistrationCommand GetValidCommand()
	{
		return new UserRegistrationCommand(
			_faker.Internet.Email(),
			_faker.Internet.Password(),
			_faker.Name.FirstName(),
			_faker.Name.LastName(),
			_faker.Date.Past(20).ToString("dd-MM-yyyy"));
	}

	private UserRegistrationCommand GetCommandWithInvalidDate()
	{
		return new UserRegistrationCommand(
			_faker.Internet.Email(),
			_faker.Internet.Password(),
			_faker.Name.FirstName(),
			_faker.Name.LastName(),
			_faker.Date.Past(20).ToString("dd-MM-yyyy"));
	}

	private AuthDto GetAuthDto()
	{
		return new AuthDto(
			_faker.Random.String2(50),
			_faker.Random.String2(50));
	}

	private UserModel GetUserModel(UserRegistrationCommand command, Guid userId)
	{
		return new UserModel(
			userId,
			command.Email,
			"hashed_password",
			Role.User,
			command.FirstName,
			command.LastName,
			DateTime.Parse(command.DateOfBirth));
	}
}