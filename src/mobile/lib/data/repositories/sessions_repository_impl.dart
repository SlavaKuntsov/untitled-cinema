// lib/data/repositories/session_repository_impl.dart
import '../../core/errors/exceptions.dart';
import '../../core/errors/failures.dart';
import '../../domain/entities/session/hall.dart';
import '../../domain/entities/session/seat_type.dart';
import '../../domain/entities/session/selected_seat.dart';
import '../../domain/entities/session/session.dart';
import '../../domain/entities/session/session_seats.dart';
import '../../domain/repositories/sessions_repository.dart';
import '../datasources/session_remote_data_source.dart';

class SessionRepositoryImpl implements SessionRepository {
  final SessionRemoteDataSource remoteDataSource;

  SessionRepositoryImpl({required this.remoteDataSource});

  @override
  Future<List<Session>> getSessions({
    int limit = 10,
    int offset = 0,
    String? movieId,
    String? date,
    String? hall,
  }) async {
    try {
      // Получаем данные из data source и возвращаем как есть,
      // поскольку SessionModel уже является наследником Session
      return await remoteDataSource.getSessions(
        limit: limit,
        offset: offset,
        movieId: movieId,
        date: date,
        hall: hall,
      );
    } catch (e) {
      if (e is ServerException) {
        throw ServerFailure(e.message);
      }
      throw ServerFailure('Ошибка при получении сессий: ${e.toString()}');
    }
  }

  @override
  Future<Session> getSessionById(String id) async {
    try {
      return await remoteDataSource.getSessionById(id);
    } catch (e) {
      if (e is ServerException) {
        throw ServerFailure(e.message);
      }
      throw ServerFailure('Ошибка при получении сессии: ${e.toString()}');
    }
  }

  @override
  Future<List<Hall>> getHalls() async {
    try {
      return await remoteDataSource.getHalls();
    } catch (e) {
      if (e is ServerException) {
        throw ServerFailure(e.message);
      }
      throw ServerFailure('Ошибка при получении залов: ${e.toString()}');
    }
  }

  @override
  Future<Hall> getHallById(String id) async {
    try {
      final qwe = await remoteDataSource.getHallById(id);
      return qwe;
    } catch (e) {
      if (e is ServerException) {
        throw ServerFailure(e.message);
      }
      throw ServerFailure('Ошибка при получении зала: ${e.toString()}');
    }
  }

  @override
  Future<List<SeatType>> getSeatTypesByHallId(String hallId) async {
    try {
      return await remoteDataSource.getSeatTypesByHallId(hallId);
    } catch (e) {
      if (e is ServerException) {
        throw ServerFailure(e.message);
      }
      throw ServerFailure(
        'Ошибка при получении типов сидений по залу: ${e.toString()}',
      );
    }
  }

  @override
  Future<SessionSeats> getReservedSeats(String sessionId) async {
    try {
      return await remoteDataSource.getReservedSeats(sessionId);
    } catch (e) {
      if (e is ServerException) {
        throw ServerFailure(e.message);
      }
      throw ServerFailure(
        'Ошибка при получении забронированных мест: ${e.toString()}',
      );
    }
  }

  @override
  Future<void> startSeatsConnection(String sessionId) async {
    try {
      await remoteDataSource.startSeatsConnection(sessionId);
    } catch (e) {
      throw ServerFailure(
        'Ошибка при подключении к обновлениям: ${e.toString()}',
      );
    }
  }

  @override
  Future<void> stopSeatsConnection(String sessionId) async {
    try {
      await remoteDataSource.stopSeatsConnection(sessionId);
    } catch (e) {
      // Игнорируем ошибки при отключении
    }
  }

  @override
  Stream<SessionSeats> seatsUpdateStream() {
    return remoteDataSource.seatsUpdateStream();
  }

  @override
  Future<SelectedSeat> getSelectedSeat(
    String sessionId,
    int row,
    int column,
  ) async {
    return await remoteDataSource.getSelectedSeat(sessionId, row, column);
  }
}
