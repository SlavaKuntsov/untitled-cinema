import 'dart:convert';

import 'package:intl/intl.dart';
import 'package:untitledCinema/data/models/movie/movie_model.dart';

import '../../core/constants/api_constants.dart';
import '../../core/errors/exceptions.dart';
import '../../core/network/api_client.dart';
import '../models/paginated_response.dart';

abstract class MovieRemoteDataSource {
  Future<PaginatedResponseModel<MovieModel>> getMovies({
    int limit = 10,
    int offset = 0,
    String? sortBy,
    String? sortDirection,
    DateTime? date,
    List<String>? filters,
    List<String>? filterValues,
  });

  Future<MovieModel> getMovieById(String id);

  Future<String> getMoviePosterUrl(String id);

  Future<List<String>> getMovieFrames(String id);

  Future<List<String>> getMovieGenres();
}

class MovieRemoteDataSourceImpl implements MovieRemoteDataSource {
  final ApiClient client;

  MovieRemoteDataSourceImpl({required this.client});

  @override
  Future<PaginatedResponseModel<MovieModel>> getMovies({
    int limit = 10,
    int offset = 1,
    String? sortBy = 'title',
    String? sortDirection = 'asc',
    DateTime? date,
    List<String>? filters,
    List<String>? filterValues,
  }) async {
    try {
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
      if (date != null)
        queryParams['date'] = DateFormat('dd-MM-yyyy').format(date);
      if (date == null) {
        queryParams['date'] = DateFormat('dd-MM-yyyy').format(DateTime.now());
      }

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

      final dynamic responseData = await client.get(
        ApiConstants.movies,
        queryParameters: queryParams,
      );

      if (responseData != null && responseData is Map<String, dynamic>) {
        return PaginatedResponseModel<MovieModel>.fromJson(
          responseData,
          (json) => MovieModel.fromJson(json),
        );
      } else {
        throw ServerException('Некорректный формат данных от сервера');
      }
    } catch (e) {
      if (e is ServerException) {
        throw e;
      }
      throw ServerException('Ошибка при получении фильмов: ${e.toString()}');
    }
  }

  @override
  Future<MovieModel> getMovieById(String id) async {
    final response = await client.get('${ApiConstants.movies}/$id');

    if (response != null) {
      return MovieModel.fromJson(response);
    } else {
      throw Exception('Failed to load movie: ${response.statusCode}');
    }
  }

  @override
  Future<String> getMoviePosterUrl(String id) async {
    final response = await client.get('${ApiConstants.movies}/$id/poster');

    if (response.statusCode == 200) {
      final data = json.decode(response.body);
      return data['url'] ?? '';
    } else {
      throw Exception('Failed to load movie poster: ${response.statusCode}');
    }
  }

  @override
  Future<List<String>> getMovieFrames(String id) async {
    try {
      final response = await client.get('${ApiConstants.movies}/$id/frames');

      if (response != null) {
        // Предполагаем, что response - это список объектов JSON
        final List<dynamic> framesData = response as List<dynamic>;

        // Извлекаем только поле frameName из каждого объекта
        final List<String> frameNames =
            framesData.map((frame) => frame['frameName'] as String).toList();

        return frameNames;
      } else {
        return [];
      }
    } catch (e) {
      print('Ошибка при получении кадров: ${e.toString()}');
      return [];
    }
  }

  @override
  Future<List<String>> getMovieGenres() async {
    try {
      final response = await client.get(ApiConstants.movieGenres);

      if (response != null) {
        // Convert dynamic list to string list
        final List<dynamic> genresData = response as List<dynamic>;

        // Check if each item is a Map with a 'name' field
        if (genresData.isNotEmpty && genresData.first is Map<String, dynamic>) {
          final List<String> genres =
              genresData
                  .map(
                    (genre) =>
                        (genre as Map<String, dynamic>)['name']?.toString() ??
                        "",
                  )
                  .where((name) => name.isNotEmpty)
                  .toList();
          return genres;
        }

        // Fallback to direct toString if not a Map structure
        final List<String> genres =
            genresData.map((genre) => genre.toString()).toList();
        return genres;
      } else {
        return [];
      }
    } catch (e) {
      print('Ошибка при получении жанров: ${e.toString()}');
      return [];
    }
  }
}
