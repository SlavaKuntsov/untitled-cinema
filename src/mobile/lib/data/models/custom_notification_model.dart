// lib/data/models/notification/custom_notification_model.dart

import '../../domain/entities/custom_notification.dart';

class CustomNotificationModel extends CustomNotification {
  const CustomNotificationModel({
    required super.id,
    required super.userId,
    required super.message,
    required super.type,
    required super.createdAt,
  });

  factory CustomNotificationModel.fromJson(Map<String, dynamic> json) {
    return CustomNotificationModel(
      id: json['id'] ?? '',
      userId: json['userId'] ?? '',
      message: json['message'] ?? '',
      type: json['type'] ?? 'info',
      createdAt:
          json['createdAt'] != null
              ? DateTime.parse(json['createdAt'])
              : DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'userId': userId,
      'message': message,
      'type': type,
      'createdAt': createdAt.toIso8601String(),
    };
  }
}
