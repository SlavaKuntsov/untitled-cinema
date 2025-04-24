import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

import '../../core/constants/api_constants.dart';
import '../../core/constants/oauth_constants.dart';
import '../../core/errors/exceptions.dart';

class GoogleAuthService {
  final GoogleSignIn _googleSignIn;
  final SharedPreferences _prefs;

  GoogleAuthService({
    required GoogleSignIn googleSignIn,
    required SharedPreferences prefs,
  }) : _googleSignIn = googleSignIn,
       _prefs = prefs;

  // Метод для вывода отладочной информации
  Future<void> debugGoogleSignIn() async {
    try {
      // Выводим информацию о текущем состоянии
      debugPrint(
        'GoogleSignIn состояние: ${_googleSignIn.currentUser?.email ?? "Нет пользователя"}',
      );
      debugPrint('Scopes: ${_googleSignIn.scopes.join(", ")}');

      // Проверяем, залогинен ли пользователь
      final isSignedIn = await _googleSignIn.isSignedIn();
      debugPrint('isSignedIn: $isSignedIn');

      // Если залогинен, показываем информацию
      if (isSignedIn && _googleSignIn.currentUser != null) {
        final user = _googleSignIn.currentUser!;
        debugPrint('Email: ${user.email}');
        debugPrint('Name: ${user.displayName}');
        debugPrint('ID: ${user.id}');
      }
    } catch (e) {
      debugPrint('Ошибка при debug Google Sign-In: $e');
    }
  }

  // Метод для выхода из Google аккаунта
  Future<void> signOut() async {
    try {
      await _googleSignIn.signOut();
      debugPrint('Успешно вышли из Google аккаунта');
    } catch (e) {
      debugPrint('Ошибка при выходе из Google аккаунта: $e');
    }
  }

  // Метод для отключения аккаунта (полный сброс)
  Future<void> disconnect() async {
    try {
      await _googleSignIn.disconnect();
      debugPrint('Аккаунт Google отключен');
    } catch (e) {
      debugPrint('Ошибка при отключении Google аккаунта: $e');
    }
  }

  Future<Map<String, dynamic>> signInWithGoogle() async {
    try {
      // Сначала выведем отладочную информацию
      await debugGoogleSignIn();

      // При повторных ошибках попробуем сначала выйти
      await signOut();

      debugPrint('Начинаем процесс входа через Google');

      // Запускаем процесс входа через Google
      final GoogleSignInAccount? googleUser = await _googleSignIn.signIn();
      if (googleUser == null) {
        debugPrint('Вход отменен пользователем');
        throw const GoogleSignInException('Google sign-in was canceled');
      }

      debugPrint('Получен Google аккаунт: ${googleUser.email}');

      // Получаем данные аутентификации от Google
      final GoogleSignInAuthentication googleAuth =
          await googleUser.authentication;

      debugPrint('Получены токены от Google');
      debugPrint('idToken length: ${googleAuth.idToken?.length ?? 0}');
      debugPrint('accessToken length: ${googleAuth.accessToken?.length ?? 0}');

      // Проверим идентификатор клиента OAuth
      debugPrint('ClientID: ${GoogleOAuthConstants.CLIENT_ID}');

      debugPrint('-----------idToken: ${googleAuth.idToken}');

      // Отправляем запрос на сервер для входа через Google
      // Меняем подход - вместо GET запроса используем POST с данными
      // final response = await http.get(
      //   Uri.parse(ApiConstants.googleAuth),
      //   headers: {
      //     'Content-Type': 'application/json',
      //     'Accept': 'application/json',
      //   },
      // );
      final response = await http.post(
        Uri.parse(ApiConstants.googleMobileAuth),
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
        body: json.encode({'idToken': googleAuth.idToken}),
      );

      debugPrint('Ответ от сервера: ${response.statusCode}');
      if (response.statusCode >= 200 && response.statusCode < 300) {
        debugPrint('Тело ответа: ${response.body}');

        // Сохраняем refresh token из куки (если он там есть)
        _saveRefreshTokenFromCookies(response);

        // Парсим JSON ответ
        final responseData = json.decode(response.body);

        // Сохраняем access token
        if (responseData['accessToken'] != null) {
          await _prefs.setString('access_token', responseData['accessToken']);
          debugPrint('Access token сохранен');
        } else if (responseData['authResultDto'] != null &&
            responseData['authResultDto']['accessToken'] != null) {
          await _prefs.setString(
            'access_token',
            responseData['authResultDto']['accessToken'],
          );
          debugPrint('Access token сохранен из authResultDto');
        }

        return responseData;
      } else {
        debugPrint(
          'Ошибка от сервера: ${response.statusCode}, ${response.body}',
        );
        throw ServerException(
          'Server error: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      debugPrint('Ошибка при входе через Google: $e');
      throw GoogleSignInException('Google sign-in failed: $e');
    }
  }

  // Извлекаем refresh token из кук ответа
  void _saveRefreshTokenFromCookies(http.Response response) {
    if (response.headers.containsKey('set-cookie')) {
      final cookies = response.headers['set-cookie'];
      debugPrint('Получены куки: $cookies');
      if (cookies != null) {
        // Ищем refresh token в куках
        final refreshTokenCookies =
            cookies
                .split(';')
                .where(
                  (cookie) => cookie.trim().startsWith(
                    JwtConstants.REFRESH_COOKIE_NAME,
                  ),
                )
                .toList();

        if (refreshTokenCookies.isNotEmpty) {
          final refreshTokenCookie = refreshTokenCookies.first;
          final refreshToken = refreshTokenCookie.split('=')[1];
          _prefs.setString('refresh_token', refreshToken);
          debugPrint('Refresh token сохранен: $refreshToken');
        } else {
          debugPrint('Refresh token не найден в куках');
        }
      }
    } else {
      debugPrint('Куки отсутствуют в ответе');
    }
  }
}

// Константы JWT, соответствующие серверным
class JwtConstants {
  static const String REFRESH_COOKIE_NAME = "yummy-cackes";
}
