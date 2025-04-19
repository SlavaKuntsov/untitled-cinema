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

  // Фабричный метод для создания объекта User из JSON
  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id'] ?? '',
      email: json['email'] ?? '',
      name: json['name'] ?? json['firstName'] ?? '',  // Поддерживает оба варианта (name или firstName)
      photoUrl: json['photoUrl'] ?? json['photo'],  // Поддерживает варианты photoUrl или photo
      isEmailVerified: json['isEmailVerified'] ?? false,
      createdAt: json['createdAt'] != null 
          ? DateTime.parse(json['createdAt']) 
          : DateTime.now(),
    );
  }

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
