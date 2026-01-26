import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';

import '../../../core/errors/failures.dart';
import '../../../data/models/auth/token_model.dart';
import '../../repositories/auth_repository.dart';

class RegistrationUseCase {
  final AuthRepository repository;

  RegistrationUseCase(this.repository);

  Future<Either<Failure, TokenModel>> call(RegistrationParams params) async {
    return await repository.register(
      email: params.email,
      password: params.password,
      firstName: params.firstName,
      lastName: params.lastName,
      dateOfBirth: params.dateOfBirth,
    );
  }
}

class RegistrationParams extends Equatable {
  final String email;
  final String password;
  final String firstName;
  final String lastName;
  final String dateOfBirth;

  const RegistrationParams({
    required this.email,
    required this.password,
    required this.firstName,
    required this.lastName,
    required this.dateOfBirth,
  });

  @override
  List<Object?> get props => [email, password];
}
