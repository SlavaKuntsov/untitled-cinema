using Bogus;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;
using MapsterMapper;
using Moq;
using Redis.Services;
using UserService.Application.Data;
using UserService.Application.Handlers.Commands.Users.UserRegistration;
using UserService.Application.Handlers.Queries.Users.GetUserById;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;
using Xunit;

namespace UserService.Tests.Unit.Auth;

public class AuthorizeCommandTests
{
	private readonly Mock<IDBContext> _dbcontextMock = new();

	private readonly Faker _faker = new();

	private readonly UserRegistrationCommandHandler _handler;

	private readonly Mock<IJwt> _jwtMock = new();

	private readonly Mock<IMapper> _mapperMock = new();

	private readonly Mock<IPasswordHash> _passwordHashMock = new();

	private readonly Mock<IRedisCacheService> _redisCacheService = new();

	private readonly Mock<IUsersRepository> _usersRepositoryMock = new();

	public AuthorizeCommandTests()
	{
		_handler = new UserRegistrationCommandHandler(
			_usersRepositoryMock.Object,
			_jwtMock.Object,
			_passwordHashMock.Object,
			_dbcontextMock.Object,
			_mapperMock.Object
		);
	}

	[Fact]
	public async Task ShouldReturnEntity_WhenUserExist()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var userEntity = CreateUserEntity(userId);
		var userModel = CreateUserModel(userId);

		_usersRepositoryMock
			.Setup(repo => repo.GetAsync(userId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(userEntity);

		_mapperMock
			.Setup(m => m.Map<UserModel>(It.IsAny<UserEntity>()))
			.Returns(userModel);

		var handler = CreateHandler();
		var command = new GetUserByIdQuery(userId);

		// Act
		var userAct = await handler.Handle(command, CancellationToken.None);

		// Assert
		userAct.Should().NotBeNull("User should be not null");
		userAct.Id.Should().Be(userId);
		userAct.Email.Should().Be(userEntity.Email);
	}

	[Fact]
	public async Task ShouldThrowNotFoundException_WhenUserDoesNotExist()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var userEntity = CreateUserEntity(userId);

		_usersRepositoryMock
			.Setup(repo => repo.GetAsync(userId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(userEntity);

		var handler = CreateHandler();
		var command = new GetUserByIdQuery(userId);

		// Act & Assert
		await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
	}

	[Fact]
	public async Task ShouldReturnUserFromCache_WhenUserExistsInCache()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var userEntity = CreateUserEntity(userId);

		_redisCacheService
			.Setup(cache => cache.GetValueAsync<UserEntity>($"users_{userId}"))
			.ReturnsAsync(userEntity);

		var handler = CreateHandler();
		var command = new GetUserByIdQuery(userId);

		// Act
		var userAct = await handler.Handle(command, CancellationToken.None);

		// Assert
		userAct.Should().NotBeNull();
		userAct.Id.Should().Be(userId);

		_usersRepositoryMock.Verify(
			repo => repo.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
			Times
				.Never);
	}

	private UserModel CreateUserModel(Guid userId)
	{
		return new UserModel(
			userId,
			_faker.Internet.Email(),
			_faker.Internet.Password(),
			Role.User,
			_faker.Name.FirstName(),
			_faker.Name.LastName(),
			_faker.Date.Past(20).ToString());
	}

	private UserEntity CreateUserEntity(Guid userId)
	{
		return new UserEntity
		{
			Id = userId,
			Email = _faker.Internet.Email(),
			Password = _faker.Internet.Password(),
			FirstName = _faker.Name.FirstName(),
			LastName = _faker.Name.LastName(),
			DateOfBirth = _faker.Date.Past(20).ToString()
		};
	}

	private GetUserByIdQueryHandler CreateHandler()
	{
		return new GetUserByIdQueryHandler(
			_usersRepositoryMock.Object,
			_redisCacheService.Object,
			_mapperMock.Object
		);
	}
}