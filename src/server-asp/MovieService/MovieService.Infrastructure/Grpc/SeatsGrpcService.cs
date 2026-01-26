// using MovieService.Domain.Entities;
// using MovieService.Domain.Interfaces.Grpc;
// using Protobufs.Seats;
//
// namespace MovieService.Infrastructure.Grpc;
//
// public class SeatsGrpcService(SeatsService.SeatsServiceClient client) : ISeatsGrpcService
// {
// 	public async Task CreateEmptySessionSeats(
// 		Guid sessionId,
// 		IList<SeatEntity> availableSeats,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var request = new CreateSeatsRequest
// 		{
// 			SessionId = sessionId.ToString(),
// 			Seats =
// 			{
// 				availableSeats.Select(
// 					seat => new Seat
// 					{
// 						Id = seat.Id.ToString(),
// 						Row = seat.Row,
// 						Column = seat.Column
// 					})
// 			}
// 		};
//
// 		await client.CreateSeatsAsync(request, cancellationToken: cancellationToken);
// 	}
// }