class ServerException implements Exception {
  final String message;

  const ServerException([this.message = 'Server error occurred']);

  @override
  String toString() => message;
}

class CacheException implements Exception {
  final String message;

  const CacheException([this.message = 'Cache error occurred']);

  @override
  String toString() => message;
}

class NoInternetException implements Exception {
  final String message;

  const NoInternetException([this.message = 'No internet connection']);

  @override
  String toString() => message;
}

class BadRequestException implements Exception {
  final String message;

  const BadRequestException([this.message = 'Bad request']);

  @override
  String toString() => message;
}

class UnauthorizedException implements Exception {
  final String message;

  const UnauthorizedException([this.message = 'Unauthorized']);

  @override
  String toString() => message;
}

class NotFoundException implements Exception {
  final String message;

  const NotFoundException([this.message = 'Resource not found']);

  @override
  String toString() => message;
}

class TimeoutException implements Exception {
  final String message;

  const TimeoutException([this.message = 'Connection timeout']);

  @override
  String toString() => message;
}

class AuthException implements Exception {
  final String message;

  const AuthException([this.message = 'Authentication error']);

  @override
  String toString() => message;
}

class GoogleSignInException implements Exception {
  final String message;

  const GoogleSignInException([this.message = 'Google sign in error']);

  @override
  String toString() => message;
}
