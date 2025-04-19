import 'package:dartz/dartz.dart';
import 'package:google_sign_in/google_sign_in.dart';

import '../../core/errors/exceptions.dart';
import '../../core/errors/failures.dart';
import '../../core/network/network_info.dart';
import '../../domain/entities/auth/token.dart';
import '../../domain/entities/auth/user.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_remote_data_source.dart';
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
        return Left(AuthFailure(e.toString()));
      } on ServerException catch (e) {
        return Left(ServerFailure(e.toString()));
      } catch (e) {
        return Left(ServerFailure(e.toString()));
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
  }) async {
    if (await networkInfo.isConnected) {
      try {
        final tokenModel = await remoteDataSource.register(
          firstName: firstName,
          lastName: lastName,
          email: email,
          password: password,
        );
        return Right(tokenModel);
      } on AuthException catch (e) {
        return Left(AuthFailure(e.toString()));
      } on ServerException catch (e) {
        return Left(ServerFailure(e.toString()));
      } catch (e) {
        return Left(ServerFailure(e.toString()));
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
        return Left(GoogleSignInFailure(e.toString()));
      } on ServerException catch (e) {
        return Left(ServerFailure(e.toString()));
      } catch (e) {
        return Left(ServerFailure(e.toString()));
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
        return Left(AuthFailure(e.toString()));
      } on ServerException catch (e) {
        return Left(ServerFailure(e.toString()));
      } catch (e) {
        return Left(ServerFailure(e.toString()));
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
        return Left(AuthFailure(e.toString()));
      } on ServerException catch (e) {
        return Left(ServerFailure(e.toString()));
      } catch (e) {
        return Left(ServerFailure(e.toString()));
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
      return Left(AuthFailure(e.toString()));
    }
  }
}
