import 'dart:convert';
import 'package:dartz/dartz.dart';
import 'package:google_sign_in/google_sign_in.dart';

import '../../core/errors/exceptions.dart';
import '../../core/errors/failures.dart';
import '../../core/network/network_info.dart';
import '../../domain/entities/auth/token.dart';
import '../../domain/entities/auth/user.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth/auth_remote_data_source.dart';
import '../models/auth/token_model.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;
  final GoogleSignIn _googleSignClient;

  AuthRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
    required GoogleSignIn googleSignIn,
  }) : _googleSignClient = googleSignIn;

  // Helper method to extract detail from error message
  String _extractDetailFromError(String errorMessage) {
    try {
      // Handle error message that starts with curly brace but isn't valid JSON
      if (errorMessage.startsWith('{') && errorMessage.contains('detail:')) {
        // Extract detail value using string manipulation
        final detailStart = errorMessage.indexOf('detail:') + 'detail:'.length;
        final detailEnd = errorMessage.indexOf(',', detailStart);
        if (detailEnd > detailStart) {
          return errorMessage.substring(detailStart, detailEnd).trim();
        }
      }

      // Try parsing as JSON if above method fails
      final Map<String, dynamic> errorJson = jsonDecode(errorMessage);
      return errorJson['detail'] ?? errorMessage;
      
    } catch (e) {
      // If all parsing fails, return the original message
      return errorMessage;
    }
  }

  @override
  Future<Either<Failure, TokenModel>> login({
    required String email,
    required String password,
  }) async {
    if (await networkInfo.isConnected) {
      try {
        final accessToken = await remoteDataSource.login(
          email: email,
          password: password,
        );
        return Right(accessToken);
      } on AuthException catch (e) {
        return Left(AuthFailure(_extractDetailFromError(e.toString())));
      } on ServerException catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      } catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      }
    } else {
      return const Left(NetworkFailure('No internet connection'));
    }
  }

  @override
  Future<Either<Failure, TokenModel>> register({
    required String email,
    required String password,
    required String firstName,
    required String lastName,
    required String dateOfBirth,
  }) async {
    if (await networkInfo.isConnected) {
      try {
        final tokenModel = await remoteDataSource.register(
          firstName: firstName,
          lastName: lastName,
          email: email,
          password: password,
          dateOfBirth: dateOfBirth,
        );
        return Right(tokenModel);
      } on AuthException catch (e) {
        return Left(AuthFailure(_extractDetailFromError(e.toString())));
      } on ServerException catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      } catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      }
    } else {
      return const Left(NetworkFailure('No internet connection'));
    }
  }

  @override
  Future<Either<Failure, Token>> googleSignIn() async {
    if (await networkInfo.isConnected) {
      try {
        final tokenModel = await remoteDataSource.googleSignIn();
        return Right(tokenModel);
      } on GoogleSignInException catch (e) {
        return Left(GoogleSignInFailure(_extractDetailFromError(e.toString())));
      } on ServerException catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      } catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      }
    } else {
      return const Left(NetworkFailure('No internet connection'));
    }
  }

  @override
  Future<Either<Failure, bool>> logout() async {
    if (await networkInfo.isConnected) {
      try {
        final result = await remoteDataSource.logout();
        // Выход из Google аккаунта, если пользователь был авторизован через Google
        await _googleSignClient.signOut();
        return Right(result);
      } on AuthException catch (e) {
        return Left(AuthFailure(_extractDetailFromError(e.toString())));
      } on ServerException catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      } catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      }
    } else {
      return const Left(NetworkFailure('No internet connection'));
    }
  }

  @override
  Future<Either<Failure, User>> getCurrentUser() async {
    if (await networkInfo.isConnected) {
      try {
        final userModel = await remoteDataSource.getCurrentUser();
        return Right(userModel);
      } on AuthException catch (e) {
        return Left(AuthFailure(_extractDetailFromError(e.toString())));
      } on ServerException catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      } catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      }
    } else {
      return const Left(NetworkFailure('No internet connection'));
    }
  }

  @override
  Future<Either<Failure, bool>> isAuthenticated() async {
    try {
      final result = await remoteDataSource.isAuthenticated();
      return Right(result);
    } catch (e) {
      return Left(AuthFailure(_extractDetailFromError(e.toString())));
    }
  }

  @override
  Future<Either<Failure, User>> updateUser({
    required String firstName,
    required String lastName,
    required String dateOfBirth,
  }) async {
    if (await networkInfo.isConnected) {
      try {
        final user = await remoteDataSource.updateUser(
          firstName: firstName,
          lastName: lastName,
          dateOfBirth: dateOfBirth,
        );
        return Right(user);
      } on AuthException catch (e) {
        return Left(AuthFailure(_extractDetailFromError(e.toString())));
      } on ServerException catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      } catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      }
    } else {
      return const Left(NetworkFailure('No internet connection'));
    }
  }

  @override
  Future<Either<Failure, bool>> deleteUserAccount() async {
    if (await networkInfo.isConnected) {
      try {
        final result = await remoteDataSource.deleteUserAccount();
        return Right(result);
      } on AuthException catch (e) {
        return Left(AuthFailure(_extractDetailFromError(e.toString())));
      } on ServerException catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      } catch (e) {
        return Left(ServerFailure(_extractDetailFromError(e.toString())));
      }
    } else {
      return const Left(NetworkFailure('No internet connection'));
    }
  }
}
