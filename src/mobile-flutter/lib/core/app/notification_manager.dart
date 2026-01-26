// // lib/core/app/notification_manager.dart
// import 'package:flutter/material.dart';
//
// import '../services/notification_service.dart';
// import '../utils/app_lifecycle_observer.dart';
//
// class NotificationManager {
//   final NotificationService _notificationService;
//   final AppLifecycleObserver _lifecycleObserver;
//
//   NotificationManager({required NotificationService notificationService})
//     : _notificationService = notificationService,
//       _lifecycleObserver = AppLifecycleObserver() {
//     _lifecycleObserver.onStateChange = _handleAppLifecycleState;
//   }
//
//   void initialize(String token) {
//     _notificationService.initialize(token);
//   }
//
//   void _handleAppLifecycleState(AppLifecycleState state) {
//     switch (state) {
//       case AppLifecycleState.resumed:
//         _notificationService.setAppStatus(true);
//         break;
//       case AppLifecycleState.paused:
//       case AppLifecycleState.inactive:
//       case AppLifecycleState.detached:
//       case AppLifecycleState.hidden:
//         _notificationService.setAppStatus(false);
//         break;
//     }
//   }
//
//   void dispose() {
//     _lifecycleObserver.dispose();
//   }
// }
