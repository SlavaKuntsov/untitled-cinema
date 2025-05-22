// lib/domain/repositories/booking_repository.dart
import 'package:dartz/dartz.dart';

import '../../core/errors/failures.dart';
import '../entities/bookings/bookings.dart';
import '../entities/paginated_items.dart';
import '../entities/session/seat.dart';

abstract class BookingRepository {
  Future<PaginatedItems<Booking>> getBookingHistory({
    required String userId,
    int limit = 10,
    int offset = 1,
    List<String>? filters,
    List<String>? filterValues,
    String? sortBy,
    String? sortDirection,
    String? date,
  });

  Future<Either<Failure, Booking>> getBookingById(String bookingId);

  Future<Either<Failure, bool>> createBooking({
    required String userId,
    required String sessionId,
    required List<Seat> seats,
  });

  Future<Either<Failure, bool>> cancelBooking(String bookingId);
  Future<Either<Failure, bool>> payBooking(String bookingId, String userId);
}
