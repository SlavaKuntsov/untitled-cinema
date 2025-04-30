// lib/domain/repositories/session_repository.dart
import '../entities/session/hall.dart';
import '../entities/session/seat_type.dart';
import '../entities/session/session.dart';

abstract class SessionRepository {
  /// Получает список сессий
  Future<List<Session>> getSessions({
    int limit = 10,
    int offset = 0,
    String? movieId,
    String? date,
    String? hall,
  });

  /// Получает информацию о конкретной сессии по ID
  Future<Session> getSessionById(String id);

  /// Получает список всех залов
  Future<List<Hall>> getHalls();

  /// Получает зал по ID
  Future<Hall> getHallById(String id);

  /// Получает типы сидений по ID зала
  Future<List<SeatType>> getSeatTypesByHallId(String hallId);
}
