import 'package:flutter/material.dart';
import 'package:mobile/core/config/google_services_config.dart';
import 'package:provider/provider.dart';

import 'config/theme.dart';
import 'core/config/environment_config.dart';
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
  EnvironmentConfig.setEnvironment(Environment.dev);

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
    navigatorKey.currentState?.pushNamedAndRemoveUntil('/login', (route) => false);
  }
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    final themeProvider = Provider.of<ThemeProvider>(context);
    final authProvider = Provider.of<AuthProvider>(context);

    return MaterialApp(
      navigatorKey: navigatorKey, // Устанавливаем глобальный ключ навигатора
      title: 'Cinema App',
      theme: AppTheme.lightTheme,
      darkTheme: AppTheme.darkTheme,
      themeMode: themeProvider.themeMode,
      debugShowCheckedModeBanner: false,
      initialRoute: '/',
      routes: {
        '/': (context) => const SplashScreen(),
        '/login': (context) => const LoginScreen(),
        '/register': (context) => const RegisterScreen(),
        '/home': (context) => const HomeScreen(),
      },
    );
  }
}

// Простая реализация страницы-заглушки для Home экрана
class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final authProvider = Provider.of<AuthProvider>(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Cinema App'),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: () {
              authProvider.logout();
              Navigator.of(context).pushReplacementNamed('/login');
            },
          ),
        ],
      ),
      body: const Center(
        child: Text('Главный экран приложения', style: TextStyle(fontSize: 18)),
      ),
    );
  }
}

// Простая реализация страницы-заглушки для SplashScreen
class SplashScreen extends StatefulWidget {
  const SplashScreen({super.key});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen> {
  @override
  void initState() {
    super.initState();
    _checkAuthStatus();
  }

  Future<void> _checkAuthStatus() async {
    // Имитация загрузки
    await Future.delayed(const Duration(seconds: 2));

    if (mounted) {
      final authProvider = Provider.of<AuthProvider>(context, listen: false);
      await authProvider.checkAuthStatus();

      if (authProvider.authStatus == AuthStatus.authenticated) {
        Navigator.of(context).pushReplacementNamed('/home');
      } else {
        Navigator.of(context).pushReplacementNamed('/login');
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Text(
              'Cinema App',
              style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 24),
            CircularProgressIndicator(
              color: Theme.of(context).colorScheme.secondary,
            ),
          ],
        ),
      ),
    );
  }
}
