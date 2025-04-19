import 'dart:io';

import 'package:flutter/material.dart';
import 'package:package_info_plus/package_info_plus.dart';

class DebugTools {
  static Future<String> getAppInfo() async {
    try {
      final PackageInfo packageInfo = await PackageInfo.fromPlatform();
      final String packageName = packageInfo.packageName;
      final String appName = packageInfo.appName;
      final String version = packageInfo.version;
      final String buildNumber = packageInfo.buildNumber;

      return '''
App Info:
  App Name: $appName
  Package Name: $packageName
  Version: $version
  Build Number: $buildNumber
''';
    } catch (e) {
      return 'Error getting app info: $e';
    }
  }

  static void printSha1Instructions() {
    debugPrint('''
=== КАК ПОЛУЧИТЬ SHA-1 ОТПЕЧАТОК ===

Для Windows:
1. Найдите файл debug.keystore (обычно в %USERPROFILE%\\.android\\debug.keystore)
2. Запустите команду:
   keytool -list -v -keystore "%USERPROFILE%\\.android\\debug.keystore" -alias androiddebugkey -storepass android -keypass android

Для macOS/Linux:
1. Найдите файл debug.keystore (обычно в ~/.android/debug.keystore)
2. Запустите команду:
   keytool -list -v -keystore ~/.android/debug.keystore -alias androiddebugkey -storepass android -keypass android

В выводе команды найдите строку "SHA1: XX:XX:XX:XX:..." - это и есть ваш SHA-1 отпечаток.

=== ИНСТРУКЦИИ ДЛЯ GOOGLE CLOUD CONSOLE ===
1. Перейдите на https://console.cloud.google.com/
2. Выберите ваш проект (software-security-chat)
3. Перейдите в "Credentials" (Учетные данные)
4. Найдите ваш OAuth client ID для Android
5. Добавьте новый SHA-1 сертификат, если текущий не совпадает
6. Проверьте, что package name совпадает с вашим (${Platform.isAndroid ? 'используйте команду выше' : 'N/A для этой платформы'})
''');
  }

  static Widget buildDebugInfoCard() {
    return FutureBuilder<String>(
      future: getAppInfo(),
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Card(
            child: Padding(
              padding: EdgeInsets.all(16.0),
              child: CircularProgressIndicator(),
            ),
          );
        }

        return Card(
          margin: const EdgeInsets.all(16.0),
          child: Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Отладочная информация',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 8),
                Text(snapshot.data ?? 'Загрузка информации...'),
                const SizedBox(height: 16),
                const Text(
                  'Возможные причины ошибки Google Sign-In:',
                  style: TextStyle(fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 8),
                const Text(
                  '1. Неправильный SHA-1 отпечаток в Google Cloud Console',
                ),
                const Text('2. Несовпадение Package Name в конфигурации'),
                const Text('3. Не включен Google Sign-In API в консоли'),
                const Text('4. Проблемы с OAuth consent screen'),
                const SizedBox(height: 16),
                ElevatedButton(
                  onPressed: () {
                    printSha1Instructions();
                  },
                  child: const Text('Вывести инструкции в консоль'),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
