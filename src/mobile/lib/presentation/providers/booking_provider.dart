// lib/presentation/providers/booking_provider.dart
import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:untitledCinema/domain/entities/session/seat.dart';

import '../../domain/entities/bookings/bookings.dart';
import '../../domain/repositories/booking_repository.dart';

enum BookingLoadingStatus { initial, loading, loaded, error }

class _FilterPair {
  final List<String> filters;
  final List<String> values;

  _FilterPair({required this.filters, required this.values});
}

class BookingState {
  final List<Booking> bookings;
  final int total;
  final BookingLoadingStatus status;
  final String? errorMessage;
  final int currentPage;
  final int pageSize;
  final String nextRef;
  final String prevRef;

  bool get hasNextPage => (currentPage) * pageSize < total;
  bool get hasPrevPage => currentPage > 1;

  BookingState({
    this.bookings = const [],
    this.total = 0,
    this.status = BookingLoadingStatus.initial,
    this.errorMessage,
    this.currentPage = 1,
    this.pageSize = 10,
    this.nextRef = '',
    this.prevRef = '',
  });

  BookingState copyWith({
    List<Booking>? bookings,
    int? total,
    BookingLoadingStatus? status,
    String? errorMessage,
    int? currentPage,
    int? pageSize,
    String? nextRef,
    String? prevRef,
  }) {
    return BookingState(
      bookings: bookings ?? this.bookings,
      total: total ?? this.total,
      status: status ?? this.status,
      errorMessage: errorMessage,
      currentPage: currentPage ?? this.currentPage,
      pageSize: pageSize ?? this.pageSize,
      nextRef: nextRef ?? this.nextRef,
      prevRef: prevRef ?? this.prevRef,
    );
  }
}

class BookingProvider extends ChangeNotifier {
  final BookingRepository _repository;
  final SharedPreferences _prefs;

  List<String>? _currentFilters;
  List<String>? _currentFilterValues;
  String? _currentDate;

  BookingState _bookingState = BookingState();
  BookingState get bookingState => _bookingState;

  Booking? _selectedBooking;
  Booking? get selectedBooking => _selectedBooking;

  bool _isCreating = false;
  bool get isCreating => _isCreating;

  bool _isCanceling = false;
  bool get isCanceling => _isCanceling;

  bool _isPaying = false;
  bool get isPaying => _isPaying;

  String? _operationError;
  String? get operationError => _operationError;

  BookingProvider({
    required BookingRepository repository,
    required SharedPreferences prefs,
  }) : _repository = repository,
       _prefs = prefs;

  Future<void> fetchBookingHistory({
    required String userId,
    int? page,
    int? limit,
    List<String>? filters,
    List<String>? filterValues,
    String? sortBy,
    String? sortDirection,
    String? date,
  }) async {
    try {
      _currentFilters = filters;
      _currentFilterValues = filterValues;
      _currentDate = date;

      _bookingState = _bookingState.copyWith(
        status: BookingLoadingStatus.loading,
        errorMessage: null,
      );
      notifyListeners();

      final pageSize = limit ?? _bookingState.pageSize;
      final currentPage = page ?? _bookingState.currentPage;
      final offset = currentPage ?? _bookingState.currentPage;

      final result = await _repository.getBookingHistory(
        userId: userId,
        limit: pageSize,
        offset: offset,
        filters: filters,
        filterValues: filterValues,
        sortBy: sortBy,
        sortDirection: sortDirection,
        date: date,
      );

      _bookingState = _bookingState.copyWith(
        bookings: result.items,
        total: result.total,
        status: BookingLoadingStatus.loaded,
        currentPage: currentPage,
        pageSize: pageSize,
        nextRef: result.nextRef ?? '',
        prevRef: result.prevRef ?? '',
      );
      notifyListeners();
    } catch (e) {
      _bookingState = _bookingState.copyWith(
        status: BookingLoadingStatus.error,
        errorMessage: e.toString(),
      );
      notifyListeners();
    }
  }

  Future<void> fetchBookingById(String bookingId) async {
    _bookingState = _bookingState.copyWith(
      status: BookingLoadingStatus.loading,
      errorMessage: null,
    );
    notifyListeners();

    final result = await _repository.getBookingById(bookingId);

    result.fold(
      (failure) {
        _bookingState = _bookingState.copyWith(
          status: BookingLoadingStatus.error,
          errorMessage: failure.message,
        );
        _selectedBooking = null;
        notifyListeners();
      },
      (booking) {
        _selectedBooking = booking;
        _bookingState = _bookingState.copyWith(
          status: BookingLoadingStatus.loaded,
        );
        notifyListeners();
      },
    );
  }

