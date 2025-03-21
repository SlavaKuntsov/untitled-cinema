﻿using UserService.Application.Interfaces.Auth;

namespace UserService.API.Contracts;

public record UpdateUserRequest(
	string Firstname,
	string Lastname,
	string DateOfBirth);
