// lib/domain/repositories/booking_repository.dart
import 'package:dartz/dartz.dart';

import '../../core/errors/failures.dart';
import '../entities/bookings/bookings.dart';
import '../entities/paginated_items.dart';

abstract class BookingRepository {
  /// Получает историю бронирований пользователя
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

  /// Получает детали бронирования по ID
  Future<Either<Failure, Booking>> getBookingById(String bookingId);

  /// Создает новое бронирование
  Future<Either<Failure, Booking>> createBooking({
    required String sessionId,
    required List<Map<String, dynamic>> seats,
  });

  /// Отменяет бронирование
  Future<Either<Failure, bool>> cancelBooking(String bookingId);
}
