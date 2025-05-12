// lib/domain/repositories/session_repository.dart
import 'package:untitledCinema/domain/entities/session/selected_seat.dart';

import '../entities/session/hall.dart';
import '../entities/session/seat_type.dart';
import '../entities/session/session.dart';
import '../entities/session/session_seats.dart';

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

  /// Получает список забронированных мест для сессии
  Future<SessionSeats> getReservedSeats(String sessionId);

  /// Запускает соединение с хабом для обновлений бронирований
  Future<void> startSeatsConnection(String sessionId);

  /// Останавливает соединение с хабом
  Future<void> stopSeatsConnection(String sessionId);

  /// Получает поток обновлений бронирований
  Stream<SessionSeats> seatsUpdateStream();

  Future<SelectedSeat> getSelectedSeat(String sessionId, int row, int column);
}
