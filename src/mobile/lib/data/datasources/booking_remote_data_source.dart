import 'dart:core';

import '../../core/constants/api_constants.dart';
import '../../core/errors/exceptions.dart';
import '../../core/network/api_client.dart';
import '../../domain/entities/bookings/bookings.dart';
import '../../domain/entities/paginated_items.dart';
import '../models/bookings/booking_model.dart';
import '../models/paginated_response.dart';

abstract class BookingRemoteDataSource {
  /// Получает историю бронирований пользователя
  Future<PaginatedItems<Booking>> getBookingHistory({
    required String userId,
    int limit = 10,
    int offset = 0,
    List<String>? filters,
    List<String>? filterValues,
    String? sortBy,
    String? sortDirection,
    String? date,
  });

  /// Получает детали бронирования по ID
  Future<BookingModel> getBookingById(String bookingId);

  /// Создает новое бронирование
  Future<BookingModel> createBooking({
    required String sessionId,
    required List<Map<String, dynamic>> seats,
  });

  /// Отменяет бронирование
  Future<bool> cancelBooking(String bookingId);
}

class BookingRemoteDataSourceImpl implements BookingRemoteDataSource {
  final ApiClient client;

  BookingRemoteDataSourceImpl({required this.client});

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
    try {
      final queryParams = <String, dynamic>{
        'UserId': userId.toString(),
        'Limit': limit.toString(),
        'Offset': offset.toString(),
      };

      if (sortBy != null) {
        queryParams['SortBy'] = sortBy;
      } else {
        queryParams['SortBy'] = 'date';
      }

      if (sortDirection != null) {
        queryParams['SortDirection'] = sortDirection;
      } else {
        queryParams['SortDirection'] = 'desc';
      }

      if (date != null) {
        queryParams['Date'] = date.toString();
      }

      if (filters != null &&
          filterValues != null &&
          filters.length > 0 &&
          filters.length == filterValues.length) {
        final Map<String, List<String>> multiParams = {};

        for (var i = 0; i < filters.length; i++) {
          if (!multiParams.containsKey('Filter')) {
            multiParams['Filter'] = [];
          }
          multiParams['Filter']!.add(filters[i]);

          if (!multiParams.containsKey('FilterValue')) {
            multiParams['FilterValue'] = [];
          }
          multiParams['FilterValue']!.add(filterValues[i]);
        }

        multiParams.forEach((key, values) {
          for (var i = 0; i < values.length; i++) {
            queryParams['$key'] = values[i];
          }
        });
      }

      final dynamic response = await client.get(
        ApiConstants.bookingsHistory,
        queryParameters: queryParams,
      );

      if (response != null && response is Map<String, dynamic>) {
        return PaginatedResponseModel<BookingModel>.fromJson(
          response,
          (json) => BookingModel.fromJson(json),
        );
      } else {
        throw ServerException('Некорректный формат данных от сервера');
      }
    } catch (e) {
      if (e is ServerException) {
        throw e; // Пробрасываем специальные исключения
      }
      throw ServerException(
        'Ошибка при получении истории бронирований: ${e.toString()}',
      );
    }
  }

  @override
  Future<BookingModel> getBookingById(String bookingId) async {
    final response = await client.get('${ApiConstants.bookings}/$bookingId');

    if (response.statusCode == 200) {
      return BookingModel.fromJson(response.data);
    } else {
      throw Exception('Failed to load movie poster: ${response.statusCode}');
    }
  }

  @override
  Future<BookingModel> createBooking({
    required String sessionId,
    required List<Map<String, dynamic>> seats,
  }) async {
    final body = {'sessionId': sessionId, 'seats': seats};

    final response = await client.post(ApiConstants.bookings, data: body);

    if (response.statusCode == 201) {
      return BookingModel.fromJson(response.data);
    } else {
      throw Exception('Failed to load movie poster: ${response.statusCode}');
    }
  }

  @override
  Future<bool> cancelBooking(String bookingId) async {
    final response = await client.delete(
      '${ApiConstants.bookings}/$bookingId/cancel',
    );

    if (response.statusCode == 200) {
      return true;
    } else {
      throw Exception('Failed to load movie poster: ${response.statusCode}');
    }
  }
}
