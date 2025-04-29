// lib/data/repositories/session_repository_impl.dart
import '../../core/errors/exceptions.dart';
import '../../core/errors/failures.dart';
import '../../domain/entities/session/session.dart';
import '../../domain/repositories/sessions_repository.dart';
import '../datasources/session_remote_data_source.dart';

class SessionRepositoryImpl implements SessionRepository {
  final SessionRemoteDataSource remoteDataSource;

  SessionRepositoryImpl({required this.remoteDataSource});

  @override
  Future<List<Session>> getSessions({
    int limit = 50,
    int offset = 1,
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
}
