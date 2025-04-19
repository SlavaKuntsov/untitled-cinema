import 'package:flutter/foundation.dart';
import 'package:google_sign_in/google_sign_in.dart';

import '../constants/oauth_constants.dart';

/// Конфигурация для Google Services, адаптированная для различных сред
class GoogleServicesConfig {
  /// Создает экземпляр GoogleSignIn с подходящими настройками
  /// в зависимости от среды исполнения
  static GoogleSignIn createGoogleSignIn({
    List<String> scopes = const ['email', 'profile', 'openid'],
    String? clientId,
  }) {
    // ВАЖНО: для получения idToken в Android необходимо указать serverClientId
    final serverClientId = GoogleOAuthConstants.WEB_CLIENT_ID;

    if (kIsWeb) {
      // Web требует явного clientId
      return GoogleSignIn(scopes: scopes, clientId: clientId);
    } else if (defaultTargetPlatform == TargetPlatform.android) {
      // На Android необходимо указывать serverClientId для получения idToken
      return GoogleSignIn(
        scopes: scopes,
        serverClientId: serverClientId, // Это критически важный параметр!
      );
    } else if (defaultTargetPlatform == TargetPlatform.iOS) {
      // iOS использует serverClientId и не требует clientId
      return GoogleSignIn(scopes: scopes, serverClientId: serverClientId);
    } else {
      // Для других платформ
      return GoogleSignIn(scopes: scopes, clientId: clientId);
    }
  }
}
