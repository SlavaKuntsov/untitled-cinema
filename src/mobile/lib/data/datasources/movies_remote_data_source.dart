import 'dart:convert';

import 'package:untitledCinema/data/models/movie/movie_model.dart';

import '../../core/constants/api_constants.dart';
import '../../core/errors/exceptions.dart';
import '../../core/network/api_client.dart';
import '../models/movie/paginated_movies_model.dart';

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
      // offset++;
      // Формируем URL с параметрами запроса
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
      if (date != null) queryParams['date'] = date.toIso8601String();

      // Добавляем фильтры, если они есть
      if (filters != null && filterValues != null) {
        for (var i = 0; i < filters.length; i++) {
          if (i < filterValues.length) {
            queryParams['filters[$i]'] = filters[i];
            queryParams['filterValues[$i]'] = filterValues[i];
          }
        }
      }

      // Выполняем запрос к API
      final dynamic responseData = await client.get(
        ApiConstants.movies,
        queryParameters: queryParams,
      );

      // Проверяем, что responseData не null и является Map
      if (responseData != null && responseData is Map<String, dynamic>) {
        // Заменяем localhost на 192.168.0.101 в постерах
        // _replaceLocalhostInPosters(responseData);

        return PaginatedResponseModel<MovieModel>.fromJson(
          responseData,
          (json) => MovieModel.fromJson(json),
        );
      } else {
        throw ServerException('Некорректный формат данных от сервера');
      }
    } catch (e) {
      if (e is ServerException) {
        throw e; // Пробрасываем специальные исключения
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
}
