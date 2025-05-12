import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:untitledCinema/presentation/providers/movie_provider.dart';
import 'package:untitledCinema/presentation/providers/notification_provider.dart';
import 'package:untitledCinema/presentation/providers/session_provider.dart';
import 'package:untitledCinema/presentation/screens/navigation_screen.dart';
import 'package:untitledCinema/presentation/screens/splash_screen.dart';

import 'config/theme.dart';
import 'core/config/environment_config.dart';
import 'core/config/google_services_config.dart';
import 'core/constants/app_constants.dart';
import 'di/injection_container.dart' as di;
import 'presentation/providers/auth_provider.dart';
import 'presentation/providers/theme_provider.dart';
import 'presentation/screens/auth/login_screen.dart';
import 'presentation/screens/auth/register_screen.dart';

// Глобальный ключ для доступа к навигатору из любого места приложения
final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Устанавливаем окружение (dev для разработки)
  EnvironmentConfig.setEnvironment(Environment.prod);
  // EnvironmentConfig.setEnvironment(Environment.dev);

  // Инициализируем зависимости
  await di.init();

  GoogleServicesConfig.createGoogleSignIn();

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => di.sl<ThemeProvider>()),
        ChangeNotifierProvider(
          create: (_) {
            final provider = di.sl<AuthProvider>();
            // Добавляем слушатель для отслеживания изменений статуса аутентификации
            provider.addListener(() {
              // Если статус изменился на 'не аутентифицирован' и есть сообщение об ошибке
              if (provider.authStatus == AuthStatus.unauthenticated &&
                  provider.errorMessage != null &&
                  provider.errorMessage!.contains('сессия истекла')) {
                // Показываем сообщение и перенаправляем на экран входа
                _handleSessionExpired(provider.errorMessage!);
              }
            });
            return provider;
          },
        ),
        ChangeNotifierProvider(create: (_) => di.sl<MovieProvider>()),
        ChangeNotifierProvider(create: (_) => di.sl<SessionProvider>()),
        ChangeNotifierProvider(create: (_) => di.sl<NotificationProvider>()),
      ],
      child: const MyApp(),
    ),
  );
}

// Обработка истечения сессии - показывает сообщение и перенаправляет на экран входа
void _handleSessionExpired(String message) {
  final context = navigatorKey.currentContext;
  if (context != null) {
    // Показываем сообщение
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red,
        duration: const Duration(seconds: 5),
      ),
    );

    // Перенаправляем на экран входа
    navigatorKey.currentState?.pushNamedAndRemoveUntil(
      '/login',
      (route) => false,
    );
  }
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    final themeProvider = Provider.of<ThemeProvider>(context);

    return MaterialApp(
      navigatorKey: navigatorKey, // Устанавливаем глобальный ключ навигатора
      title: AppConstants.appName,
      theme: AppTheme.lightTheme,
      darkTheme: AppTheme.darkTheme,
      themeMode: themeProvider.themeMode,
      debugShowCheckedModeBanner: false,
      initialRoute: '/',
      routes: {
        '/': (context) => const SplashScreen(),
        '/login': (context) => const LoginScreen(),
        '/register': (context) => const RegisterScreen(),
        '/home': (context) => const NavigationScreen(),
      },
    );
  }
}
