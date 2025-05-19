// lib/data/repositories/booking_repository_impl.dart
import 'package:dartz/dartz.dart';

import '../../core/errors/exceptions.dart';
import '../../core/errors/failures.dart';
import '../../core/network/network_info.dart';
import '../../domain/entities/bookings/bookings.dart';
import '../../domain/entities/paginated_items.dart';
import '../../domain/repositories/booking_repository.dart';
import '../datasources/booking_remote_data_source.dart';

class BookingRepositoryImpl implements BookingRepository {
  final BookingRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  BookingRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<PaginatedItems<Booking>> getBookingHistory({
    required String userId,
    int limit = 10,
    int offset = 1,
    List<String>? filters,
    List<String>? filterValues,
    String? sortBy,
    String? sortDirection,
    String? date,
  }) async {
    return await remoteDataSource.getBookingHistory(
      userId: userId,
      limit: limit,
      offset: offset,
      filters: filters,
      filterValues: filterValues,
      sortBy: sortBy,
      sortDirection: sortDirection,
      date: date,
    );
  }

  @override
  Future<Either<Failure, Booking>> getBookingById(String bookingId) async {
    if (await networkInfo.isConnected) {
      try {
        final booking = await remoteDataSource.getBookingById(bookingId);
        return Right(booking);
      } on ServerException catch (e) {
        return Left(ServerFailure(e.message));
      }
    } else {
      return Left(NetworkFailure('Отсутствует подключение к интернету'));
    }
  }

  @override
  Future<Either<Failure, Booking>> createBooking({
    required String sessionId,
    required List<Map<String, dynamic>> seats,
  }) async {
    if (await networkInfo.isConnected) {
      try {
        final booking = await remoteDataSource.createBooking(
          sessionId: sessionId,
          seats: seats,
        );
        return Right(booking);
      } on ServerException catch (e) {
        return Left(ServerFailure(e.message));
      }
    } else {
      return Left(NetworkFailure('Отсутствует подключение к интернету'));
    }
  }

  @override
  Future<Either<Failure, bool>> cancelBooking(String bookingId) async {
    if (await networkInfo.isConnected) {
      try {
        final result = await remoteDataSource.cancelBooking(bookingId);
        return Right(result);
      } on ServerException catch (e) {
        return Left(ServerFailure(e.message));
      }
    } else {
      return Left(NetworkFailure('Отсутствует подключение к интернету'));
    }
  }
}
