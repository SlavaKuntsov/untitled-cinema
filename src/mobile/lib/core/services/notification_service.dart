// lib/core/services/notification_service.dart
import 'dart:async';
import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:rxdart/subjects.dart';
import 'package:signalr_netcore/signalr_client.dart';
import 'package:untitledCinema/core/services/signalr_service.dart';

import '../../data/models/custom_notification_model.dart';
import '../../domain/entities/custom_notification.dart';
import '../constants/api_constants.dart';

class NotificationService {
  // Dio клиент
  final Dio _dio;
  // SignalR сервис
  final SignalRService? signalRService;

  // SignalR соединение
  HubConnection? _hubConnection;
  bool _isConnected = false;

  // Стрим контроллер для уведомлений
  final _notificationsController =
      BehaviorSubject<List<CustomNotification>>.seeded([]);
  Stream<List<CustomNotification>> get notifications =>
      _notificationsController.stream;
  List<CustomNotification> get currentNotifications =>
      _notificationsController.value;

  // Инициализация локальных уведомлений
  final FlutterLocalNotificationsPlugin _flutterLocalNotificationsPlugin =
      FlutterLocalNotificationsPlugin();

  // Для определения находится ли приложение в фоне
  bool _isAppInForeground = true;

  NotificationService({required Dio client, this.signalRService})
    : _dio = client;

  // Инициализация сервиса
  Future<void> initialize(String accessToken) async {
    // Настройка Dio с токеном
    _dio.options.headers['Authorization'] = 'Bearer $accessToken';

    // Инициализируем локальные уведомления
    const androidInitSettings = AndroidInitializationSettings(
      '@mipmap/ic_launcher',
    );
    const iosInitSettings = DarwinInitializationSettings(
      requestSoundPermission: true,
      requestBadgePermission: true,
      requestAlertPermission: true,
    );
    const initSettings = InitializationSettings(
      android: androidInitSettings,
      iOS: iosInitSettings,
    );
    await _flutterLocalNotificationsPlugin.initialize(initSettings);

    // Подключаемся к SignalR если есть токен
    if (accessToken.isNotEmpty) {
      await startConnection(accessToken);
    }
  }

  // Метод для обновления статуса приложения (в фоне или на переднем плане)
  void setAppStatus(bool isInForeground) {
    _isAppInForeground = isInForeground;
  }

  // Запуск подключения к SignalR
  Future<void> startConnection(String accessToken) async {
    if (_isConnected || accessToken.isEmpty) {
      return;
    }

    try {
      // Создаем URL для подключения

      // Создаем подключение с токеном доступа
      _hubConnection =
          HubConnectionBuilder()
              .withUrl(
                ApiConstants.notificationsHub,
                options: HttpConnectionOptions(
                  accessTokenFactory: () async => accessToken,
                ),
              )
              .withAutomaticReconnect()
              .build();

      // Обработчик для получения уведомлений
      _hubConnection!.on('ReceiveNotification', _handleNotification);

      // Запуск подключения
      await _hubConnection!.start();
      debugPrint('SignalR Notification Connected');
      _isConnected = true;
    } catch (e) {
      debugPrint('Error connecting to SignalR: $e');
      _isConnected = false;
    }
  }

  // Остановка подключения
  Future<void> stopConnection() async {
    if (_hubConnection != null && _isConnected) {
      await _hubConnection!.stop();
      _isConnected = false;
      debugPrint('SignalR Notification Disconnected');
    }
  }

  // Обработчик уведомлений от SignalR
  void _handleNotification(List<Object?>? parameters) {
    if (parameters != null && parameters.isNotEmpty) {
      try {
        final notificationJson = parameters[0];
        if (notificationJson is Map<String, dynamic>) {
          final notification = CustomNotificationModel.fromJson(
            notificationJson,
          );

          // Добавляем в список уведомлений
          _notificationsController.add([...currentNotifications, notification]);

          // Отображаем уведомление
          _showNotification(notification);
        } else {
          debugPrint('Received invalid notification format');
        }
      } catch (e) {
        debugPrint('Error handling notification: $e');
      }
    }
  }

