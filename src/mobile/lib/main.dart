import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:untitledCinema/presentation/providers/booking_provider.dart';
import 'package:untitledCinema/presentation/providers/booking_statistics_provider.dart';
import 'package:untitledCinema/presentation/providers/day_management_provider.dart';
import 'package:untitledCinema/presentation/providers/hall_managment_provider.dart';
import 'package:untitledCinema/presentation/providers/movie_management_provider.dart';
import 'package:untitledCinema/presentation/providers/movie_provider.dart';
import 'package:untitledCinema/presentation/providers/movie_statistics_provider.dart';
import 'package:untitledCinema/presentation/providers/session_provider.dart';
import 'package:untitledCinema/presentation/providers/user_management_provider.dart';
import 'package:untitledCinema/presentation/screens/booking_statistics_screen.dart';
import 'package:untitledCinema/presentation/screens/days_management_screen.dart';
import 'package:untitledCinema/presentation/screens/halls_management_screen.dart';
import 'package:untitledCinema/presentation/screens/history_screen.dart';
import 'package:untitledCinema/presentation/screens/movie_statistics_screen.dart';
import 'package:untitledCinema/presentation/screens/movies_management_screen.dart';
import 'package:untitledCinema/presentation/screens/navigation_screen.dart';
import 'package:untitledCinema/presentation/screens/splash_screen.dart';
import 'package:untitledCinema/presentation/screens/users_management_screen.dart';

import 'config/theme.dart';
import 'core/config/environment_config.dart';
import 'core/config/google_services_config.dart';
import 'core/constants/app_constants.dart';
import 'di/injection_container.dart' as di;
import 'domain/entities/custom_notification.dart';
import 'presentation/providers/auth_provider.dart';
import 'presentation/providers/theme_provider.dart';
import 'presentation/screens/auth/login_screen.dart';
import 'presentation/screens/auth/register_screen.dart';

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();
final GlobalKey<ScaffoldMessengerState> scaffoldMessengerKey =
    GlobalKey<ScaffoldMessengerState>();

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  EnvironmentConfig.setEnvironment(Environment.prod);
  // EnvironmentConfig.setEnvironment(Environment.dev);

  await di.init();

  GoogleServicesConfig.createGoogleSignIn();

  // Получаем NotificationManager и слушаем изменения аутентификации
  // final notificationManager = di.sl<NotificationManager>();
  final authProvider = di.sl<AuthProvider>();

  // Добавляем слушатель для инициализации уведомлений при авторизации
  // authProvider.addListener(() {
  //   if (authProvider.authStatus == AuthStatus.authenticated) {
  //     final token = authProvider.token;
  //     if (token != null && token.isNotEmpty) {
  //       notificationManager.initialize(token);
  //     }
  //   }
  // });

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => di.sl<ThemeProvider>()),
        ChangeNotifierProvider(
          create: (_) {
            // Добавляем слушатель для отслеживания изменений статуса аутентификации
            authProvider.addListener(() {
              // Если статус изменился на 'не аутентифицирован' и есть сообщение об ошибке
              if (authProvider.authStatus == AuthStatus.unauthenticated &&
                  authProvider.errorMessage != null &&
                  authProvider.errorMessage!.contains('сессия истекла')) {
                // Показываем сообщение и перенаправляем на экран входа
                _handleSessionExpired(authProvider.errorMessage!);
              }
            });
            return authProvider;
          },
        ),
        ChangeNotifierProvider(create: (_) => di.sl<MovieProvider>()),
        ChangeNotifierProvider(create: (_) => di.sl<SessionProvider>()),
        ChangeNotifierProvider(create: (_) => di.sl<BookingProvider>()),
        ChangeNotifierProvider(create: (_) => di.sl<UserManagementProvider>()),
        ChangeNotifierProvider(create: (_) => di.sl<DayManagementProvider>()),
        ChangeNotifierProvider(create: (_) => di.sl<HallManagementProvider>()),
        ChangeNotifierProvider(create: (_) => di.sl<MovieManagementProvider>()),
        ChangeNotifierProvider(
          create: (_) => di.sl<BookingStatisticsProvider>(),
        ),
        ChangeNotifierProvider(
          create: (_) => di.sl<MovieStatisticsProvider>(),
        ),
        // ChangeNotifierProvider(create: (_) => di.sl<NotificationProvider>()),
      ],
      child: const MyApp(),
    ),
  );
}

// Обработка истечения сессии - показывает сообщение и перенаправляет на экран входа
void _handleSessionExpired(String message) {
  final context = navigatorKey.currentContext;
  if (context != null) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red,
        duration: const Duration(seconds: 5),
      ),
    );

    navigatorKey.currentState?.pushNamedAndRemoveUntil(
      '/login',
      (route) => false,
    );
  }
}

class MyApp extends StatefulWidget {
  const MyApp({super.key});

  @override
  State<MyApp> createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  @override
  void initState() {
    super.initState();

    // Подписываемся на уведомления
    // Future.delayed(Duration.zero, () {
    //   final notificationProvider = Provider.of<NotificationProvider>(
    //     context,
    //     listen: false,
    //   );
    //
    //   // Слушаем in-app уведомления для отображения SnackBar
    //   notificationProvider.inAppNotifications.listen(_showNotificationSnackBar);
    // });
  }

  void _showNotificationSnackBar(CustomNotification notification) {
    Color backgroundColor;

    switch (notification.type.toLowerCase()) {
      case 'success':
        backgroundColor = Colors.green;
        break;
      case 'error':
        backgroundColor = Colors.red;
        break;
      case 'warn':
        backgroundColor = Colors.orange;
        break;
      case 'info':
      default:
        backgroundColor = Colors.blue;
        break;
    }

    scaffoldMessengerKey.currentState?.showSnackBar(
      SnackBar(
        content: Text(notification.message),
        backgroundColor: backgroundColor,
        duration: const Duration(seconds: 5),
        action: SnackBarAction(
          label: 'Закрыть',
          textColor: Colors.white,
          onPressed: () {
            scaffoldMessengerKey.currentState?.hideCurrentSnackBar();
          },
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final themeProvider = Provider.of<ThemeProvider>(context);

    return MaterialApp(
      navigatorKey: navigatorKey,
      scaffoldMessengerKey: scaffoldMessengerKey,
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
        '/history': (context) => const HistoryScreen(),
        '/users_management': (context) => const UsersManagementScreen(),
        '/days_management': (context) => const DaysManagementScreen(),
        '/halls_management': (context) => const HallsManagementScreen(),
        '/movies_management': (context) => const MoviesManagementScreen(),
        '/booking_management': (context) => const BookingStatisticsScreen(),
        '/movie_statistics': (context) => const MovieStatisticsScreen(),
      },
    );
  }
}
