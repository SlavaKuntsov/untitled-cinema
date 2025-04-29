// lib/domain/repositories/session_repository.dart
import '../entities/session/session.dart';

abstract class SessionRepository {
  Future<List<Session>> getSessions({
    int limit = 10,
    int offset = 0,
    String? movieId,
    String? date,
    String? hall,
  });

  Future<Session> getSessionById(String id);
}
