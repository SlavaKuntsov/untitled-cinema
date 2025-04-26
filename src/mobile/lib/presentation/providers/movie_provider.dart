// presentation/providers/movie_provider.dart
import 'package:flutter/material.dart';

import '../../domain/entities/movie/movie.dart';
import '../../domain/repositories/movies_repository.dart';

// Различные состояния для UI
enum MovieStatus { initial, loading, loaded, error }

// Состояние провайдера фильмов
class MoviesState {
  final List<Movie> movies;
  final int total;
  final MovieStatus status;
  final String? errorMessage;
  final int currentPage;
  final int pageSize;
  final String nextRef;
  final String prevRef;

  bool get hasNextPage => (currentPage) * pageSize < total;
  bool get hasPrevPage => currentPage > 1;

  MoviesState({
    this.movies = const [],
    this.total = 0,
    this.status = MovieStatus.initial,
    this.errorMessage,
    this.currentPage = 1,
    this.pageSize = 10,
    this.nextRef = '',
    this.prevRef = '',
  });

  // Копирование с новыми значениями
  MoviesState copyWith({
    List<Movie>? movies,
    int? total,
    MovieStatus? status,
    String? errorMessage,
    int? currentPage,
    int? pageSize,
    String? nextRef,
    String? prevRef,
  }) {
    return MoviesState(
      movies: movies ?? this.movies,
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

// Provider для управления состоянием фильмов
class MovieProvider extends ChangeNotifier {
  final MovieRepository _repository;

  // Изменено: используем поле с именем moviesState вместо state
  MoviesState _moviesState = MoviesState();
  MoviesState get moviesState => _moviesState;

  MovieProvider({required MovieRepository repository})
    : _repository = repository;

  Future<void> fetchMovies({
    int? page,
    int? limit,
    String? sortBy,
    String? sortDirection,
    DateTime? date,
    List<String>? filters,
    List<String>? filterValues,
  }) async {
    try {
      // Устанавливаем состояние загрузки
      _moviesState = _moviesState.copyWith(
        status: MovieStatus.loading,
        errorMessage: null,
      );
      notifyListeners();

      final pageSize = limit ?? _moviesState.pageSize;
      final currentPage = page ?? _moviesState.currentPage;
      // final offset = currentPage * pageSize;
      final offset = currentPage ?? _moviesState.currentPage;

      // Выполняем API запрос через repository
      final result = await _repository.getMovies(
        limit: pageSize,
        offset: offset,
        sortBy: sortBy,
        sortDirection: sortDirection,
        date: date,
        filters: filters,
        filterValues: filterValues,
      );

      // Обновляем состояние
      _moviesState = _moviesState.copyWith(
        movies: result.items,
        total: result.total,
        status: MovieStatus.loaded,
        currentPage: currentPage,
        pageSize: pageSize,
        nextRef: result.nextRef,
        prevRef: result.prevRef,
      );
      notifyListeners();
    } catch (e) {
      _moviesState = _moviesState.copyWith(
        status: MovieStatus.error,
        errorMessage: e.toString(),
      );
      notifyListeners();
    }
  }

  // Переход на следующую страницу
  Future<void> nextPage() async {
    if (_moviesState.hasNextPage) {
      await fetchMovies(page: _moviesState.currentPage + 1);
    }
  }

  // Переход на предыдущую страницу
  Future<void> prevPage() async {
    if (_moviesState.hasPrevPage) {
      await fetchMovies(page: _moviesState.currentPage - 1);
    }
  }

  // Добавлено: метод для обновления списка фильмов
  Future<void> refreshMovies() async {
    await fetchMovies(page: 0);
  }
}
