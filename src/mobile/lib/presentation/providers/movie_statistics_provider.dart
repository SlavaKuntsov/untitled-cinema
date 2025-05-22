import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'dart:convert';
import 'package:http/http.dart' as http;

import '../../core/constants/api_constants.dart';
import '../../domain/entities/movie/movie.dart';
import '../../core/network/api_client.dart';

class MovieStatisticsProvider extends ChangeNotifier {
  final ApiClient _apiClient;
  
  bool _isLoading = false;
  String? _errorMessage;
  List<Movie> _movies = [];
  Map<String, int> _movieViewCounts = {};
  Map<String, double> _movieRevenue = {};
  DateTime _startDate = DateTime.now().subtract(const Duration(days: 30));
  DateTime _endDate = DateTime.now();

  MovieStatisticsProvider({required ApiClient apiClient}) : _apiClient = apiClient;

  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;
  List<Movie> get movies => _movies;
  Map<String, int> get movieViewCounts => _movieViewCounts;
  Map<String, double> get movieRevenue => _movieRevenue;
  DateTime get startDate => _startDate;
  DateTime get endDate => _endDate;

  Future<void> fetchMovieStatistics() async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      // Fetch all movies first
      await _fetchAllMovies();
      
      // Then fetch statistics for each movie
      await _fetchMovieViews();
      await _fetchMovieRevenue();
      
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _errorMessage = 'Error: ${e.toString()}';
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> _fetchAllMovies() async {
    try {
      final response = await _apiClient.get(
        ApiConstants.movies,
        queryParameters: {
          'limit': '100',
          'offset': '1',
          'sortBy': 'title',
          'sortDirection': 'asc',
        },
      );

      if (response != null && response is Map<String, dynamic>) {
        final List<Movie> loadedMovies = [];
        final items = response['items'] as List;

        for (var item in items) {
          if (item is Map<String, dynamic>) {
            loadedMovies.add(Movie.fromJson(item));
          }
        }

        _movies = loadedMovies;
      } else {
        _movies = [];
      }
    } catch (e) {
      throw Exception('Failed to load movies: ${e.toString()}');
    }
  }

  Future<void> _fetchMovieViews() async {
    try {
      // This would ideally be a real API call to get movie views
      // For now, we'll simulate some data based on the movies we have
      _movieViewCounts = {};
      
      // Simulate view counts for each movie (random data for demonstration)
      for (var movie in _movies) {
        // Generate a pseudo-random view count based on movie ID
        final viewCount = (movie.id.codeUnits.reduce((a, b) => a + b) % 1000) + 50;
        _movieViewCounts[movie.id] = viewCount;
      }
    } catch (e) {
      throw Exception('Failed to load movie views: ${e.toString()}');
    }
  }

  Future<void> _fetchMovieRevenue() async {
    try {
      // This would ideally be a real API call to get movie revenue
      // For now, we'll simulate some data based on the movies we have
      _movieRevenue = {};
      
      // Simulate revenue for each movie
      for (var movie in _movies) {
        // Calculate a pseudo-random revenue based on movie price and ID
        final avgTickets = (_movieViewCounts[movie.id] ?? 100) / 2;
        final revenue = movie.price * avgTickets;
        _movieRevenue[movie.id] = revenue;
      }
    } catch (e) {
      throw Exception('Failed to load movie revenue: ${e.toString()}');
    }
  }

  void setDateRange(DateTime start, DateTime end) {
    _startDate = start;
    _endDate = end;
    // Refetch data with new date range
    fetchMovieStatistics();
  }

  // Get top movies by view count
  List<Movie> get topMoviesByViews {
    final sortedMovies = List<Movie>.from(_movies);
    sortedMovies.sort((a, b) => 
      (_movieViewCounts[b.id] ?? 0).compareTo(_movieViewCounts[a.id] ?? 0)
    );
    return sortedMovies;
  }

  // Get top movies by revenue
  List<Movie> get topMoviesByRevenue {
    final sortedMovies = List<Movie>.from(_movies);
    sortedMovies.sort((a, b) => 
      (_movieRevenue[b.id] ?? 0).compareTo(_movieRevenue[a.id] ?? 0)
    );
    return sortedMovies;
  }

  // Get view count data for chart
  List<MovieChartData> get viewCountChartData {
    final topMovies = topMoviesByViews.take(10).toList();
    return topMovies.map((movie) => 
      MovieChartData(
        movie: movie,
        value: _movieViewCounts[movie.id] ?? 0,
      )
    ).toList();
  }

  // Get revenue data for chart
  List<MovieChartData> get revenueChartData {
    final topMovies = topMoviesByRevenue.take(10).toList();
    return topMovies.map((movie) => 
      MovieChartData.fromDouble(
        movie: movie,
        value: _movieRevenue[movie.id] ?? 0,
      )
    ).toList();
  }
}

class MovieChartData {
  final Movie movie;
  final double value;

  MovieChartData({
    required this.movie,
    required int value,
  }) : value = value.toDouble();

  MovieChartData.fromDouble({
    required this.movie,
    required this.value,
  });
} 