  Future<bool> createBooking({
    required String userId,
    required String sessionId,
    required List<Seat> seats,
  }) async {
    _isCreating = true;
    _operationError = null;
    notifyListeners();

    final result = await _repository.createBooking(
      userId: userId,
      sessionId: sessionId,
      seats: seats,
    );

    bool success = false;
    result.fold(
      (failure) {
        _operationError = failure.message;
        success = false;
      },
      (isSuccess) {
        success = isSuccess;
        if (isSuccess) {
          refreshBookings(userId);
        }
      },
    );

    _isCreating = false;
    notifyListeners();
    return success;
  }

  Future<bool> cancelBooking(String bookingId) async {
    // _isCanceling = true;
    // _operationError = null;
    // notifyListeners();
    //
    // final result = await _repository.cancelBooking(bookingId);
    //
    // bool success = false;
    // result.fold(
    //   (failure) {
    //     _operationError = failure.message;
    //     success = false;
    //   },
    //   (opSuccess) {
    //     success = opSuccess;
    //     if (opSuccess) {
    //       if (_selectedBooking?.id == bookingId) {
    //         // _selectedBooking = Booking(
    //         //   id: _selectedBooking!.id,
    //         //   userId: _selectedBooking!.userId,
    //         //   sessionId: _selectedBooking!.sessionId,
    //         //   seats: _selectedBooking!.seats,
    //         //   status: 'Cancelled',
    //         //   createdAt: _selectedBooking!.createdAt,
    //         //   updatedAt: DateTime.now(),
    //         // );
    //       }
    //       refreshBookings(_prefs.getString('userId')!);
    //     }
    //   },
    // );
    //
    // _isCanceling = false;
    // notifyListeners();
    // return success;

    try {
      _bookingState = _bookingState.copyWith(
        status: BookingLoadingStatus.loading,
        errorMessage: null,
      );
      notifyListeners();

      final result = await _repository.cancelBooking(bookingId);

      _bookingState = _bookingState.copyWith(
        status: BookingLoadingStatus.loaded,
      );
      notifyListeners();

      return true;
    } catch (e) {
      _bookingState = _bookingState.copyWith(
        status: BookingLoadingStatus.error,
        errorMessage: e.toString(),
      );
      notifyListeners();
      return false;
    }
  }

  Future<bool> payBooking(String bookingId, String userId) async {
    try {
      _bookingState = _bookingState.copyWith(
        status: BookingLoadingStatus.loading,
        errorMessage: null,
      );
      notifyListeners();

      final result = await _repository.payBooking(bookingId, userId);

      _bookingState = _bookingState.copyWith(
        status: BookingLoadingStatus.loaded,
      );
      notifyListeners();

      return true;
    } catch (e) {
      _bookingState = _bookingState.copyWith(
        status: BookingLoadingStatus.error,
        errorMessage: e.toString(),
      );
      notifyListeners();
      return false;
    }
  }

  Future<void> nextPage(String userId, int selectedPageSize) async {
    if (_bookingState.hasNextPage) {
      await fetchBookingHistory(
        userId: userId,
        page: _bookingState.currentPage + 1,
        limit: selectedPageSize,
        filters: _getActiveFilters()?.filters,
        filterValues: _getActiveFilters()?.values,
        date: _getActiveDate(),
      );
    }
  }

  Future<void> prevPage(String userId, int selectedPageSize) async {
    if (_bookingState.hasPrevPage) {
      await fetchBookingHistory(
        userId: userId,
        page: _bookingState.currentPage - 1,
        limit: selectedPageSize,
        filters: _getActiveFilters()?.filters,
        filterValues: _getActiveFilters()?.values,
        date: _getActiveDate(),
      );
    }
  }

  Future<void> refreshBookings(String userId, {int? pageSize}) async {
    await fetchBookingHistory(
      userId: userId,
      page: 1,
      limit: pageSize ?? _bookingState.pageSize,
      filters: _getActiveFilters()?.filters,
      filterValues: _getActiveFilters()?.values,
      date: _getActiveDate(),
    );
  }

  void clearSelectedBooking() {
    _selectedBooking = null;
    notifyListeners();
  }

  void clearOperationError() {
    _operationError = null;
    notifyListeners();
  }

  _FilterPair? _getActiveFilters() {
    if (_currentFilters != null &&
        _currentFilterValues != null &&
        _currentFilters!.isNotEmpty) {
      return _FilterPair(
        filters: _currentFilters!,
        values: _currentFilterValues!,
      );
    }
    return null;
  }

  String? _getActiveDate() {
    return _currentDate;
  }
}
