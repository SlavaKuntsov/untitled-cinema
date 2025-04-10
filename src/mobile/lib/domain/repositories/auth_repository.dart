import 'package:dartz/dartz.dart';

import '../../core/errors/failures.dart';
import '../entities/auth/token.dart';
import '../entities/auth/user.dart';

abstract class AuthRepository {
  Future<Either<Failure, Token>> login({
    required String email,
    required String password,
  });

  Future<Either<Failure, Token>> register({
    required String name,
    required String email,
    required String password,
  });

  Future<Either<Failure, Token>> googleSignIn();

  Future<Either<Failure, bool>> logout();

  Future<Either<Failure, User>> getCurrentUser();

  Future<Either<Failure, bool>> isAuthenticated();
}
