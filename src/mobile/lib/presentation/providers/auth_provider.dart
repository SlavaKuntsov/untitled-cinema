import 'package:flutter/material.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:mobile/domain/usecases/auth/registration.dart';

import '../../data/datasources/google_auth_service.dart';
import '../../di/injection_container.dart';
import '../../domain/entities/auth/user.dart';
import '../../domain/usecases/auth/google_sign_in.dart';
import '../../domain/usecases/auth/login.dart';

enum AuthStatus { unknown, authenticated, unauthenticated }

class AuthProvider extends ChangeNotifier {
  final LoginUseCase _loginUseCase;
  final RegistrationUseCase _registrationUseCase;
  final GoogleSignInUseCase _googleSignInUseCase;
  final GoogleSignIn _googleSignIn;

  AuthStatus _authStatus = AuthStatus.unknown;
  User? _currentUser;
  String? _savedEmail;
  String? _errorMessage;
  bool _isLoading = false;

  AuthProvider({
    required LoginUseCase loginUseCase,
    required RegistrationUseCase registrationUseCase,
    required GoogleSignInUseCase googleSignInUseCase,
    required GoogleSignIn googleSignIn,
  }) : _loginUseCase = loginUseCase,
       _registrationUseCase = registrationUseCase,
       _googleSignInUseCase = googleSignInUseCase,
       _googleSignIn = googleSignIn;

  AuthStatus get authStatus => _authStatus;
  User? get currentUser => _currentUser;
  String? get savedEmail => _savedEmail;
  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;

  Future<void> checkAuthStatus() async {
    // Здесь можно добавить логику проверки статуса авторизации
    // Например, проверить наличие токена в SharedPreferences
    // и установить соответствующий статус

    // Для примера, установим статус не авторизован
    _authStatus = AuthStatus.unauthenticated;
    notifyListeners();
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
      (token) {
        _authStatus = AuthStatus.authenticated;
        // Здесь можно добавить загрузку данных пользователя
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
      (token) {
        _authStatus = AuthStatus.authenticated;
        // Здесь можно добавить загрузку данных пользователя
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
        (token) {
          _authStatus = AuthStatus.authenticated;
          debugPrint('Успешная аутентификация Google Sign-In');
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

  Future<void> logout() async {
    _isLoading = true;
    notifyListeners();

    // Здесь должен быть вызов usecase для выхода
    // Для примера просто установим статус

    await Future.delayed(const Duration(milliseconds: 500));

    _authStatus = AuthStatus.unauthenticated;
    _currentUser = null;
    _isLoading = false;
    notifyListeners();
  }

  void updateSavedEmail(String email) {
    _savedEmail = email;
  }

  void clearError() {
    _errorMessage = null;
    notifyListeners();
  }
}
