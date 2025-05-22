import 'package:google_sign_in/google_sign_in.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../../core/constants/api_constants.dart';
import '../../../core/errors/exceptions.dart';
import '../../../core/network/api_client.dart';
import '../../models/auth/token_model.dart';
import '../../models/auth/user_model.dart';
import 'google_auth_service.dart';

abstract class AuthRemoteDataSource {
  Future<TokenModel> login({required String email, required String password});

  Future<TokenModel> register({
    required String firstName,
    required String lastName,
    required String email,
    required String password,
    required String dateOfBirth,
  });

  Future<TokenModel> googleSignIn();

  Future<bool> logout();

  Future<UserModel> getCurrentUser();

  Future<bool> isAuthenticated();

  Future<UserModel> updateUser({
    required String firstName,
    required String lastName,
    required String dateOfBirth,
  });

  Future<bool> deleteUserAccount();
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

      final tokenData = {
        'access_token': response['accessToken'],
        'refresh_token': response['refreshToken'],
      };

      final TokenModel tokenModel = TokenModel.fromJson(tokenData);

      _saveTokens(tokenModel);

      return tokenModel;
    } catch (e) {
      throw AuthException(e.toString());
    }
  }

  @override
  Future<TokenModel> register({
    required String firstName,
    required String lastName,
    required String email,
    required String password,
    required String dateOfBirth,
  }) async {
    try {
      final response = await client.post(
        ApiConstants.register,
        data: {
          'email': email,
          'password': password,
          'firstName': firstName,
          'lastName': lastName,
          'dateOfBirth': dateOfBirth,
        },
      );

      final tokenData = {
        'access_token': response['accessToken'],
        'refresh_token': response['refreshToken'],
      };

      final TokenModel tokenModel = TokenModel.fromJson(tokenData);

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
  }

  Future<void> _clearTokens() async {
    await prefs.remove('access_token');
    await prefs.remove('refresh_token');
  }

  @override
  Future<UserModel> updateUser({
    required String firstName,
    required String lastName,
    required String dateOfBirth,
  }) async {
    try {
      final response = await client.patch(
        ApiConstants.users,
        data: {
          'firstName': firstName,
          'lastName': lastName,
          'dateOfBirth': dateOfBirth,
        },
      );

      return UserModel.fromJson(response);
    } catch (e) {
      throw AuthException(e.toString());
    }
  }

  @override
  Future<bool> deleteUserAccount() async {
    try {
      await client.delete(ApiConstants.delete);

      await _clearTokens();

      return true;
    } catch (e) {
      throw AuthException(e.toString());
    }
  }
}
