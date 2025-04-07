using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;
using Brokers.Models.DTOs;
using Domain.DTOs;
using Domain.Exceptions;
using Extensions.Enums;
using Extensions.Strings;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Handlers.Query.Bookings.GetUserBookings;

public record GetUserBookingsByIdQuery(
	Guid UserId,
	byte Limit,
	byte Offset,
	string[] Filters,
	string[] FilterValues,
	string SortBy,
	string SortDirection,
	string Date) : IRequest<PaginationWrapperDto<BookingModel>>;

public class GetUserBookingsByIdQueryHandler(
	IBookingsRepository bookingsRepository,
	IMapper mapper,
	ILogger<GetUserBookingsByIdQueryHandler> logger)
	: IRequestHandler<GetUserBookingsByIdQuery, PaginationWrapperDto<BookingModel>>
{
	public async Task<PaginationWrapperDto<BookingModel>> Handle(
		GetUserBookingsByIdQuery request,
		CancellationToken cancellationToken)
	{
		if (request.Filters.Length != request.FilterValues.Length)
			throw new InvalidOperationException("The number of Filters and FilterValues must be the same.");

		var filters = request.Filters
			.Zip(request.FilterValues, (field, value) => new FilterDto(field, value))
			.ToList();

		IQueryable<BookingEntity> query;

		if (request.UserId != Guid.Empty)
			query = bookingsRepository.Get(b => b.UserId == request.UserId);
		else
			query = bookingsRepository.Get();

		if (!string.IsNullOrWhiteSpace(request.Date))
		{
			if (!request.Date.DateFormatTryParse(out var parsedDate))
				throw new BadRequestException("Invalid date format.");

			var startDate = parsedDate.Date.ToUniversalTime();
			var endDate = startDate.AddDays(1);

			query = query.Where(b => 
				b.CreatedAt >= startDate && b.CreatedAt < endDate);
		}

		foreach (var filter in filters)
			if (!string.IsNullOrWhiteSpace(filter.Field) && !string.IsNullOrWhiteSpace(filter.Value))
				query = filter.Field.ToLower() switch
				{
					"status" => filter.Value.ToLower() switch
					{
						"notcancelled" => query.Where(
							b => b.Status != BookingStatus.Cancelled.GetDescription()),
						_ => query.Where(b => b.Status.ToLower().Contains(filter.Value.ToLower()))
					},
					_ => throw new InvalidOperationException($"Invalid filter field '{filter.Field}'.")
				};

		var totalBookings = await bookingsRepository.GetCount(query);

		if (request.SortDirection.ToLower() == "asc")
			query = request.SortBy.ToLower() switch
			{
				"status" => query.OrderBy(m => m.Status),
				"date" => query.OrderBy(m => m.CreatedAt),
				_ => throw new InvalidOperationException($"Invalid sort field '{request.SortBy}'.")
			};
		else if (request.SortDirection.ToLower() == "desc")
			query = request.SortBy.ToLower() switch
			{
				"status" => query.OrderByDescending(m => m.Status),
				"date" => query.OrderByDescending(m => m.CreatedAt),
				_ => throw new InvalidOperationException($"Invalid sort field '{request.SortBy}'.")
			};

		query = query.Skip((request.Offset - 1) * request.Limit)
			.Take(request.Limit);

		var bookingsEntities = await bookingsRepository.ToListAsync(query, cancellationToken);

		var bookings = mapper.Map<IList<BookingModel>>(bookingsEntities);

		return new PaginationWrapperDto<BookingModel>(
			bookings,
			request.Limit,
			request.Offset,
			totalBookings);
	}
}