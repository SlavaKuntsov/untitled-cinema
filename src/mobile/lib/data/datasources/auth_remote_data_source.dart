import 'package:google_sign_in/google_sign_in.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../core/constants/api_constants.dart';
import '../../core/errors/exceptions.dart';
import '../../core/network/api_client.dart';
import '../models/auth/token_model.dart';
import '../models/auth/user_model.dart';
import 'google_auth_service.dart';

abstract class AuthRemoteDataSource {
  Future<TokenModel> login({required String email, required String password});

  Future<TokenModel> register({
    required String name,
    required String email,
    required String password,
  });

  Future<TokenModel> googleSignIn();

  Future<bool> logout();

  Future<UserModel> getCurrentUser();

  Future<bool> isAuthenticated();
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  final ApiClient client;
  final GoogleSignIn _googleSignClient;
  final SharedPreferences prefs;

  AuthRemoteDataSourceImpl({
    required this.client,
    required GoogleSignIn googleSignIn,
    required this.prefs,
  }) : _googleSignClient = googleSignIn;

  @override
  Future<TokenModel> login({
    required String email,
    required String password,
  }) async {
    try {
      final response = await client.post(
        ApiConstants.login,
        data: {'email': email, 'password': password},
      );

      final TokenModel tokenModel = TokenModel.fromJson(response);

      // Сохраняем токены
      await _saveTokens(tokenModel);

      return tokenModel;
    } catch (e) {
      throw AuthException(e.toString());
    }
  }

  @override
  Future<TokenModel> register({
    required String name,
    required String email,
    required String password,
  }) async {
    try {
      final response = await client.post(
        ApiConstants.register,
        data: {'name': name, 'email': email, 'password': password},
      );

      final TokenModel tokenModel = TokenModel.fromJson(response);

      // Сохраняем токены
      await _saveTokens(tokenModel);

      return tokenModel;
    } catch (e) {
      throw AuthException(e.toString());
    }
  }

  @override
  Future<TokenModel> googleSignIn() async {
    try {
      // Создаем сервис Google аутентификации
      final googleAuthService = GoogleAuthService(
        googleSignIn: _googleSignClient,
        prefs: prefs,
      );

      // Запускаем процесс аутентификации
      final response = await googleAuthService.signInWithGoogle();

      // Извлекаем токен из ответа сервера
      if (response['authResultDto'] != null) {
        final tokenData = {
          'access_token': response['authResultDto']['accessToken'],
          'refresh_token': prefs.getString('refresh_token') ?? '',
          'token_type': 'Bearer',
          'expires_in': 3600, // Значение по умолчанию, если не указано в ответе
        };

        final TokenModel tokenModel = TokenModel.fromJson(tokenData);

        // Токены уже сохранены в GoogleAuthService, но можно еще раз для уверенности
        await _saveTokens(tokenModel);

        return tokenModel;
      } else {
        throw const GoogleSignInException(
          'Invalid response from server: missing token data',
        );
      }
    } catch (e) {
      throw GoogleSignInException(e.toString());
    }
  }

  @override
  Future<bool> logout() async {
    try {
      await client.post(ApiConstants.logout);

      // Удаляем токены
      await _clearTokens();

      return true;
    } catch (e) {
      throw AuthException(e.toString());
    }
  }

  @override
  Future<UserModel> getCurrentUser() async {
    try {
      if (!await isAuthenticated()) {
        throw const UnauthorizedException();
      }

      final response = await client.get('/auth/user');
      return UserModel.fromJson(response);
    } catch (e) {
      throw AuthException(e.toString());
    }
  }

  @override
  Future<bool> isAuthenticated() async {
    final accessToken = prefs.getString('access_token');
    return accessToken != null && accessToken.isNotEmpty;
  }

  Future<void> _saveTokens(TokenModel tokenModel) async {
    await prefs.setString('access_token', tokenModel.accessToken);
    await prefs.setString('refresh_token', tokenModel.refreshToken);
    await prefs.setInt('token_expires_in', tokenModel.expiresIn);
    await prefs.setString('token_type', tokenModel.tokenType);
  }

  Future<void> _clearTokens() async {
    await prefs.remove('access_token');
    await prefs.remove('refresh_token');
    await prefs.remove('token_expires_in');
    await prefs.remove('token_type');
  }
}
