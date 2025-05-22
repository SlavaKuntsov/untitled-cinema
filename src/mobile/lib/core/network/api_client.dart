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
          debugPrint(
            'Dio Error: ${error.type} - ${error.response?.statusCode}',
          );

          // Проверяем, что это ошибка авторизации (401)
          if (error.response?.statusCode == 401) {
            debugPrint('Получен 401 Unauthorized, пробуем обновить токен');

            // Получаем оригинальный запрос, который вызвал 401
            final RequestOptions requestOptions = error.requestOptions;

            // Проверяем, не является ли это уже запросом на обновление токена
            if (requestOptions.path.contains(ApiConstants.refreshToken)) {
              debugPrint(
                'Refresh token тоже просрочен, выполняем выход из системы',
              );
              _logout();
              return handler.next(error);
            }

            // Если уже идет процесс обновления токена, добавляем запрос в очередь
            if (_isRefreshing) {
              debugPrint(
                'Уже идет обновление токена, добавляем запрос в очередь',
              );
              final completer = _addToQueue(requestOptions);
              return handler.resolve(await completer.future);
            }

            _isRefreshing = true;
            debugPrint('Начинаем процесс обновления токена');

            try {
              // Пытаемся обновить токен
              final refreshSuccess = await _refreshToken();
              debugPrint(
                'Обновление токена: ${refreshSuccess ? "успешно" : "неудачно"}',
              );

              // Если обновление токена успешно, повторяем запрос
              if (refreshSuccess) {
                debugPrint('Повторяем оригинальный запрос с новым токеном');
                // Повторяем текущий запрос с новым токеном
                final response = await _retry(requestOptions);

                // Обрабатываем очередь отложенных запросов
                _processQueue();

                return handler.resolve(response);
              } else {
                // Если не удалось обновить токен - выходим из системы
                debugPrint(
                  'Не удалось обновить токен, выполняем выход из системы',
                );
                _logout();

                // Очищаем очередь запросов
                _processQueue(success: false);

                return handler.next(error);
              }
            } catch (e) {
              debugPrint('Ошибка при обновлении токена: $e');

              // Выходим из системы при ошибке обновления
              _logout();

              // Очищаем очередь запросов при ошибке
              _processQueue(success: false);

              return handler.next(error);
            } finally {
              _isRefreshing = false;
            }
          }

          // Для других ошибок просто продолжаем обработку
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
    debugPrint(
      'Обработка очереди запросов (${_requestRetryQueue.length}), success=$success',
    );
    for (var item in _requestRetryQueue) {
      if (success) {
        // Повторяем запрос с новым токеном и завершаем completer
        _retry(item.requestOptions)
            .then((response) {
              item.completer.complete(response);
            })
            .catchError((e) {
              debugPrint('Ошибка при повторном запросе: $e');
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
    debugPrint('Повторный запрос: ${requestOptions.path} с новым токеном');

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
        debugPrint('Refresh token отсутствует или пуст');
        return false;
      }

      debugPrint('Начинаем запрос на обновление токена с refresh_token');

      // Создаем новый экземпляр Dio чтобы избежать рекурсивных вызовов интерцептора
      final refreshDio = Dio();
      refreshDio.options.connectTimeout = const Duration(seconds: 15);
      refreshDio.options.receiveTimeout = const Duration(seconds: 15);

      Response<dynamic> response;

      // Пробуем POST запрос вместо GET
      try {
        // Сначала пробуем метод POST, который является более типичным для обновления токена
        debugPrint('Пробуем POST метод для обновления токена');
        response = await refreshDio.get(
          ApiConstants.refreshToken,
          data: {'refreshToken': refreshToken},
          options: Options(
            headers: {
              'Content-Type': 'application/json',
              'Cookie': '${ApiConstants.REFRESH_COOKIE_NAME}=$refreshToken',
            },
          ),
        );
      } catch (postError) {
        // Если POST не сработал, пробуем GET (как в вашем оригинальном коде)
        debugPrint('POST не сработал, пробуем GET: $postError');
        response = await refreshDio.get(
          ApiConstants.refreshToken,
          options: Options(
            followRedirects: true,
            validateStatus: (status) {
              return status != null && status < 500;
            },
            headers: {
              'Cookie': '${ApiConstants.REFRESH_COOKIE_NAME}=$refreshToken',
            },
          ),
        );
      }

      debugPrint('Ответ от сервера: ${response.statusCode}');

      if (response.statusCode == 200) {
        dynamic responseData = response.data;

        // Обработка разных форматов ответа
        String? newAccessToken;

        if (responseData is Map<String, dynamic>) {
          // Проверяем разные возможные ключи для access token
          newAccessToken =
              responseData['accessToken'] as String? ??
              responseData['access_token'] as String? ??
              responseData['token'] as String?;
        }

        if (newAccessToken != null) {
          debugPrint('Получен новый access token');
          await _prefs.setString('access_token', newAccessToken);
        } else {
          debugPrint('Не удалось найти access token в ответе: $responseData');
          return false;
        }

        // Получаем новый refresh token из куки ответа
        if (response.headers.map.containsKey('set-cookie')) {
          final cookies = response.headers.map['set-cookie'];
          debugPrint('Получены куки: $cookies');

          if (cookies != null && cookies.isNotEmpty) {
            bool refreshTokenFound = false;

            for (var cookie in cookies) {
              if (cookie.contains(ApiConstants.REFRESH_COOKIE_NAME)) {
                final refreshTokenValue =
                    cookie.split(';').first.split('=').last;

                if (refreshTokenValue.isNotEmpty) {
                  await _prefs.setString('refresh_token', refreshTokenValue);
                  debugPrint('Сохранен новый refresh token из куки');
                  refreshTokenFound = true;
                  break;
                }
              }
            }

            // Если refresh token не найден в куках, проверяем в теле ответа
            if (!refreshTokenFound && responseData is Map<String, dynamic>) {
              final newRefreshToken =
                  responseData['refreshToken'] as String? ??
                  responseData['refresh_token'] as String?;

              if (newRefreshToken != null) {
                await _prefs.setString('refresh_token', newRefreshToken);
                debugPrint('Сохранен новый refresh token из тела ответа');
              }
            }
          }
        }

        return true;
      }

      debugPrint('Неуспешный статус ответа: ${response.statusCode}');
      return false;
    } catch (e) {
      debugPrint('Исключение при обновлении токена: $e');
      return false;
    }
  }

  // Метод для выхода из системы при неудачном обновлении токена
  void _logout() {
    debugPrint('Выполняется _logout()');
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
      return _handleDioError(e);
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  Future<dynamic> post(String url, {dynamic data}) async {
    try {
      final response = await _dio.post(url, data: data);
      return _processResponse(response);
    } on DioException catch (e) {
      return _handleDioError(e);
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  Future<dynamic> put(String url, {dynamic data}) async {
    try {
      final response = await _dio.put(url, data: data);
      return _processResponse(response);
    } on DioException catch (e) {
      return _handleDioError(e);
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  Future<dynamic> patch(String url, {dynamic data}) async {
    try {
      final response = await _dio.patch(url, data: data);
      return _processResponse(response);
    } on DioException catch (e) {
      return _handleDioError(e);
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  Future<dynamic> delete(String url, {dynamic data}) async {
    try {
      final response = await _dio.delete(url, data: data);
      return _processResponse(response);
    } on DioException catch (e) {
      return _handleDioError(e);
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  dynamic _processResponse(Response response) {
    switch (response.statusCode) {
      case 200:
      case 201:
        return response.data;
      case 204:
        // Handle 204 No Content as success (common for DELETE operations)
        return null;
      case 400:
        throw BadRequestException(response.data.toString());
      case 401:
      case 403:
        // Не выбрасываем исключение здесь, потому что это уже обрабатывается в интерцепторе
        // Если мы дошли до этой точки, значит интерцептор не смог восстановить запрос
        debugPrint(
          '_processResponse получил код ${response.statusCode} после обработки интерцептором',
        );
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

  dynamic _handleDioError(DioException error) {
    debugPrint(
      'Обработка Dio ошибки: ${error.type}, код: ${error.response?.statusCode}',
    );

    if (error.error is SocketException) {
      throw const NoInternetException('No internet connection');
    } else if (error.type == DioExceptionType.connectionTimeout ||
        error.type == DioExceptionType.receiveTimeout) {
      throw TimeoutException('Connection timeout');
    } else if (error.response != null) {
      if (error.response!.statusCode == 401 ||
          error.response!.statusCode == 403) {
        // Не делаем ничего для 401/403, так как интерцептор уже попытался обработать это
        // и если мы попали сюда, значит обновление токена не помогло
        debugPrint('_handleDioError: 401/403 после обработки интерцептором');
        throw UnauthorizedException(
          error.response?.data.toString() ?? 'Unauthorized',
        );
      }
      return _processResponse(error.response!);
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
