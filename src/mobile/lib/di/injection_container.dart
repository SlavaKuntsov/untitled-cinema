import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:dio/dio.dart';
import 'package:get_it/get_it.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:untitledCinema/data/datasources/movies_remote_data_source.dart';
import 'package:untitledCinema/data/datasources/session_remote_data_source.dart';
import 'package:untitledCinema/data/repositories/sessions_repository_impl.dart';
import 'package:untitledCinema/domain/repositories/sessions_repository.dart';
import 'package:untitledCinema/presentation/providers/booking_provider.dart';
import 'package:untitledCinema/presentation/providers/booking_statistics_provider.dart';
import 'package:untitledCinema/presentation/providers/day_management_provider.dart';
import 'package:untitledCinema/presentation/providers/hall_managment_provider.dart';
import 'package:untitledCinema/presentation/providers/movie_management_provider.dart';
import 'package:untitledCinema/presentation/providers/movie_provider.dart';
import 'package:untitledCinema/presentation/providers/session_provider.dart';
import 'package:untitledCinema/presentation/providers/user_management_provider.dart';

import '../core/constants/oauth_constants.dart';
import '../core/network/api_client.dart';
import '../core/network/network_info.dart';
import '../core/services/signalr_service.dart';
import '../data/datasources/auth/auth_remote_data_source.dart';
import '../data/datasources/auth/google_auth_service.dart';
import '../data/datasources/booking_remote_data_source.dart';
import '../data/repositories/auth_repository_impl.dart';
import '../data/repositories/booking_repository_impl.dart';
import '../data/repositories/movies_repository_impl.dart';
import '../domain/repositories/auth_repository.dart';
import '../domain/repositories/booking_repository.dart';
import '../domain/repositories/movies_repository.dart';
import '../domain/usecases/auth/google_sign_in.dart';
import '../domain/usecases/auth/login.dart';
import '../domain/usecases/auth/registration.dart';
import '../presentation/providers/auth_provider.dart';
import '../presentation/providers/theme_provider.dart';

final sl = GetIt.instance;

Future<void> init() async {
  // Providers
  sl.registerFactory(
    () => AuthProvider(
      repository: sl(),
      loginUseCase: sl(),
      registrationUseCase: sl(),
      googleSignInUseCase: sl(),
      googleSignIn: sl(),
      prefs: sl(),
    ),
  );
  sl.registerFactory(() => MovieProvider(repository: sl()));
  sl.registerFactory(() => SessionProvider(repository: sl()));
  sl.registerFactory(() => BookingProvider(repository: sl(), prefs: sl()));
  sl.registerFactory(() => UserManagementProvider(apiClient: sl()));
  sl.registerFactory(() => DayManagementProvider(apiClient: sl()));
  sl.registerFactory(() => HallManagementProvider(apiClient: sl()));
  sl.registerFactory(() => MovieManagementProvider(apiClient: sl()));
  sl.registerFactory(() => BookingStatisticsProvider());
  sl.registerFactory(() => ThemeProvider());

  // Notification Provider с внедрением зависимости
  // sl.registerFactory(() => NotificationProvider(notificationService: sl()));

  // Notification Service
  // sl.registerLazySingleton<NotificationService>(
  //   () => NotificationService(
  //     client: sl<Dio>(),
  //     signalRService: sl<SignalRService>(),
  //   ),
  // );

  // Notification Manager
  // sl.registerLazySingleton(
  //   () => NotificationManager(notificationService: sl<NotificationService>()),
  // );

  // Use cases
  sl.registerLazySingleton(() => LoginUseCase(sl()));
  sl.registerLazySingleton(() => RegistrationUseCase(sl()));
  sl.registerLazySingleton(() => GoogleSignInUseCase(sl()));

  sl.registerLazySingleton(
    () => GoogleAuthService(googleSignIn: sl(), prefs: sl()),
  );

  sl.registerLazySingleton(() {
    final prefs = sl<SharedPreferences>();
    final accessToken = prefs.getString('access_token');

    return SignalRService(accessToken: accessToken);
  });

  // Repository
  sl.registerLazySingleton<AuthRepository>(
    () => AuthRepositoryImpl(
      remoteDataSource: sl(),
      networkInfo: sl(),
      googleSignIn: sl(),
    ),
  );
  //Repository
  sl.registerLazySingleton<MovieRepository>(
    () => MovieRepositoryImpl(remoteDataSource: sl()),
  );
  // Repository
  sl.registerLazySingleton<BookingRepository>(
    () => BookingRepositoryImpl(remoteDataSource: sl(), networkInfo: sl()),
  );
  //Repository
  sl.registerLazySingleton<SessionRepository>(
    () => SessionRepositoryImpl(remoteDataSource: sl()),
  );

  // Data sources
  sl.registerLazySingleton<AuthRemoteDataSource>(
    () =>
        AuthRemoteDataSourceImpl(client: sl(), googleSignIn: sl(), prefs: sl()),
  );
  // Data sources
  sl.registerLazySingleton<MovieRemoteDataSource>(
    () => MovieRemoteDataSourceImpl(client: sl()),
  );
  // Data sources
  sl.registerLazySingleton<SessionRemoteDataSource>(
    () => SessionRemoteDataSourceImpl(client: sl(), signalRService: sl()),
  );
  // Data sources
  sl.registerLazySingleton<BookingRemoteDataSource>(
    () => BookingRemoteDataSourceImpl(client: sl()),
  );

  // Core
  sl.registerLazySingleton<NetworkInfo>(() => NetworkInfoImpl(sl()));

  sl.registerLazySingleton<ApiClient>(
    () => ApiClient(
      sl<Dio>(),
      sl<SharedPreferences>(),
      onLogoutRequired: () {
        if (sl.isRegistered<AuthProvider>()) {
          sl<AuthProvider>().forceLogout();
        }
      },
    ),
  );

  // External
  final sharedPreferences = await SharedPreferences.getInstance();
  sl.registerLazySingleton(() => sharedPreferences);

  sl.registerLazySingleton(() => Dio());

  sl.registerLazySingleton(() => Connectivity());

  sl.registerLazySingleton(
    () => GoogleSignIn(
      scopes: ['email', 'profile', 'openid'],
      serverClientId: GoogleOAuthConstants.WEB_CLIENT_ID,
    ),
  );
}
