import 'dart:async';
import 'dart:io';

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../constants/api_constants.dart';
import '../errors/exceptions.dart';

class ApiClient {
  final Dio _dio;
  final SharedPreferences _prefs;

  // Флаг для предотвращения множественных запросов на обновление токена
  bool _isRefreshing = false;

  // Очередь запросов ожидающих обновления токена
  final List<_RequestRetryQueue> _requestRetryQueue = [];

  // Колбэк для уведомления о необходимости выхода из системы
  final Function? onLogoutRequired;

  ApiClient(this._dio, this._prefs, {this.onLogoutRequired}) {
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
            // Получаем оригинальный запрос, который вызвал 401
            final RequestOptions requestOptions = error.requestOptions;

            // Если уже идет процесс обновления токена, добавляем запрос в очередь
            if (_isRefreshing) {
              final completer = _addToQueue(requestOptions);
              return handler.resolve(await completer.future);
            }

            _isRefreshing = true;

            try {
              // Пытаемся обновить токен
              final refreshSuccess = await _refreshToken();

              // Если обновление токена успешно, повторяем запрос
              if (refreshSuccess) {
                // Повторяем текущий запрос с новым токеном
                final response = await _retry(requestOptions);

                // Обрабатываем очередь отложенных запросов
                _processQueue();

                return handler.resolve(response);
              } else {
                // Если не удалось обновить токен - выходим из системы
                _logout();

                // Очищаем очередь запросов
                _processQueue(success: false);

                return handler.next(error);
              }
            } catch (e) {
              debugPrint('Ошибка при обновлении токена: $e');

              // Очищаем очередь запросов при ошибке
              _processQueue(success: false);

              return handler.next(error);
            } finally {
              _isRefreshing = false;
            }
          }
          return handler.next(error);
        },
      ),
    );
  }

  // Добавляет запрос в очередь и возвращает Completer для будущего результата
  Completer<Response> _addToQueue(RequestOptions requestOptions) {
    final completer = Completer<Response>();
    _requestRetryQueue.add(_RequestRetryQueue(requestOptions, completer));
    return completer;
  }

  // Обрабатывает очередь отложенных запросов
  void _processQueue({bool success = true}) {
    for (var item in _requestRetryQueue) {
      if (success) {
        // Повторяем запрос с новым токеном и завершаем completer
        _retry(item.requestOptions)
            .then((response) {
              item.completer.complete(response);
            })
            .catchError((e) {
              item.completer.completeError(e);
            });
      } else {
        // При ошибке обновления токена отклоняем все запросы
        final error = DioException(
          requestOptions: item.requestOptions,
          error: 'Token refresh failed',
          type: DioExceptionType.unknown,
        );
        item.completer.completeError(error);
      }
    }
    // Очищаем очередь
    _requestRetryQueue.clear();
  }

  Future<Response<dynamic>> _retry(RequestOptions requestOptions) async {
    final token = _prefs.getString('access_token');

    final options = Options(
      method: requestOptions.method,
      headers: {
        ...requestOptions.headers,
        // Обновляем токен в заголовке
        if (token != null && token.isNotEmpty)
          ApiConstants.authorization: '${ApiConstants.bearer} $token',
      },
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
          'Cookie': '${ApiConstants.REFRESH_COOKIE_NAME}=$refreshToken',
        },
      );

      // Создаем новый экземпляр Dio чтобы избежать рекурсивных вызовов интерцептора
      final refreshDio = Dio();
      refreshDio.options.connectTimeout = const Duration(seconds: 15);
      refreshDio.options.receiveTimeout = const Duration(seconds: 15);

      // Отправляем GET запрос на refresh-token эндпоинт
      final response = await refreshDio.get<Map<String, dynamic>>(
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
              if (cookie.contains(ApiConstants.REFRESH_COOKIE_NAME)) {
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
      debugPrint('Ошибка обновления токена: $e');
      return false;
    }
  }

  // Метод для выхода из системы при неудачном обновлении токена
  void _logout() {
    _prefs.remove('access_token');
    _prefs.remove('refresh_token');

    // Вызываем колбэк если он передан
    if (onLogoutRequired != null) {
      onLogoutRequired!();
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

// Класс для хранения запросов в очереди
class _RequestRetryQueue {
  final RequestOptions requestOptions;
  final Completer<Response> completer;

  _RequestRetryQueue(this.requestOptions, this.completer);
}
