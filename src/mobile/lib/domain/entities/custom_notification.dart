// lib/domain/entities/notification/custom_notification.dart
import 'package:equatable/equatable.dart';

class CustomNotification extends Equatable {
  final String id;
  final String userId;
  final String message;
  final String type;
  final DateTime createdAt;

  const CustomNotification({
    required this.id,
    required this.userId,
    required this.message,
    required this.type,
    required this.createdAt,
  });

  @override
  List<Object?> get props => [id, userId, message, type, createdAt];

  // Маппинг типа уведомления в цвет для SnackBar
  bool get isSuccess => type.toLowerCase() == 'success';
  bool get isError => type.toLowerCase() == 'error';
  bool get isWarning => type.toLowerCase() == 'warn';
  bool get isInfo => type.toLowerCase() == 'info';
}