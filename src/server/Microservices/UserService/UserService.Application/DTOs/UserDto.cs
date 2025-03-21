﻿using System.Globalization;

namespace UserService.Application.DTOs;

public class UserDto
{
	public Guid Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string Role { get; set; } = string.Empty;
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public decimal Balance { get; set; }

	private DateTime _dateOfBirth;

	public string DateOfBirth
	{
		get => _dateOfBirth.ToString(Domain.Constants.DateTimeConstants.DATE_FORMAT);
		set
		{
			if (DateTime.TryParseExact(value,
				[Domain.Constants.DateTimeConstants.DATE_FORMAT, "dd.MM.yyyy HH:mm:ss", "MM/dd/yyyy", "MM/dd/yyyy HH:mm:ss"],
				CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
			{
				_dateOfBirth = date;
			}
			else
			{
				throw new FormatException($"Invalid date format: {value}");
			}
		}
	}
}