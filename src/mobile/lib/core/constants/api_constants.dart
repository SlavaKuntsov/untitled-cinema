import '../config/environment_config.dart';

class ApiConstants {
  // User Service Endpoints
  static String get userServiceBaseUrl => EnvironmentConfig.userServiceBaseUrl;

  // Auth endpoints в User Service
  static String get login => '$userServiceBaseUrl/users/login';
  static String get register => '$userServiceBaseUrl/users/registration';
  static String get googleAuth => '$userServiceBaseUrl/auth/google-login';
  static String get googleResponse =>
      '$userServiceBaseUrl/auth/google-response';
  static String get googleMobileAuth =>
      '$userServiceBaseUrl/auth/google-mobile-auth';
  static String get logout => '$userServiceBaseUrl/auth/unauthorize';
  static String get refreshToken => '$userServiceBaseUrl/auth/refresh-token';
  static String get authorize => '$userServiceBaseUrl/auth/authorize';

  // Movie Service Endpoints
  static String get movieServiceBaseUrl =>
      EnvironmentConfig.movieServiceBaseUrl;

  // Movie endpoints
  static String get movies => '$movieServiceBaseUrl/movies';
  static String get movieDetails => '$movieServiceBaseUrl/movies';
  static String get nowPlaying => '$movieServiceBaseUrl/movies/now-playing';
  static String get upcoming => '$movieServiceBaseUrl/movies/upcoming';
  static String get search => '$movieServiceBaseUrl/movies/search';

  // Booking Service Endpoints
  static String get bookingServiceBaseUrl =>
      EnvironmentConfig.bookingServiceBaseUrl;

  // Booking endpoints
  static String get bookings => '$bookingServiceBaseUrl/bookings';
  static String get createBooking => '$bookingServiceBaseUrl/bookings/create';

  // API Keys и прочие константы
  static const String googleClientId =
      '613641131431-k6tqdavhgcfqvkqi1aeo347il4g20boi.apps.googleusercontent.com';
  static const String contentType = 'application/json';
  static const String authorization = 'Authorization';
  static const String bearer = 'Bearer';
}

// Константы JWT
class JwtConstants {
  static const String REFRESH_COOKIE_NAME = "yummy-cackes";
}
