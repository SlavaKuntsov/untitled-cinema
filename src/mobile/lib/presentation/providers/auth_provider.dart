import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../core/constants/api_constants.dart';
import '../../core/network/api_client.dart';
import '../../data/datasources/auth/google_auth_service.dart';
import '../../di/injection_container.dart';
import '../../domain/entities/auth/user.dart';
import '../../domain/usecases/auth/google_sign_in.dart';
import '../../domain/usecases/auth/login.dart';
import '../../domain/usecases/auth/registration.dart';

enum AuthStatus { unknown, authenticated, unauthenticated }

class AuthProvider extends ChangeNotifier {
  final LoginUseCase _loginUseCase;
  final RegistrationUseCase _registrationUseCase;
  final GoogleSignInUseCase _googleSignInUseCase;
  final GoogleSignIn _googleSignIn;
  final SharedPreferences _prefs;

  AuthStatus _authStatus = AuthStatus.unknown;
  User? _currentUser;
  String? _savedEmail;
  String? _errorMessage;
  bool _isLoading = false;
  String? _token;

  AuthProvider({
    required LoginUseCase loginUseCase,
    required RegistrationUseCase registrationUseCase,
    required GoogleSignInUseCase googleSignInUseCase,
    required GoogleSignIn googleSignIn,
    required SharedPreferences prefs,
  }) : _loginUseCase = loginUseCase,
       _registrationUseCase = registrationUseCase,
       _googleSignInUseCase = googleSignInUseCase,
       _googleSignIn = googleSignIn,
       _prefs = prefs;

  AuthStatus get authStatus => _authStatus;
  User? get currentUser => _currentUser;
  String? get savedEmail => _savedEmail;
  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;
  String? get token => _prefs.getString('access_token');

  Future<void> checkAuthStatus() async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      // Получаем SharedPreferences для проверки наличия токенов
      final prefs = sl<SharedPreferences>();
      final accessToken = prefs.getString('access_token');
      final refreshToken = prefs.getString('refresh_token');

      _token = accessToken;

      debugPrint(accessToken);
      debugPrint(refreshToken);

      debugPrint(
        'Проверка статуса авторизации: access_token=${accessToken != null}',
      );

      // Если нет токенов, то пользователь не авторизован
      if (accessToken == null ||
          accessToken.isEmpty ||
          refreshToken == null ||
          refreshToken.isEmpty) {
        _authStatus = AuthStatus.unauthenticated;
        _isLoading = false;
        notifyListeners();
        return;
      }

      // Есть токены, проверяем их валидность через API
      final apiClient = sl<ApiClient>();

