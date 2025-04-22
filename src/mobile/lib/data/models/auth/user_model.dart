import '../../../domain/entities/auth/user.dart';

class UserModel extends User {
  const UserModel({
    required super.id,
    required super.email,
    required super.name,
    required super.role,
    required super.dateOfBirth,
    required super.balance,
    super.photoUrl,
    required super.createdAt,
  });

  factory UserModel.fromJson(Map<String, dynamic> json) {
    return UserModel(
      id: json['id'],
      email: json['email'],
      name: json['name'],
      photoUrl: json['photo_url'],
      role: json['role'] ?? '',
      dateOfBirth: json['dateOfBirth'] ?? '',
      balance: json['balance'] ?? 0,
      createdAt: DateTime.parse(json['created_at']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'email': email,
      'name': name,
      'photo_url': photoUrl,
      'created_at': createdAt.toIso8601String(),
    };
  }

  factory UserModel.fromEntity(User user) {
    return UserModel(
      id: user.id,
      email: user.email,
      name: user.name,
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
    String? name,
    String? photoUrl,
    String? role,
    String? dateOfBirth,
    double? balance,
    DateTime? createdAt,
  }) {
    return UserModel(
      id: id ?? this.id,
      email: email ?? this.email,
      name: name ?? this.name,
      role: role ?? this.role,
      dateOfBirth: dateOfBirth ?? this.dateOfBirth,
      balance: balance ?? this.balance,
      photoUrl: photoUrl ?? this.photoUrl,
      createdAt: createdAt ?? this.createdAt,
    );
  }
}
