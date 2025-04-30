// lib/presentation/providers/state/session_state.dart
import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:untitledCinema/domain/repositories/sessions_repository.dart';

import '../../../domain/entities/session/hall.dart';
import '../../../domain/entities/session/session.dart';
import '../../domain/entities/session/seat_type.dart';

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

  // Добавляем поля для залов
  List<Hall> _halls = [];
  List<Hall> get halls => _halls;

  bool _isLoadingHalls = false;
  bool get isLoadingHalls => _isLoadingHalls;

  String? _hallErrorMessage;
  String? get hallErrorMessage => _hallErrorMessage;

  Hall? _selectedHall;
  Hall? get selectedHall => _selectedHall;

  // Состояние для типов сидений
  List<SeatType> _seatTypes = [];
  List<SeatType> get seatTypes => _seatTypes;

  List<SeatType> _hallSeatTypes = [];
  List<SeatType> get hallSeatTypes => _hallSeatTypes;

  bool _isLoadingSeatTypes = false;
  bool get isLoadingSeatTypes => _isLoadingSeatTypes;

  String? _seatTypeErrorMessage;
  String? get seatTypeErrorMessage => _seatTypeErrorMessage;

  SeatType? _selectedSeatType;
  SeatType? get selectedSeatType => _selectedSeatType;

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

  Future<List<Hall>> fetchAllHalls() async {
    try {
      _isLoadingHalls = true; // Добавили установку флага загрузки
      _hallErrorMessage = null;
      notifyListeners();

      _halls = await _repository.getHalls();

      _isLoadingHalls = false;
      notifyListeners();

      return _halls;
    } catch (e) {
      _isLoadingHalls = false;
      _hallErrorMessage = 'Ошибка при получении залов: ${e.toString()}';
      notifyListeners();
      throw e;
    }
  }

  Future<Hall> fetchHallById({required String id}) async {
    try {
      _isLoadingHalls = true;
      _hallErrorMessage = null;
      notifyListeners();

      _selectedHall = await _repository.getHallById(id);

      _isLoadingHalls = false;
      notifyListeners();

      return _selectedHall!;
    } catch (e) {
      _isLoadingHalls = false;
      _hallErrorMessage = 'Ошибка при получении зала: ${e.toString()}';
      notifyListeners();
      throw e;
    }
  }

  void clearSelectedHall() {
    _selectedHall = null;
    notifyListeners();
  }

  Future<List<SeatType>> fetchSeatTypesByHallId({
    required String hallId,
  }) async {
    try {
      _isLoadingSeatTypes = true;
      _seatTypeErrorMessage = null;
      notifyListeners();

      _hallSeatTypes = await _repository.getSeatTypesByHallId(hallId);

      _isLoadingSeatTypes = false;
      notifyListeners();

      return _hallSeatTypes;
    } catch (e) {
      _isLoadingSeatTypes = false;
      _seatTypeErrorMessage =
          'Ошибка при получении типов сидений по залу: ${e.toString()}';
      notifyListeners();
      throw e;
    }
  }

  void clearSelectedSeatType() {
    _selectedSeatType = null;
    notifyListeners();
  }
}