      try {
        // Отправляем запрос на авторизацию
        final response = await apiClient.get(ApiConstants.authorize);

        // Обрабатываем ответ в зависимости от его типа
        if (response != null) {
          _authStatus = AuthStatus.authenticated;

          // Пробуем извлечь данные пользователя
          try {
            Map<String, dynamic>? userData;

            if (response is Map<String, dynamic>) {
              // Если response уже является Map, проверяем наличие ключа 'user'
              if (response.containsKey('user') &&
                  response['user'] is Map<String, dynamic>) {
                userData = response['user'] as Map<String, dynamic>;
                debugPrint('Получены данные пользователя из ключа "user"');
              } else {
                // Возможно, сами данные и есть пользователь
                userData = response;
                debugPrint('Используем весь ответ как данные пользователя');
              }
            } else if (response is List &&
                response.isNotEmpty &&
                response[0] is Map<String, dynamic>) {
              // Иногда API может вернуть массив с одним объектом
              userData = response[0] as Map<String, dynamic>;
              debugPrint(
                'Получены данные пользователя из первого элемента массива',
              );
            } else if (response.toString().contains('{') &&
                response.toString().contains('}')) {
              // Если ответ является строкой, содержащей JSON
              try {
                final dynamic jsonData = jsonDecode(response.toString());
                if (jsonData is Map<String, dynamic>) {
                  userData = jsonData;
                  debugPrint('Преобразовали строку в JSON');
                  if (userData.containsKey('user') &&
                      userData['user'] is Map<String, dynamic>) {
                    userData = userData['user'] as Map<String, dynamic>;
                    debugPrint(
                      'Получены данные пользователя из ключа "user" в JSON',
                    );
                  }
                }
              } catch (e) {
                debugPrint('Не удалось преобразовать строку в JSON: $e');
              }
            }

            // Если удалось извлечь данные пользователя, создаем объект User
            if (userData != null) {
              // Проверяем наличие необходимых полей
              if (userData.containsKey('email')) {
                _currentUser = User.fromJson(userData);
                debugPrint(
                  'Создан объект пользователя: ${_currentUser?.name}, email: '
                  '${_currentUser?.email}, role: ${_currentUser?.role}',
                );
              } else {
                debugPrint(
                  'В данных пользователя отсутствует email: $userData',
                );
              }
            } else {
              debugPrint(
                'Не удалось извлечь данные пользователя из ответа: $response',
              );
            }
          } catch (e) {
            debugPrint('Ошибка при обработке данных пользователя: $e');
            // При ошибке обработки данных пользователя, но валидном токене,
            // мы все равно считаем пользователя авторизованным
          }
        } else {
          debugPrint('Получен пустой ответ от authorize');
          // Даже если ответ пустой, но нет ошибки, считаем токен валидным
          _authStatus = AuthStatus.authenticated;
        }

        _isLoading = false;
        notifyListeners();
      } catch (e) {
        // Если произошла ошибка (например, 401 Unauthorized)
        // то считаем, что пользователь не авторизован
        // Интерцептор Dio уже должен обработать ситуацию с refresh токеном
        // и если он тоже невалиден, то вызовет forceLogout
        debugPrint('Ошибка при проверке токена: $e');
        _authStatus = AuthStatus.unauthenticated;
        _isLoading = false;
        notifyListeners();
      }
    } catch (e) {
      debugPrint('Общая ошибка при проверке статуса авторизации: $e');
      _authStatus = AuthStatus.unauthenticated;
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<bool> login({required String email, required String password}) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final result = await _loginUseCase(
      LoginParams(email: email, password: password),
    );

    _isLoading = false;

    return result.fold(
      (failure) {
        _errorMessage = failure.message;
        _authStatus = AuthStatus.unauthenticated;
        notifyListeners();
        return false;
      },
      (response) {
        _authStatus = AuthStatus.authenticated;

        // Модель AccessTokenModel не содержит данных пользователя
        // Загрузим данные пользователя через отдельный запрос
        checkAuthStatus();

        notifyListeners();
        return true;
      },
    );
  }

  Future<bool> registration({
    required String email,
    required String password,
    required String firstName,
    required String lastName,
    required String dateOfBirth,
  }) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final result = await _registrationUseCase(
      RegistrationParams(
        email: email,
        password: password,
        firstName: firstName,
        lastName: lastName,
        dateOfBirth: dateOfBirth,
      ),
    );

    _isLoading = false;

    return result.fold(
      (failure) {
        _errorMessage = failure.message;
        _authStatus = AuthStatus.unauthenticated;
        notifyListeners();
        return false;
      },
      (response) {
        _authStatus = AuthStatus.authenticated;

        // Создаем базовый объект пользователя, так как мы знаем его имя и email
        _currentUser = User(
          id: '', // ID будет получен позже при запросе данных пользователя
          email: email,
          name: '$firstName $lastName',
          dateOfBirth: dateOfBirth,
          role: '',
          balance: 0,
          createdAt: DateTime.now(),
        );

        // Загрузим полные данные пользователя
        checkAuthStatus();

        notifyListeners();
        return true;
      },
    );
  }

  Future<bool> signInWithGoogle() async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      debugPrint('Запуск Google Sign-In...');
      // Получаем сервис для Google Auth
      final googleAuthService = sl<GoogleAuthService>();

      // Попробуем сначала получить отладочную информацию
      await googleAuthService.debugGoogleSignIn();

      final result = await _googleSignInUseCase();

      _isLoading = false;

      return result.fold(
        (failure) {
          _errorMessage = failure.message;
          _authStatus = AuthStatus.unauthenticated;
          debugPrint('Ошибка Google Sign-In: ${failure.message}');
          notifyListeners();
          return false;
        },
        (response) {
          _authStatus = AuthStatus.authenticated;
          debugPrint('Успешная аутентификация Google Sign-In');

          // Загрузим данные из Google аккаунта
          _loadGoogleUserData();

          // Также загрузим данные пользователя с сервера
          checkAuthStatus();

          notifyListeners();
          return true;
        },
      );
    } catch (e) {
      debugPrint('Исключение при Google Sign-In: $e');
      _isLoading = false;
      _errorMessage = 'Ошибка входа через Google: $e';
      _authStatus = AuthStatus.unauthenticated;
      notifyListeners();
      return false;
    }
  }

  Future<void> _loadGoogleUserData() async {
    try {
      final googleUser = await _googleSignIn.signInSilently();
      if (googleUser != null) {
        // Создаем базовый объект пользователя из данных Google
        _currentUser = User(
          id: googleUser.id ?? '', // Добавлена проверка на null
          email: googleUser.email ?? '', // Добавлена проверка на null
          name: googleUser.displayName ?? '',
          photoUrl:
              googleUser.photoUrl ??
              '', // Проверьте, может ли это поле быть null
          role: '',
          dateOfBirth: '',
          // Если balance должен быть типа Decimal
          balance: 0,
          createdAt: DateTime.now(),
        );
        debugPrint(
          'Получены данные пользователя из Google: ${_currentUser?.name}',
        );
      }
    } catch (e) {
      debugPrint('Ошибка при загрузке данных пользователя из Google: $e');
    }
  }

  Future<void> logout() async {
    _isLoading = true;
    notifyListeners();

    try {
      // Получаем SharedPreferences для удаления токенов
      final prefs = sl<SharedPreferences>();

      // Получаем ApiClient для отправки запроса на выход
      final apiClient = sl<ApiClient>();

      try {
        // Пытаемся отправить запрос на удаление сессии на сервере
        await apiClient.post(ApiConstants.logout);
      } catch (e) {
        // Игнорируем ошибки при запросе на выход
        debugPrint('Ошибка при запросе на выход: $e');
      }

      // Удаляем токены локально в любом случае
      await prefs.remove('access_token');
      await prefs.remove('refresh_token');

      // Выходим из Google аккаунта, если он использовался
      try {
        await _googleSignIn.signOut();
      } catch (e) {
        // Игнорируем ошибки при выходе из Google
        debugPrint('Ошибка при выходе из Google: $e');
      }

      // Обновляем состояние
      _authStatus = AuthStatus.unauthenticated;
      _currentUser = null;
    } catch (e) {
      debugPrint('Общая ошибка при выходе из системы: $e');
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  // Обновленный метод forceLogout в AuthProvider
  void forceLogout() {
    debugPrint('Принудительный выход из системы: токен истек');

    // Очищаем учетные данные
    _authStatus = AuthStatus.unauthenticated;
    _currentUser = null;
    _errorMessage = 'Ваша сессия истекла. Пожалуйста, войдите снова.';

    // Очищаем токены в SharedPreferences (для страховки)
    final prefs = sl<SharedPreferences>();
    prefs.remove('access_token');
    prefs.remove('refresh_token');

    // Уведомляем слушателей об изменении
    notifyListeners();

    // Навигация на экран входа будет обработана в main.dart через слушатель
  }

  void updateSavedEmail(String email) {
    debugPrint("-------- $email");
    _savedEmail = email;
    notifyListeners();
  }

  void clearError() {
    _errorMessage = null;
    notifyListeners();
  }
}
