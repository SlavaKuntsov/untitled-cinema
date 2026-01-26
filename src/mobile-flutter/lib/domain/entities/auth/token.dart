import 'package:equatable/equatable.dart';

class Token extends Equatable {
  final String accessToken;
  final String refreshToken;

  const Token({required this.accessToken, required this.refreshToken});

  @override
  List<Object?> get props => [accessToken, refreshToken];
}
