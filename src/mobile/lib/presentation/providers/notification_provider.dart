// lib/presentation/providers/notification_provider.dart
import 'package:flutter/material.dart';
import 'package:untitledCinema/core/services/notification_service.dart';

import '../../domain/entities/custom_notification.dart';

class NotificationProvider extends ChangeNotifier {
  final NotificationService _notificationService;
  List<CustomNotification> _notifications = [];
  bool _isLoading = false;
  String? _errorMessage;

  List<CustomNotification> get notifications => _notifications;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  // Dependency Injection через конструктор
  NotificationProvider({required NotificationService notificationService})
    : _notificationService = notificationService {
    // Подписываемся на поток уведомлений в конструкторе
    _notificationService.notifications.listen((notifications) {
      _notifications = notifications;
      notifyListeners();
    });
  }

  // Подписка на уведомления - теперь только инициализация с токеном
  void initialize(String token) {
    _notificationService.initialize(token);
  }

  // Статус приложения
  void setAppStatus(bool isInForeground) {
    _notificationService.setAppStatus(isInForeground);
  }

  // Получение уведомлений с сервера
  Future<void> fetchNotifications(String token) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      _notifications = await _notificationService.getNotifications(token);

      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _isLoading = false;
      _errorMessage = 'Ошибка при получении уведомлений: ${e.toString()}';
      notifyListeners();
    }
  }

  // Отправка уведомления
  Future<bool> sendNotification(String token, String message) async {
    try {
      return await _notificationService.sendNotification(token, message);
    } catch (e) {
      _errorMessage = 'Ошибка при отправке уведомления: ${e.toString()}';
      notifyListeners();
      return false;
    }
  }

  // Удаление уведомления
  Future<bool> deleteNotification(String token, String id) async {
    try {
      return await _notificationService.deleteNotification(token, id);
    } catch (e) {
      _errorMessage = 'Ошибка при удалении уведомления: ${e.toString()}';
      notifyListeners();
      return false;
    }
  }

  // Получение стрима in-app уведомлений для виджетов
  Stream<CustomNotification> get inAppNotifications =>
      _notificationService.inAppNotifications;

  @override
  void dispose() {
    // Не закрываем _notificationService.dispose() здесь,
    // так как сервис может использоваться в других местах
    super.dispose();
  }
}
