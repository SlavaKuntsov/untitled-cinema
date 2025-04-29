// lib/presentation/providers/state/session_state.dart
import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:untitledCinema/domain/repositories/sessions_repository.dart';

import '../../domain/entities/session/session.dart';

enum SessionStatus { initial, loading, loaded, error }

class SessionsState extends Equatable {
  final List<Session> sessions;
  final SessionStatus status;
  final String? errorMessage;
  final bool hasReachedMax;

  const SessionsState({
    this.sessions = const [],
    this.status = SessionStatus.initial,
    this.errorMessage,
    this.hasReachedMax = false,
  });

  SessionsState copyWith({
    List<Session>? sessions,
    SessionStatus? status,
    String? errorMessage,
    bool? hasReachedMax,
  }) {
    return SessionsState(
      sessions: sessions ?? this.sessions,
      status: status ?? this.status,
      errorMessage: errorMessage,
      hasReachedMax: hasReachedMax ?? this.hasReachedMax,
    );
  }

  @override
  List<Object?> get props => [sessions, status, errorMessage, hasReachedMax];
}

// Provider для управления состоянием сессий
class SessionProvider extends ChangeNotifier {
  final SessionRepository _repository;

  SessionsState _sessionsState = SessionsState();
  SessionsState get sessionsState => _sessionsState;

  SessionProvider({required SessionRepository repository})
    : _repository = repository;

  Future<List<Session>> fetchSessionsByMovie({
    required String movieId,
    required String date,
    String? hall,
    int limit = 50,
    int offset = 1,
  }) async {
    try {
      _sessionsState = _sessionsState.copyWith(
        status: SessionStatus.loading,
        errorMessage: null,
      );
      notifyListeners();

      final sessions = await _repository.getSessions(
        limit: limit,
        offset: offset,
        movieId: movieId,
        date: date,
        hall: hall,
      );

      _sessionsState = _sessionsState.copyWith(
        sessions: sessions,
        status: SessionStatus.loaded,
        hasReachedMax: sessions.length < limit,
      );
      notifyListeners();

      return sessions;
    } catch (e) {
      _sessionsState = _sessionsState.copyWith(
        status: SessionStatus.error,
        errorMessage: 'Ошибка при получении сессий: ${e.toString()}',
      );
      notifyListeners();
      throw e;
    }
  }

  Future<Session> fetchSessionById({required String id}) async {
    try {
      final session = await _repository.getSessionById(id);
      return session;
    } catch (e) {
      _sessionsState = _sessionsState.copyWith(
        status: SessionStatus.error,
        errorMessage: 'Ошибка при получении сессии: ${e.toString()}',
      );
      notifyListeners();
      throw e;
    }
  }
}
