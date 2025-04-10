/// Константы для Google OAuth 2.0 авторизации
class GoogleOAuthConstants {
  /// Client ID для мобильного приложения
  static const String CLIENT_ID =
      "613641131431-k6tqdavhgcfqvkqi1aeo347il4g20boi.apps.googleusercontent.com";

  /// Project ID проекта в Google Cloud Console
  static const String PROJECT_ID = "software-security-chat";

  /// URI для авторизации пользователя
  static const String AUTH_URI = "https://accounts.google.com/o/oauth2/auth";

  /// URI для обмена кода на токены
  static const String TOKEN_URI = "https://oauth2.googleapis.com/token";

  /// URL для проверки сертификатов
  static const String AUTH_PROVIDER_CERT_URL =
      "https://www.googleapis.com/oauth2/v1/certs";

  /// Список разрешений для авторизации
  static const List<String> SCOPES = ['email', 'profile', 'openid'];
}
