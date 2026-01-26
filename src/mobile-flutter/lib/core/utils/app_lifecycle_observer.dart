// lib/core/utils/app_lifecycle_observer.dart
import 'package:flutter/material.dart';

class AppLifecycleObserver with WidgetsBindingObserver {
  Function(AppLifecycleState)? onStateChange;

  AppLifecycleObserver() {
    WidgetsBinding.instance.addObserver(this);
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    onStateChange?.call(state);
  }

  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
  }
}
