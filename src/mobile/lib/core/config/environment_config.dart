enum Environment { dev, prod }

class EnvironmentConfig {
  static Environment _environment = Environment.dev;
  static String ipAddress = "192.168.84.232";

  static void setEnvironment(Environment env) {
    _environment = env;
  }

  static Environment getEnvironment() {
    return _environment;
  }

  static bool isDevelopment() {
    return _environment == Environment.dev;
  }

  static bool isProduction() {
    return _environment == Environment.prod;
  }

  static String get userServiceBaseUrl {
    switch (_environment) {
      case Environment.dev:
        return 'http://10.0.2.2:7001';
      // return 'https://localhost:7001';
      case Environment.prod:
        return 'http://$ipAddress:7001';
    }
  }

  static String get movieServiceBaseUrl {
    switch (_environment) {
      case Environment.dev:
        return 'https://localhost:7002';
      case Environment.prod:
        return 'http://$ipAddress:7002';
    }
  }

  static String get bookingServiceBaseUrl {
    switch (_environment) {
      case Environment.dev:
        return 'https://localhost:7003';
      case Environment.prod:
        return 'http://$ipAddress:7003';
    }
  }
}
