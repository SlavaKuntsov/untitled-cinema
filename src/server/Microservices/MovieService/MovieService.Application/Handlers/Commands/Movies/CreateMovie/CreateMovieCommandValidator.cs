﻿using FluentValidation;

using MovieService.Application.Validators;
using MovieService.Domain.Constants;

namespace MovieService.Application.Handlers.Commands.Movies.CreateMovie;

public class CreateMovieCommandValidator : BaseCommandValidator<CreateMovieCommand>
{
	public CreateMovieCommandValidator()
	{
		RuleFor(x => x.Title)
			.NotEmpty().WithMessage("Title is required.")
			.MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

		RuleFor(x => x.Genres)
			.NotEmpty().WithMessage("At least one genre is required.")
			.Must(g => g.All(genre => !string.IsNullOrWhiteSpace(genre)))
			.WithMessage("All genres must be non-empty strings.");

		RuleFor(x => x.Description)
			.NotEmpty().WithMessage("Description is required.")
			.MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

		RuleFor(x => Convert.ToInt32(x.DurationMinutes))
			.GreaterThan(0).WithMessage("Duration must be greater than zero.");

		RuleFor(x => Convert.ToInt32(x.AgeLimit))
			.GreaterThan(0).WithMessage("Age Limit must be greater than zero.");

		RuleFor(x => x.Producer)
			.NotEmpty().WithMessage("Producer is required.")
			.MaximumLength(50).WithMessage("Producer must not exceed 50 characters.");

		RuleFor(x => x.ReleaseDate)
			.NotEmpty().WithMessage("Release date is required.")
			.Must(GeneralValidator.BeAValidDate)
			.WithMessage($"Release date must be a valid date with '{DateTimeConstants.DATE_TIME_FORMAT}' format.");

		RuleFor(x => x.Poster)
			.NotEmpty().WithMessage("Poster is required.")
			.Must(poster => poster.Length <= MoviesConstants.MAX_POSTER_SIZE * 1024 * 1024)
			.WithMessage("Poster must not exceed 5 MB.");
	}
}