  // Показать уведомление (выбирает между SnackBar и системным уведомлением)
  void _showNotification(CustomNotification notification) {
    if (_isAppInForeground) {
      // Если приложение активно, отправляем событие для показа SnackBar
      _showInAppNotification(notification);
    } else {
      // Если приложение в фоне или закрыто, показываем системное уведомление
      _showSystemNotification(notification);
    }
  }

  // Stream для передачи уведомлений в виджеты (для SnackBar)
  final _inAppNotificationController =
      StreamController<CustomNotification>.broadcast();
  Stream<CustomNotification> get inAppNotifications =>
      _inAppNotificationController.stream;

  // Для отображения в приложении
  void _showInAppNotification(CustomNotification notification) {
    _inAppNotificationController.add(notification);
  }

  // Для отображения системного уведомления
  Future<void> _showSystemNotification(CustomNotification notification) async {
    // Определяем важность уведомления на основе типа
    final AndroidNotificationDetails androidDetails =
        AndroidNotificationDetails(
          'notifications_channel',
          'Уведомления',
          channelDescription: 'Канал для уведомлений приложения',
          importance: Importance.max,
          priority: Priority.high,
          ticker: 'ticker',
          color: _getNotificationColor(notification.type),
        );

    const DarwinNotificationDetails iosDetails = DarwinNotificationDetails(
      presentAlert: true,
      presentBadge: true,
      presentSound: true,
    );

    final NotificationDetails notificationDetails = NotificationDetails(
      android: androidDetails,
      iOS: iosDetails,
    );

    await _flutterLocalNotificationsPlugin.show(
      notification.hashCode,
      'Уведомление', // Заголовок
      notification.message, // Сообщение
      notificationDetails,
    );
  }

  // Dio запросы к API уведомлений
  Future<List<CustomNotification>> getNotifications(String accessToken) async {
    try {
      _dio.options.headers['Authorization'] = 'Bearer $accessToken';

      final response = await _dio.get(ApiConstants.notifications);

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = response.data;
        final notifications =
            jsonList
                .map((json) => CustomNotificationModel.fromJson(json))
                .toList();

        _notificationsController.add(notifications);
        return notifications;
      } else {
        throw Exception('Failed to load notifications: ${response.statusCode}');
      }
    } on DioException catch (e) {
      debugPrint('Dio error getting notifications: ${e.message}');
      return [];
    } catch (e) {
      debugPrint('Error getting notifications: $e');
      return [];
    }
  }

  Future<bool> sendNotification(String accessToken, String message) async {
    try {
      _dio.options.headers['Authorization'] = 'Bearer $accessToken';

      final response = await _dio.post(
        ApiConstants.notifications,
        data: json.encode(message),
      );

      return response.statusCode == 200;
    } on DioException catch (e) {
      debugPrint('Dio error sending notification: ${e.message}');
      return false;
    } catch (e) {
      debugPrint('Error sending notification: $e');
      return false;
    }
  }

  Future<bool> deleteNotification(String accessToken, String id) async {
    try {
      _dio.options.headers['Authorization'] = 'Bearer $accessToken';

      final response = await _dio.delete('${ApiConstants.notifications}/$id');

      if (response.statusCode == 204) {
        // Удаляем уведомление из локального списка
        final updatedList =
            currentNotifications
                .where((notification) => notification.id != id)
                .toList();
        _notificationsController.add(updatedList);
        return true;
      } else {
        return false;
      }
    } on DioException catch (e) {
      debugPrint('Dio error deleting notification: ${e.message}');
      return false;
    } catch (e) {
      debugPrint('Error deleting notification: $e');
      return false;
    }
  }

  // Получение цвета на основе типа уведомления
  Color _getNotificationColor(String type) {
    switch (type.toLowerCase()) {
      case 'success':
        return Colors.green;
      case 'error':
        return Colors.red;
      case 'warn':
        return Colors.orange;
      case 'info':
      default:
        return Colors.blue;
    }
  }

  // Освобождение ресурсов
  void dispose() {
    stopConnection();
    _notificationsController.close();
    _inAppNotificationController.close();
  }
}
