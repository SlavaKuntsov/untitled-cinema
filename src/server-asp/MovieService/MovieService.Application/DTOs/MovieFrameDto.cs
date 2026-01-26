namespace MovieService.Application.DTOs;

public record struct MovieFrameDto(
	Guid Id,
	Guid MovieId,
	string FrameName,
	string FrameUrl);