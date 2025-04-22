import 'package:equatable/equatable.dart';

class User extends Equatable {
  final String id;
  final String email;
  final String name;
  final String role;
  final String dateOfBirth;
  final double balance;
  final String? photoUrl;
  final DateTime createdAt;

  const User({
    required this.id,
    required this.email,
    required this.name,
    required this.role,
    required this.dateOfBirth,
    required this.balance,
    this.photoUrl,
    required this.createdAt,
  });

  factory User.fromJson(Map<String, dynamic> json) {
    // Функция для безопасного преобразования числовых типов в double
    double parseDouble(dynamic value) {
      if (value == null) return 0.0;
      if (value is int) return value.toDouble();
      if (value is double) return value;
      if (value is String) {
        try {
          return double.parse(value);
        } catch (e) {
          return 0.0;
        }
      }
      return 0.0;
    }

    return User(
      id: json['id']?.toString() ?? '',
      email: json['email']?.toString() ?? '',
      role: json['role']?.toString() ?? '',
      dateOfBirth: json['dateOfBirth']?.toString() ?? '',
      balance: parseDouble(json['balance']),
      name: json['name']?.toString() ?? json['firstName']?.toString() ?? '',
      photoUrl: json['photoUrl']?.toString() ?? json['photo']?.toString(),
      createdAt:
          json['createdAt'] != null
              ? DateTime.parse(json['createdAt'].toString())
              : DateTime.now(),
    );
  }

  @override
  List<Object?> get props => [id, email, name, photoUrl, createdAt];
}
