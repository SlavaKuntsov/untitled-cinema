import 'package:dartz/dartz.dart';

import '../../../core/errors/failures.dart';
import '../../entities/auth/token.dart';
import '../../repositories/auth_repository.dart';

class GoogleSignInUseCase {
  final AuthRepository repository;

  GoogleSignInUseCase(this.repository);

  Future<Either<Failure, Token>> call() async {
    return await repository.googleSignIn();
  }
}
