import 'dart:convert';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:intl/intl.dart';

import '../../core/constants/api_constants.dart';
import '../../core/network/api_client.dart';
import '../../domain/entities/movie/genre.dart';
import '../../domain/entities/movie/movie.dart';
import '../../domain/entities/movie/movie_frame.dart';

class MovieManagementProvider extends ChangeNotifier {
  final ApiClient _apiClient;

  MovieManagementProvider({required ApiClient apiClient})
    : _apiClient = apiClient;

  List<Movie> _movies = [];
  List<MovieFrame> _frames = [];
  List<Genre> _genres = [];
  Movie? _selectedMovie;
  bool _isLoading = false;
  String? _errorMessage;

  List<Movie> get movies => _movies;
  List<MovieFrame> get frames => _frames;
  List<Genre> get genres => _genres;
  Movie? get selectedMovie => _selectedMovie;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  // ==================== MOVIES ====================

  Future<void> fetchAllMovies({
    int limit = 20,
    int offset = 1,
    List<String>? filters,
    List<String>? filterValues,
    String? sortBy = 'title',
    String? sortDirection = 'asc',
    DateTime? date,
  }) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final queryParams = <String, String>{
        'limit': limit.toString(),
        'offset': offset.toString(),
      };

      if (sortBy != null)
        queryParams['sortBy'] = sortBy;
      else
        queryParams['sortBy'] = 'title';
      if (sortDirection != null)
        queryParams['sortDirection'] = sortDirection;
      else
        queryParams['sortDirection'] = 'asc';
      // if (date != null)
      //   queryParams['date'] = DateFormat('dd-MM-yyyy').format(date);
      // if (date == null) {
      // queryParams['date'] = DateFormat('dd-MM-yyyy').format(DateTime.now());
      // }

      // Добавляем фильтры, если они есть
      if (filters != null &&
          filterValues != null &&
          filters.length > 0 &&
          filters.length == filterValues.length) {
        final Map<String, List<String>> multiParams = {};

        for (var i = 0; i < filters.length; i++) {
          if (!multiParams.containsKey('Filter')) {
            multiParams['Filter'] = [];
          }
          multiParams['Filter']!.add(filters[i]);

          if (!multiParams.containsKey('FilterValue')) {
            multiParams['FilterValue'] = [];
          }
          multiParams['FilterValue']!.add(filterValues[i]);
        }

        multiParams.forEach((key, values) {
          for (var i = 0; i < values.length; i++) {
            queryParams['$key'] = values[i];
          }
        });
      }

