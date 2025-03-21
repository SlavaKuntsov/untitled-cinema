using System.Globalization;

using FluentValidation;

using MovieService.Application.Validators;

namespace MovieService.Application.Handlers.Commands.Movies.UpdateMovie;

public class UpdateMovieCommandValidator : BaseCommandValidator<UpdateMovieCommand>
{
	public UpdateMovieCommandValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage("SessionId is required.");

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

		RuleFor(x => x.Producer)
			.NotEmpty().WithMessage("Producer is required.")
			.MaximumLength(50).WithMessage("Producer must not exceed 50 characters.");

		RuleFor(x => x.ReleaseDate)
			.NotEmpty().WithMessage("Release date is required.")
			.Must(GeneralValidator.BeAValidDate)
			.WithMessage($"Release date must be a valid date with '{Domain.Constants.DateTimeConstants.DATE_TIME_FORMAT}' format.");
	}
}