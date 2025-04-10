import 'dart:io';

import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../constants/api_constants.dart';
import '../errors/exceptions.dart';

class ApiClient {
  final Dio _dio;
  final SharedPreferences _prefs;

  ApiClient(this._dio, this._prefs) {
    // Настройка Dio без базового URL, т.к. у нас разные базовые URL
    _dio.options.connectTimeout = const Duration(seconds: 15);
    _dio.options.receiveTimeout = const Duration(seconds: 15);
    _dio.options.headers = {
      'Content-Type': ApiConstants.contentType,
      'Accept': ApiConstants.contentType,
    };

    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          // Добавление токена в заголовок, если он существует
          final token = _prefs.getString('access_token');
          if (token != null && token.isNotEmpty) {
            options.headers[ApiConstants.authorization] =
                '${ApiConstants.bearer} $token';
          }
          return handler.next(options);
        },
        onError: (DioException error, handler) async {
          // Обработка ошибки 401 - Обновление токена
          if (error.response?.statusCode == 401) {
            if (await _refreshToken()) {
              // Повторный запрос с новым токеном
              return handler.resolve(await _retry(error.requestOptions));
            }
          }
          return handler.next(error);
        },
      ),
    );
  }

  Future<Response<dynamic>> _retry(RequestOptions requestOptions) async {
    final options = Options(
      method: requestOptions.method,
      headers: requestOptions.headers,
    );
    return _dio.request<dynamic>(
      requestOptions.path,
      data: requestOptions.data,
      queryParameters: requestOptions.queryParameters,
      options: options,
    );
  }

  Future<bool> _refreshToken() async {
    try {
      final refreshToken = _prefs.getString('refresh_token');
      if (refreshToken == null || refreshToken.isEmpty) {
        return false;
      }

      // Создаем особые параметры для Dio, которые сохраняют куки
      final dioOptions = Options(
        followRedirects: true,
        validateStatus: (status) {
          return status != null && status < 500;
        },
        headers: {
          'Cookie': '${JwtConstants.REFRESH_COOKIE_NAME}=$refreshToken',
        },
      );

      // Отправляем GET запрос на refresh-token эндпоинт
      final response = await Dio().get<Map<String, dynamic>>(
        ApiConstants.refreshToken,
        options: dioOptions,
      );

      if (response.statusCode == 200 && response.data != null) {
        // Получаем новый access token из ответа
        await _prefs.setString('access_token', response.data!['accessToken']);

        // Получаем новый refresh token из куки ответа
        if (response.headers.map.containsKey('set-cookie')) {
          final cookies = response.headers.map['set-cookie'];
          if (cookies != null && cookies.isNotEmpty) {
            for (var cookie in cookies) {
              if (cookie.contains(JwtConstants.REFRESH_COOKIE_NAME)) {
                final refreshTokenValue =
                    cookie.split(';').first.split('=').last;
                await _prefs.setString('refresh_token', refreshTokenValue);
                break;
              }
            }
          }
        }
        return true;
      }
      return false;
    } catch (e) {
      return false;
    }
  }

  Future<dynamic> get(
    String url, {
    Map<String, dynamic>? queryParameters,
  }) async {
    try {
      final response = await _dio.get(url, queryParameters: queryParameters);
      return _processResponse(response);
    } on DioException catch (e) {
      _handleDioError(e);
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  Future<dynamic> post(String url, {dynamic data}) async {
    try {
      final response = await _dio.post(url, data: data);
      return _processResponse(response);
    } on DioException catch (e) {
      _handleDioError(e);
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  Future<dynamic> put(String url, {dynamic data}) async {
    try {
      final response = await _dio.put(url, data: data);
      return _processResponse(response);
    } on DioException catch (e) {
      _handleDioError(e);
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  Future<dynamic> delete(String url, {dynamic data}) async {
    try {
      final response = await _dio.delete(url, data: data);
      return _processResponse(response);
    } on DioException catch (e) {
      _handleDioError(e);
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  dynamic _processResponse(Response response) {
    switch (response.statusCode) {
      case 200:
      case 201:
        return response.data;
      case 400:
        throw BadRequestException(response.data.toString());
      case 401:
      case 403:
        throw UnauthorizedException(response.data.toString());
      case 404:
        throw NotFoundException(response.data.toString());
      case 500:
      default:
        throw ServerException(
          'Error occurred with status code: ${response.statusCode}',
        );
    }
  }

  void _handleDioError(DioException error) {
    if (error.error is SocketException) {
      throw const NoInternetException('No internet connection');
    } else if (error.type == DioExceptionType.connectionTimeout ||
        error.type == DioExceptionType.receiveTimeout) {
      throw TimeoutException('Connection timeout');
    } else if (error.response != null) {
      _processResponse(error.response!);
    } else {
      throw ServerException(error.message ?? 'Unknown error occurred');
    }
  }
}
