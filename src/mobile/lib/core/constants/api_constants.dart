import '../config/environment_config.dart';

class ApiConstants {
  // User Service Endpoints
  static String get userServiceBaseUrl => EnvironmentConfig.userServiceBaseUrl;

  // Auth endpoints в User Service
  static String get login => '$userServiceBaseUrl/users/login';
  static String get users => '$userServiceBaseUrl/users';
  static String get delete => '$userServiceBaseUrl/users/me';
  static String get register => '$userServiceBaseUrl/users/registration';
  static String get googleAuth => '$userServiceBaseUrl/auth/google-login';
  static String get googleResponse =>
      '$userServiceBaseUrl/auth/google-response';
  static String get googleMobileAuth =>
      '$userServiceBaseUrl/auth/google-mobile-auth';
  static String get logout => '$userServiceBaseUrl/auth/unauthorize';
  static String get refreshToken => '$userServiceBaseUrl/auth/refreshToken';
  static String get authorize => '$userServiceBaseUrl/auth/authorize';
  static String get notifications => '$userServiceBaseUrl/notifications';
  static String get notificationsHub => '$userServiceBaseUrl/notificationsHub';

  // Admin API endpoints
  static String getAllUsers() => '$userServiceBaseUrl/users';
  static String deleteUser(String userId) =>
      '$userServiceBaseUrl/users/$userId';

  // Movie Service Endpoints
  static String get movieServiceBaseUrl =>
      EnvironmentConfig.movieServiceBaseUrl;

  // Movie endpoints
  static String get movies => '$movieServiceBaseUrl/movies';
  static String get movieDetails => '$movieServiceBaseUrl/movies';
  static String get movieGenres => '$movieServiceBaseUrl/movies/genres';
  static String get nowPlaying => '$movieServiceBaseUrl/movies/now-playing';
  static String get upcoming => '$movieServiceBaseUrl/movies/upcoming';
  static String get search => '$movieServiceBaseUrl/movies/search';
  static String get moviePoster => '$movieServiceBaseUrl/movies/poster';
  static String get movieFrame => '$movieServiceBaseUrl/movies/frames';
  static String get sessions => '$movieServiceBaseUrl/sessions';
  static String get halls => '$movieServiceBaseUrl/halls';
  static String get seatsType => '$movieServiceBaseUrl/seats/types';
  static String get seats => '$movieServiceBaseUrl/seats';

  // Booking Service Endpoints
  static String get bookingServiceBaseUrl =>
      EnvironmentConfig.bookingServiceBaseUrl;

  // Booking endpoints
  static String get bookings => '$bookingServiceBaseUrl/bookings';
  static String get cancel => '$bookingServiceBaseUrl/bookings/cancel';
  static String get pay => '$bookingServiceBaseUrl/bookings/pay';
  static String get bookingsHistory =>
      '$bookingServiceBaseUrl/bookings/history';
  static String get reservedSeats =>
      '$bookingServiceBaseUrl/bookingsSeats/reserved/session';
  static String get createBooking => '$bookingServiceBaseUrl/bookings/create';

  // API Keys и прочие константы
  static const String googleClientId =
      '613641131431-k6tqdavhgcfqvkqi1aeo347il4g20boi.apps.googleusercontent.com';
  static const String contentType = 'application/json';
  static const String authorization = 'Authorization';
  static const String bearer = 'Bearer';

  // JWT константы
  static const String REFRESH_COOKIE_NAME = "yummy-cackes";

  // Day management API endpoints
  static String getAllDays() => '$movieServiceBaseUrl/days';
  static String createDay() => '$movieServiceBaseUrl/days';
  static String deleteDay(String dayId) => '$movieServiceBaseUrl/days/$dayId';

  static String getAllHalls() => '$movieServiceBaseUrl/halls';
  static String getHallById(String id) => '$movieServiceBaseUrl/halls/$id';
  static String createSimpleHall() => '$movieServiceBaseUrl/halls/simple';
  static String createCustomHall() => '$movieServiceBaseUrl/halls/custom';
  static String updateHall() => '$movieServiceBaseUrl/halls';
  static String deleteHall(String id) => '$movieServiceBaseUrl/halls/$id';

  static String getMovieById(String id) => '$movieServiceBaseUrl/movies/$id';
  static String createMovie() => '$movieServiceBaseUrl/movies';
  static String updateMovie() => '$movieServiceBaseUrl/movies';
  static String deleteMovie(String id) => '$movieServiceBaseUrl/movies/$id';

  static String getMoviePoster(String id) =>
      '$movieServiceBaseUrl/movies/$id/poster';
  static String changeMoviePoster(String id) =>
      '$movieServiceBaseUrl/movies/$id/poster';
  static String getMoviePosterByName(String fileName) =>
      '$movieServiceBaseUrl/movies/poster/$fileName';

  // Movie frames endpoints
  static String getAllMovieFrames() => '$movieServiceBaseUrl/movies/frames';
  static String getMovieFramesByMovieId(String movieId) =>
      '$movieServiceBaseUrl/movies/$movieId/frames';
  static String getMovieFrameByName(String fileName) =>
      '$movieServiceBaseUrl/movies/frames/$fileName';
  static String addMovieFrame(String movieId, int frameOrder) =>
      '$movieServiceBaseUrl/movies/$movieId/frames/$frameOrder';
  static String deleteMovieFrame(String frameId) =>
      '$movieServiceBaseUrl/movies/frames/$frameId';

  // Genre endpoints
  static String getAllGenres() => '$movieServiceBaseUrl/movies/genres';
  static String updateGenre() => '$movieServiceBaseUrl/movies/genres';
  static String deleteGenre(String id) =>
      '$movieServiceBaseUrl/movies/genres/$id';

  // Session endpoints
  static String fetchSeatTypesByHallId(String hallId) =>
      '$movieServiceBaseUrl/sessions/halls/$hallId/seat-types';
}

// Константы JWT
class JwtConstants {
  static const String REFRESH_COOKIE_NAME = "yummy-cackes";
}