      final response = await _apiClient.get(
        ApiConstants.movies,
        queryParameters: queryParams,
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

      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<Movie?> fetchMovieById(String id) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final response = await _apiClient.get(ApiConstants.getMovieById(id));

      if (response != null && response is Map<String, dynamic>) {
        final movie = Movie.fromJson(response);
        _selectedMovie = movie;

        _isLoading = false;
        notifyListeners();

        return movie;
      }

      _isLoading = false;
      notifyListeners();
      return null;
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<Movie?> createMovie({
    required String title,
    required String description,
    required List<String> genres,
    required double price,
    required int durationMinutes,
    required String producer,
    required String inRoles,
    required int ageLimit,
    required DateTime releaseDate,
  }) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final payload = {
        'title': title,
        'description': description,
        'genres': genres,
        'price': price,
        'durationMinutes': durationMinutes,
        'producer': producer,
        'inRoles': inRoles,
        'ageLimit': ageLimit,
        'releaseDate': DateFormat('dd-MM-yyyy hh:mm').format(releaseDate),
      };

      final response = await _apiClient.post(
        ApiConstants.createMovie(),
        data: payload,
      );

      if (response != null && response is Map<String, dynamic>) {
        final newMovie = Movie.fromJson(response);
        _movies.add(newMovie);

        _isLoading = false;
        notifyListeners();

        return newMovie;
      }

      _isLoading = false;
      notifyListeners();
      return null;
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<Movie?> updateMovie({
    required String id,
    String? title,
    String? description,
    List<String>? genres,
    double? price,
    int? durationMinutes,
    String? producer,
    String? inRoles,
    int? ageLimit,
    DateTime? releaseDate,
  }) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final Map<String, dynamic> payload = {'id': id};

      if (title != null) payload['title'] = title;
      if (description != null) payload['description'] = description;
      if (genres != null) payload['genres'] = genres;
      if (price != null) payload['price'] = price.toString();
      if (durationMinutes != null)
        payload['durationMinutes'] = durationMinutes.toString();
      if (producer != null) payload['producer'] = producer;
      if (inRoles != null) payload['inRoles'] = inRoles;
      if (ageLimit != null) payload['ageLimit'] = ageLimit.toString();
      if (releaseDate != null)
        payload['releaseDate'] = releaseDate.toIso8601String();

      final response = await _apiClient.patch(
        ApiConstants.updateMovie(),
        data: payload,
      );

      if (response != null && response is Map<String, dynamic>) {
        final updatedMovie = Movie.fromJson(response);
        final index = _movies.indexWhere((movie) => movie.id == id);
        if (index != -1) {
          _movies[index] = updatedMovie;
        }

        if (_selectedMovie?.id == id) {
          _selectedMovie = updatedMovie;
        }

        _isLoading = false;
        notifyListeners();

        return updatedMovie;
      }

      _isLoading = false;
      notifyListeners();
      return null;
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<void> deleteMovie(String id) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      await _apiClient.delete(ApiConstants.deleteMovie(id));

      _movies.removeWhere((movie) => movie.id == id);
      if (_selectedMovie?.id == id) {
        _selectedMovie = null;
      }

      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<String?> uploadMoviePoster(String movieId, File posterFile) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final url = ApiConstants.changeMoviePoster(movieId);

      // Create multipart request
      final request = http.MultipartRequest('PATCH', Uri.parse(url));

      // Add authorization headers if needed
      // request.headers['Authorization'] = 'Bearer $token';

      // Add file to request
      final fileStream = http.ByteStream(posterFile.openRead());
      final fileLength = await posterFile.length();

      final multipartFile = http.MultipartFile(
        'file',
        fileStream,
        fileLength,
        filename: posterFile.path.split('/').last,
      );

      request.files.add(multipartFile);

      // Send request
      final streamedResponse = await request.send();
      final response = await http.Response.fromStream(streamedResponse);

      if (response.statusCode >= 200 && response.statusCode < 300) {
        String posterUrl;

        // Try to parse as JSON, if it fails, use response body directly as filename
        try {
          final responseData = json.decode(response.body);
          posterUrl = responseData['posterUrl'] ?? response.body;
        } catch (e) {
          // If parsing fails, the response is likely just a filename
          posterUrl = response.body.trim();
        }

        // Update movie in list with new poster URL
        final index = _movies.indexWhere((movie) => movie.id == movieId);
        if (index != -1) {
          _movies[index] = _movies[index].copyWith(poster: posterUrl);
        }

        if (_selectedMovie?.id == movieId) {
          _selectedMovie = _selectedMovie!.copyWith(poster: posterUrl);
        }

        _isLoading = false;
        notifyListeners();

        return posterUrl;
      }

      _isLoading = false;
      notifyListeners();
      return null;
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  // ==================== FRAMES ====================

  Future<void> fetchAllFrames() async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final response = await _apiClient.get(ApiConstants.getAllMovieFrames());

      if (response != null) {
        final List<MovieFrame> loadedFrames = [];

        if (response is List) {
          for (var item in response) {
            if (item is Map<String, dynamic>) {
              loadedFrames.add(MovieFrame.fromJson(item));
            }
          }
        }

        _frames = loadedFrames;
      } else {
        _frames = [];
      }

      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<List<MovieFrame>> fetchFramesByMovieId(String movieId) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final response = await _apiClient.get(
        ApiConstants.getMovieFramesByMovieId(movieId),
      );

      if (response != null) {
        final List<MovieFrame> loadedFrames = [];

        if (response is List) {
          for (var item in response) {
            if (item is Map<String, dynamic>) {
              loadedFrames.add(MovieFrame.fromJson(item));
            }
          }
        }

        _frames = loadedFrames;
        _isLoading = false;
        notifyListeners();
        return loadedFrames;
      }

      _isLoading = false;
      notifyListeners();
      return [];
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<MovieFrame?> addMovieFrame(
    String movieId,
    File frameFile, {
    int frameOrder = -1,
  }) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final url = ApiConstants.addMovieFrame(movieId, frameOrder);

      // Create multipart request
      final request = http.MultipartRequest('POST', Uri.parse(url));

      // Add authorization headers if needed
      // request.headers['Authorization'] = 'Bearer $token';

      // Add file to request
      final fileStream = http.ByteStream(frameFile.openRead());
      final fileLength = await frameFile.length();

      final multipartFile = http.MultipartFile(
        'file',
        fileStream,
        fileLength,
        filename: frameFile.path.split('/').last,
      );

      request.files.add(multipartFile);

      // Send request
      final streamedResponse = await request.send();
      final response = await http.Response.fromStream(streamedResponse);

      if (response.statusCode >= 200 && response.statusCode < 300) {
        MovieFrame newFrame;

        try {
          final responseData = json.decode(response.body);
          newFrame = MovieFrame.fromJson(responseData);
        } catch (e) {
          // If parsing fails, create a frame with the response body as filename
          final filename = response.body.trim();
          // Generate a random ID since we don't have one from the server
          final randomId = DateTime.now().millisecondsSinceEpoch.toString();
          newFrame = MovieFrame(
            id: randomId,
            movieId: movieId,
            frameName: filename,
            order: frameOrder >= 0 ? frameOrder : _frames.length,
          );
        }

        _frames.add(newFrame);
        _isLoading = false;
        notifyListeners();
        return newFrame;
      }

      _isLoading = false;
      notifyListeners();
      return null;
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<void> deleteMovieFrame(String frameId) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      await _apiClient.delete(ApiConstants.deleteMovieFrame(frameId));

      _frames.removeWhere((frame) => frame.id == frameId);

      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  // ==================== GENRES ====================

  Future<void> fetchAllGenres() async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final response = await _apiClient.get(ApiConstants.getAllGenres());

      if (response != null) {
        final List<Genre> loadedGenres = [];

        if (response is List) {
          for (var item in response) {
            if (item is Map<String, dynamic>) {
              loadedGenres.add(Genre.fromJson(item));
            }
          }
        }

        _genres = loadedGenres;
      } else {
        _genres = [];
      }

      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<Genre?> updateGenre({required String id, required String name}) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final payload = {'id': id, 'name': name};

      final response = await _apiClient.patch(
        ApiConstants.updateGenre(),
        data: payload,
      );

      if (response != null && response is Map<String, dynamic>) {
        final updatedGenre = Genre.fromJson(response);
        final index = _genres.indexWhere((genre) => genre.id == id);
        if (index != -1) {
          _genres[index] = updatedGenre;
        }

        _isLoading = false;
        notifyListeners();

        return updatedGenre;
      }

      _isLoading = false;
      notifyListeners();
      return null;
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<void> deleteGenre(String id) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      await _apiClient.delete(ApiConstants.deleteGenre(id));

      _genres.removeWhere((genre) => genre.id == id);

      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }
}
