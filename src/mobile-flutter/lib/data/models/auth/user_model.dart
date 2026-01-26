import '../../../domain/entities/auth/user.dart';

class UserModel extends User {
  const UserModel({
    required super.id,
    required super.email,
    required super.firstName,
    required super.lastName,
    required super.role,
    required super.dateOfBirth,
    required super.balance,
    super.photoUrl,
    required super.createdAt,
  });

  factory UserModel.fromJson(Map<String, dynamic> json) {
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

    return UserModel(
      id: json['id'],
      email: json['email'],
      firstName: json['firstName'],
      lastName: json['lastName'],
      role: json['role'] ?? '',
      dateOfBirth: json['dateOfBirth'] ?? '',
      balance: 0,
      createdAt: DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'email': email,
      'firstName': firstName,
      'lastName': lastName,
      'photo_url': photoUrl,
      'created_at': createdAt.toIso8601String(),
    };
  }

  factory UserModel.fromEntity(User user) {
    return UserModel(
      id: user.id,
      email: user.email,
      firstName: user.firstName,
      lastName: user.lastName,
      photoUrl: user.photoUrl,
      createdAt: user.createdAt,
      dateOfBirth: user.dateOfBirth,
      role: user.role,
      balance: user.balance,
    );
  }

  UserModel copyWith({
    String? id,
    String? email,
    String? firstName,
    String? lastName,
    String? photoUrl,
    String? role,
    String? dateOfBirth,
    double? balance,
    DateTime? createdAt,
  }) {
    return UserModel(
      id: id ?? this.id,
      email: email ?? this.email,
      firstName: firstName ?? this.firstName,
      lastName: lastName ?? this.lastName,
      role: role ?? this.role,
      dateOfBirth: dateOfBirth ?? this.dateOfBirth,
      balance: balance ?? this.balance,
      photoUrl: photoUrl ?? this.photoUrl,
      createdAt: createdAt ?? this.createdAt,
    );
  }
}
