using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bogus;

using FluentAssertions;

using MapsterMapper;

using Moq;

using Newtonsoft.Json.Linq;

using UserService.Application.DTOs;
using UserService.Application.Exceptions;
using UserService.Application.Handlers.Commands.Users.UserRegistration;
using UserService.Application.Handlers.Queries.Users.GetUserById;
using UserService.Application.Interfaces.Auth;
using UserService.Application.Interfaces.Caching;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

using Xunit;

namespace UserService.Tests.Unit.Auth;

public class AuthorizeCommandTests
{
	private readonly Mock<IUsersRepository> _usersRepositoryMock = new();
	private readonly Mock<IRedisCacheService> _redisCacheService = new();
	private readonly Mock<IPasswordHash> _passwordHashMock = new();
	private readonly Mock<IJwt> _jwtMock = new();
	private readonly Mock<IMapper> _mapperMock = new();
	private readonly UserRegistrationCommandHandler _handler;
	private readonly Faker _faker = new();

	[Fact]
	public async Task ShouldReturnEntity_WhenUserExist()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var userEntity = CreateUserEntity(userId);
		var userDto = CreateUserDto(userEntity);
		var userWithStringDateOfBirthEntity = CreateUserWithStringDateOfBirthEntity(userEntity);

		_usersRepositoryMock
			.Setup(repo => repo.GetWithStringDateOfBirthAsync(userId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(userWithStringDateOfBirthEntity);

		_mapperMock
			.Setup(m => m.Map<UserWithStringDateOfBirthDto>(It.IsAny<UserWithStringDateOfBirthEntity>()))
			.Returns(userDto);

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

		_usersRepositoryMock
			.Setup(repo => repo.GetWithStringDateOfBirthAsync(userId, It.IsAny<CancellationToken>()))
			.ReturnsAsync((UserWithStringDateOfBirthEntity?)null);

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
		var cachedUserDto = CreateUserDto(userEntity);

		_redisCacheService
			.Setup(cache => cache.GetValueAsync<UserWithStringDateOfBirthDto>($"users_{userId}"))
			.ReturnsAsync(cachedUserDto);

		var handler = CreateHandler();
		var command = new GetUserByIdQuery(userId);

		// Act
		var userAct = await handler.Handle(command, CancellationToken.None);

		// Assert
		userAct.Should().NotBeNull();
		userAct.Id.Should().Be(userId);
		userAct.Email.Should().Be(cachedUserDto.Email);

		// Проверяем, что запрос в БД не выполнялся
		_usersRepositoryMock.Verify(repo => repo.GetWithStringDateOfBirthAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	public AuthorizeCommandTests()
	{
		_handler = new UserRegistrationCommandHandler(
			_usersRepositoryMock.Object,
			_passwordHashMock.Object,
			_jwtMock.Object,
			_mapperMock.Object
		);
	}

	private UserModel CreateUserEntity(Guid userId)
	{
		return new UserModel(
			userId,
			_faker.Internet.Email(),
			_faker.Internet.Password(),
			Domain.Enums.Role.User,
			_faker.Name.FirstName(),
			_faker.Name.LastName(),
			_faker.Date.Past(20));
	}

	private UserWithStringDateOfBirthDto CreateUserDto(UserModel userEntity)
	{
		return new UserWithStringDateOfBirthDto(
			userEntity.Id,
			userEntity.Email,
			userEntity.Role.ToString(),
			userEntity.FirstName,
			userEntity.LastName,
			userEntity.DateOfBirth.ToString("dd-MM-yyyy"),
			userEntity.Balance
		);
	}

	private UserWithStringDateOfBirthEntity CreateUserWithStringDateOfBirthEntity(UserModel userEntity)
	{
		return new UserWithStringDateOfBirthEntity
		{
			Id = userEntity.Id,
			Email = userEntity.Email,
			Password = userEntity.Password,
			Role = userEntity.Role,
			FirstName = userEntity.FirstName,
			LastName = userEntity.LastName,
			DateOfBirth = userEntity.DateOfBirth.ToString("dd-MM-yyyy"),
			Balance = userEntity.Balance
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