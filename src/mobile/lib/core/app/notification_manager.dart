// lib/core/app/notification_manager.dart
import 'package:flutter/material.dart';

import '../services/notification_service.dart';
import '../utils/app_lifecycle_observer.dart';

class NotificationManager {
  final NotificationService _notificationService = NotificationService();
  final AppLifecycleObserver _lifecycleObserver = AppLifecycleObserver();

  void initialize(String token) {
    // Инициализируем сервис уведомлений
    _notificationService.initialize(token);

    // Устанавливаем обработчик для отслеживания состояния приложения
    _lifecycleObserver.onStateChange = _handleAppLifecycleState;
  }

  void _handleAppLifecycleState(AppLifecycleState state) {
    switch (state) {
      case AppLifecycleState.resumed:
        // Приложение на переднем плане
        _notificationService.setAppStatus(true);
        break;
      case AppLifecycleState.paused:
      case AppLifecycleState.inactive:
      case AppLifecycleState.detached:
      case AppLifecycleState.hidden:
        // Приложение в фоне или закрыто
        _notificationService.setAppStatus(false);
        break;
    }
  }

  void dispose() {
    _lifecycleObserver.dispose();
  }
}
