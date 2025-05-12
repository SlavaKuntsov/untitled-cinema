// lib/presentation/providers/state/session_state.dart
import 'dart:async';

import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:untitledCinema/domain/repositories/sessions_repository.dart';

import '../../../domain/entities/session/hall.dart';
import '../../../domain/entities/session/session.dart';
import '../../domain/entities/session/seat.dart';
import '../../domain/entities/session/seat_type.dart';
import '../../domain/entities/session/selected_seat.dart';

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

  // Добавляем поля для забронированных мест
  List<Seat> _reservedSeats = [];
  List<Seat> get reservedSeats => _reservedSeats;

  bool _isLoadingReservedSeats = false;
  bool get isLoadingReservedSeats => _isLoadingReservedSeats;

  String? _reservedSeatsErrorMessage;
  String? get reservedSeatsErrorMessage => _reservedSeatsErrorMessage;

  // Выбранные места
  List<SelectedSeat> _selectedSeats = [];
  List<SelectedSeat> get selectedSeats => _selectedSeats;

  // Подписка на обновления
  StreamSubscription? _seatsUpdateSubscription;

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

  // Метод для получения забронированных мест
  Future<List<Seat>> fetchReservedSeats(String sessionId) async {
    try {
      _isLoadingReservedSeats = true;
      _reservedSeatsErrorMessage = null;
      notifyListeners();

      final sessionSeats = await _repository.getReservedSeats(sessionId);
      _reservedSeats = sessionSeats.reservedSeats;

      _isLoadingReservedSeats = false;
      notifyListeners();

      return _reservedSeats;
    } catch (e) {
      _isLoadingReservedSeats = false;
      _reservedSeatsErrorMessage =
          'Ошибка при получении забронированных мест: ${e.toString()}';
      notifyListeners();
      throw e;
    }
  }

  // Проверка забронированных мест
  bool isReserved(int rowIndex, int seatIndex) {
    final int rowNumber = rowIndex + 1;
    final int seatNumber = seatIndex + 1;

    return _reservedSeats.any(
      (seat) => seat.row == rowNumber && seat.column == seatNumber,
    );
  }

  // Методы для работы с подпиской на обновления
  Future<void> startSeatsConnection(String sessionId) async {
    try {
      await _repository.startSeatsConnection(sessionId);

      // Подписываемся на обновления
      _seatsUpdateSubscription = _repository.seatsUpdateStream().listen((
        sessionSeats,
      ) {
        _reservedSeats = sessionSeats.reservedSeats;
        notifyListeners();
      });
    } catch (e) {
      _reservedSeatsErrorMessage =
          'Ошибка при подключении к обновлениям: ${e.toString()}';
      notifyListeners();
    }
  }

  Future<void> stopSeatsConnection(String sessionId) async {
    try {
      await _repository.stopSeatsConnection(sessionId);
      _seatsUpdateSubscription?.cancel();
      _seatsUpdateSubscription = null;
    } catch (e) {
      // Игнорируем ошибки при отключении
    }
  }

  // Выбор места пользователем
  Future toggleSeatSelection(
    int rowIndex,
    int seatIndex,
    String sessionId,
  ) async {
    final int rowNumber = rowIndex + 1;
    final int seatNumber = seatIndex + 1;

    // Проверяем, забронировано ли место
    if (isReserved(rowIndex, seatIndex)) {
      return; // Если забронировано - не позволяем выбрать
    }

    // Проверяем, выбрано ли уже место
    final isAlreadySelected = _selectedSeats.any(
      (seat) => seat.row == rowNumber && seat.column == seatNumber,
    );

    if (isAlreadySelected) {
      // Если выбрано - удаляем из выбранных
      _selectedSeats.removeWhere(
        (seat) => seat.row == rowNumber && seat.column == seatNumber,
      );
    } else {
      // Находим тип места
      final seatType = await _repository.getSelectedSeat(
        sessionId,
        rowNumber,
        seatNumber,
      );
      // final seatType = getSeatTypeById(seatTypeValue.toString());

      final seatTypeModel = SeatType(
        id: seatType.seatType.id,
        name: seatType.seatType.name,
        priceModifier: seatType.price,
      );

      // Создаем объект выбранного места
      final selectedSeat = SelectedSeat(
        id: seatType.id,
        row: rowNumber,
        column: seatNumber,
        seatType: seatTypeModel,
        price: seatType.price,
      );

      _selectedSeats.add(selectedSeat);
    }

    notifyListeners();
  }

  // Вспомогательная функция для получения типа места по ID
  SeatType getSeatTypeById(String typeId) {
    try {
      return _hallSeatTypes.firstWhere((type) => type.id == typeId);
    } catch (_) {
      // Если тип не найден - используем первый в списке или создаем стандартный тип
      if (_hallSeatTypes.isNotEmpty) {
        return _hallSeatTypes.first;
      }
      // Возвращаем стандартный тип
      return SeatType(id: typeId, name: 'Стандарт', priceModifier: 1.0);
    }
  }

  // Очистка выбранных мест
  void clearSelectedSeats() {
    _selectedSeats = [];
    notifyListeners();
  }

  // Получение цвета места по типу
  Color getSeatColor(int seatTypeValue) {
    // Список цветов по типам мест
    final List<Color> colors = [
      Colors.blue, // Стандарт
      Colors.orange, // Комфорт
      Colors.red, // Премиум
      Colors.purple, // VIP
    ];

    // Получаем индекс типа из значения (или используем 0 по умолчанию)
    int typeIndex = 0;
    try {
      typeIndex = int.parse(seatTypeValue.toString()) - 1;
      if (typeIndex < 0) typeIndex = 0;
      if (typeIndex >= colors.length) typeIndex = 0;
    } catch (e) {
      typeIndex = 0;
    }

    return colors[typeIndex];
  }

  @override
  void dispose() {
    _seatsUpdateSubscription?.cancel();
    super.dispose();
  }
}
