// lib/data/datasources/session_remote_data_source.dart
import 'package:dio/dio.dart';

// lib/data/datasources/session_remote_data_source_impl.dart
import '../../core/constants/api_constants.dart';
import '../../core/errors/exceptions.dart';
import '../models/session/session_model.dart';

abstract class SessionRemoteDataSource {
  /// Получает список сессий с сервера
  /// Параметры:
  /// [limit] - количество сессий для получения
  /// [offset] - смещение от начала списка
  /// [movieId] - ID фильма для фильтрации (опционально)
  /// [date] - дата для фильтрации в формате DD-MM-YYYY (опционально)
  /// [hall] - ID зала для фильтрации (опционально)
  ///
  /// Возвращает список [SessionModel]
  /// Выбрасывает [ServerException] при ошибке
  Future<List<SessionModel>> getSessions({
    int limit = 10,
    int offset = 0,
    String? movieId,
    String? date,
    String? hall,
  });

  /// Получает информацию о конкретной сессии по ID
  /// Параметры:
  /// [id] - ID сессии
  ///
  /// Возвращает [SessionModel]
  /// Выбрасывает [ServerException] при ошибке
  Future<SessionModel> getSessionById(String id);
}

class SessionRemoteDataSourceImpl implements SessionRemoteDataSource {
  final Dio client;

  SessionRemoteDataSourceImpl({required this.client});

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
}
