import 'package:dartz/dartz.dart';

import '../../core/errors/failures.dart';
import '../../data/models/auth/token_model.dart';
import '../entities/auth/token.dart';
import '../entities/auth/user.dart';

abstract class AuthRepository {
  Future<Either<Failure, TokenModel>> login({
    required String email,
    required String password,
  });

  Future<Either<Failure, TokenModel>> register({
    required String firstName,
    required String lastName,
    required String email,
    required String password,
    required String dateOfBirth,
  });

  Future<Either<Failure, Token>> googleSignIn();

  Future<Either<Failure, bool>> logout();

  Future<Either<Failure, User>> getCurrentUser();

  Future<Either<Failure, bool>> isAuthenticated();

  Future<Either<Failure, User>> updateUser({
    required String firstName,
    required String lastName,
    required String dateOfBirth,
  });

  Future<Either<Failure, bool>> deleteUserAccount();
}
