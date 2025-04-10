import 'package:equatable/equatable.dart';

class User extends Equatable {
  final String id;
  final String email;
  final String name;
  final String? photoUrl;
  final bool isEmailVerified;
  final DateTime createdAt;

  const User({
    required this.id,
    required this.email,
    required this.name,
    this.photoUrl,
    required this.isEmailVerified,
    required this.createdAt,
  });

  @override
  List<Object?> get props => [
    id,
    email,
    name,
    photoUrl,
    isEmailVerified,
    createdAt,
  ];
}
