// lib/data/datasources/session_remote_data_source.dart
import 'package:dio/dio.dart';

import '../../core/constants/api_constants.dart';
import '../../core/errors/exceptions.dart';
import '../../core/services/signalr_service.dart';
import '../../domain/entities/session/selected_seat.dart';
import '../models/session/hall_model.dart';
import '../models/session/seat_type_model.dart';
import '../models/session/selected_seat_model.dart';
import '../models/session/session_model.dart';
import '../models/session/session_seats_model.dart';

abstract class SessionRemoteDataSource {
  /// Получает список сессий с сервера
  Future<List<SessionModel>> getSessions({
    int limit = 10,
    int offset = 0,
    String? movieId,
    String? date,
    String? hall,
  });

  /// Получает информацию о конкретной сессии по ID
  Future<SessionModel> getSessionById(String id);

  /// Получает список всех залов
  Future<List<HallModel>> getHalls();

  /// Получает зал по ID
  Future<HallModel> getHallById(String id);

  /// Получает типы сидений по ID зала
  Future<List<SeatTypeModel>> getSeatTypesByHallId(String hallId);

  /// Получает список забронированных мест для сессии
  Future<SessionSeatsModel> getReservedSeats(String sessionId);

  /// Запускает соединение с хабом сигналов для обновления бронирований
  Future<void> startSeatsConnection(String sessionId);

  /// Останавливает соединение с хабом сигналов
  Future<void> stopSeatsConnection(String sessionId);

  /// Возвращает поток обновлений бронирований
  Stream<SessionSeatsModel> seatsUpdateStream();

  Future<SelectedSeat> getSelectedSeat(String sessionId, int row, int column);
}

class SessionRemoteDataSourceImpl implements SessionRemoteDataSource {
  final Dio client;
  final SignalRService signalRService;

  SessionRemoteDataSourceImpl({
    required this.client,
    required this.signalRService,
  });

  @override
  Future<List<SessionModel>> getSessions({
    int limit = 10,
    int offset = 0,
    String? movieId,
    String? date,
    String? hall,
  }) async {
    try {
      // Формируем URL с параметрами запроса
      final queryParams = <String, String>{
        'limit': limit.toString(),
        'offset': offset.toString(),
      };

      if (movieId != null) queryParams['movie'] = movieId;
      if (date != null) queryParams['date'] = date;
      if (hall != null) queryParams['hall'] = hall;

      // Выполняем запрос к API
      final response = await client.get(
        ApiConstants.sessions,
        queryParameters: queryParams,
      );

      // Проверяем, что ответ успешный
      if (response.statusCode == 200) {
        // Преобразуем данные в список моделей SessionModel
        final List<dynamic> data = response.data;
        return data.map((json) => SessionModel.fromJson(json)).toList();
      } else {
        throw ServerException(
          'Ошибка при получении сессий: ${response.statusCode}',
        );
      }
    } catch (e) {
      if (e is ServerException) {
        throw e; // Пробрасываем специальные исключения
      }
      throw ServerException('Ошибка при получении сессий: ${e.toString()}');
    }
  }

  @override
  Future<SessionModel> getSessionById(String id) async {
    try {
      final response = await client.get('${ApiConstants.sessions}/$id');

      if (response.statusCode == 200) {
        return SessionModel.fromJson(response.data);
      } else {
        throw ServerException(
          'Ошибка при получении сессии: ${response.statusCode}',
        );
      }
    } catch (e) {
      if (e is ServerException) {
        throw e;
      }
      throw ServerException('Ошибка при получении сессии: ${e.toString()}');
    }
  }

  @override
  Future<List<HallModel>> getHalls() async {
    try {
      final response = await client.get(ApiConstants.halls);

      if (response.statusCode == 200) {
        final List<dynamic> data = response.data;
        return data.map((json) => HallModel.fromJson(json)).toList();
      } else {
        throw ServerException(
          'Ошибка при получении залов: ${response.statusCode}',
        );
      }
    } catch (e) {
      if (e is ServerException) {
        throw e;
      }
      throw ServerException('Ошибка при получении залов: ${e.toString()}');
    }
  }

  // Обновленный метод в SessionRemoteDataSourceImpl
  @override
  Future<HallModel> getHallById(String id) async {
    try {
      final response = await client.get('${ApiConstants.halls}/$id');

      if (response.statusCode == 200) {
        // Проверяем тип данных в ответе
        if (response.data is Map<String, dynamic>) {
          // Если ответ - это объект, используем его напрямую
          return HallModel.fromJson(response.data);
        } else if (response.data is List && response.data.isNotEmpty) {
          // Если ответ - это список, берем первый элемент
          return HallModel.fromJson(response.data[0]);
        } else {
          throw ServerException('Неверный формат данных при получении зала');
        }
      } else {
        throw ServerException(
          'Ошибка при получении зала: ${response.statusCode}',
        );
      }
    } catch (e) {
      if (e is ServerException) {
        throw e;
      }
      throw ServerException('Ошибка при получении зала: ${e.toString()}');
    }
  }

  @override
  Future<List<SeatTypeModel>> getSeatTypesByHallId(String hallId) async {
    try {
      final response = await client.get(
        '${ApiConstants.seatsType}/hall/$hallId',
      );

      if (response.statusCode == 200) {
        final List<dynamic> data = response.data;
        return data.map((json) => SeatTypeModel.fromJson(json)).toList();
      } else {
        throw ServerException(
          'Ошибка при получении типов сидений по залу: ${response.statusCode}',
        );
      }
    } catch (e) {
      if (e is ServerException) {
        throw e;
      }
      throw ServerException(
        'Ошибка при получении типов сидений по залу: ${e.toString()}',
      );
    }
  }

  @override
  Future<SessionSeatsModel> getReservedSeats(String sessionId) async {
    try {
      final response = await client.get(
        '${ApiConstants.reservedSeats}/$sessionId',
      );

      if (response.statusCode == 200) {
        return SessionSeatsModel.fromJson(response.data);
      } else {
        throw ServerException(
          'Ошибка при получении забронированных мест: ${response.statusCode}',
        );
      }
    } catch (e) {
      if (e is ServerException) {
        throw e;
      }
      throw ServerException(
        'Ошибка при получении забронированных мест: ${e.toString()}',
      );
    }
  }

  @override
  Future<void> startSeatsConnection(String sessionId) async {
    await signalRService.startConnection(sessionId);
  }

  @override
  Future<void> stopSeatsConnection(String sessionId) async {
    await signalRService.stopConnection(sessionId);
  }

  @override
  Stream<SessionSeatsModel> seatsUpdateStream() {
    return signalRService.seatChangedStream.map(
      (event) => SessionSeatsModel.fromJson(event),
    );
  }

  @override
  Future<SelectedSeat> getSelectedSeat(
    String sessionId,
    int row,
    int column,
  ) async {
    try {
      final response = await client.get(
        '${ApiConstants.seats}/$sessionId/$row/$column',
      );

      if (response.statusCode == 200) {
        return SelectedSeatModel.fromJson(response.data);
      } else {
        throw ServerException(
          'Ошибка при получении забронированных мест: ${response.statusCode}',
        );
      }
    } catch (e) {
      if (e is ServerException) {
        throw e;
      }
      throw ServerException(
        'Ошибка при получении забронированных мест: ${e.toString()}',
      );
    }
  }
}
