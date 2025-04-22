import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

class AppTheme {
  static const Color primaryColor = Color(0xFF35353B);
  static const Color accentColor = Color(0xFF4F46E5);
  static const Color backgroundColor = Color(0xFF18181B);
  static const Color cardColor = Color(0xFF272A31);
  static const Color textPrimaryColor = Color(0xFFFFFFFF);
  static const Color textSecondaryColor = Color(0xFFB8B8B8);
  static const Color errorColor = Color(0xFFDC2626);

  // Цвета для светлой темы
  static const Color lightTextPrimaryColor = Color(
    0xFF35353B,
  ); // как primaryColor
  static const Color lightTextSecondaryColor = Color(0xFF71717A); // серый
  static const Color lightHeadingColor = Color(
    0xFF27272A,
  ); // темно-серый для заголовков

  static TextTheme _createLightTextTheme() {
    return TextTheme(
      // Заголовки
      displayLarge: GoogleFonts.comfortaa(
        fontSize: 32,
        fontWeight: FontWeight.bold,
        color: lightHeadingColor,
      ),
      displayMedium: GoogleFonts.comfortaa(
        fontSize: 28,
        fontWeight: FontWeight.bold,
        color: lightHeadingColor,
      ),
      displaySmall: GoogleFonts.comfortaa(
        fontSize: 24,
        fontWeight: FontWeight.bold,
        color: lightHeadingColor,
      ),

      // Тело текста
      bodyLarge: GoogleFonts.comfortaa(
        fontSize: 16,
        color: lightTextPrimaryColor,
      ),
      bodyMedium: GoogleFonts.comfortaa(
        fontSize: 14,
        color: lightTextPrimaryColor,
      ),
      bodySmall: GoogleFonts.comfortaa(
        fontSize: 12,
        color: lightTextSecondaryColor,
      ),

      // Заголовки уровней
      headlineLarge: GoogleFonts.comfortaa(
        fontSize: 22,
        fontWeight: FontWeight.bold,
        color: lightHeadingColor,
      ),
      headlineMedium: GoogleFonts.comfortaa(
        fontSize: 20,
        fontWeight: FontWeight.bold,
        color: lightHeadingColor,
      ),
      headlineSmall: GoogleFonts.comfortaa(
        fontSize: 18,
        fontWeight: FontWeight.w600,
        color: lightHeadingColor,
      ),

      // Подписи, маркеры и кнопки
      labelLarge: GoogleFonts.comfortaa(
        fontSize: 14,
        fontWeight: FontWeight.w500,
        letterSpacing: 1.25,
        color: accentColor,
      ),
      labelMedium: GoogleFonts.comfortaa(
        fontSize: 12,
        letterSpacing: 0.4,
        color: lightTextSecondaryColor,
      ),
      labelSmall: GoogleFonts.comfortaa(
        fontSize: 10,
        letterSpacing: 0.4,
        color: lightTextSecondaryColor,
      ),
    );
  }

  static TextTheme _createDarkTextTheme() {
    return TextTheme(
      // Заголовки
      displayLarge: GoogleFonts.comfortaa(
        fontSize: 32,
        fontWeight: FontWeight.bold,
        color: textPrimaryColor,
      ),
      displayMedium: GoogleFonts.comfortaa(
        fontSize: 28,
        fontWeight: FontWeight.bold,
        color: textPrimaryColor,
      ),
      displaySmall: GoogleFonts.comfortaa(
        fontSize: 24,
        fontWeight: FontWeight.bold,
        color: textPrimaryColor,
      ),

      // Тело текста
      bodyLarge: GoogleFonts.comfortaa(fontSize: 16, color: textPrimaryColor),
      bodyMedium: GoogleFonts.comfortaa(fontSize: 14, color: textPrimaryColor),
      bodySmall: GoogleFonts.comfortaa(fontSize: 12, color: textSecondaryColor),

      // Заголовки уровней
      headlineLarge: GoogleFonts.comfortaa(
        fontSize: 22,
        fontWeight: FontWeight.bold,
        color: textPrimaryColor,
      ),
      headlineMedium: GoogleFonts.comfortaa(
        fontSize: 20,
        fontWeight: FontWeight.bold,
        color: textPrimaryColor,
      ),
      headlineSmall: GoogleFonts.comfortaa(
        fontSize: 18,
        fontWeight: FontWeight.w600,
        color: textPrimaryColor,
      ),

      // Подписи, маркеры и кнопки
      labelLarge: GoogleFonts.comfortaa(
        fontSize: 14,
        fontWeight: FontWeight.w500,
        letterSpacing: 1.25,
        color: accentColor,
      ),
      labelMedium: GoogleFonts.comfortaa(
        fontSize: 12,
        letterSpacing: 0.4,
        color: textSecondaryColor,
      ),
      labelSmall: GoogleFonts.comfortaa(
        fontSize: 10,
        letterSpacing: 0.4,
        color: textSecondaryColor,
      ),
    );
  }

  static final ThemeData lightTheme = ThemeData(
    textTheme: _createLightTextTheme(),
    brightness: Brightness.light,
    primaryColor: primaryColor,
    scaffoldBackgroundColor: Colors.white,
    colorScheme: const ColorScheme.light(
      primary: primaryColor,
      secondary: accentColor,
      error: errorColor,
      onBackground: lightTextPrimaryColor,
      onSurface: lightTextPrimaryColor,
    ),
    // Остальные настройки без изменений...
    appBarTheme: const AppBarTheme(
      backgroundColor: Colors.white,
      foregroundColor: primaryColor,
      elevation: 0,
    ),
    cardTheme: CardTheme(
      color: Colors.white,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      elevation: 2,
    ),
    inputDecorationTheme: InputDecorationTheme(
      filled: true,
      fillColor: Colors.grey[100],
      border: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: BorderSide.none,
      ),
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: BorderSide.none,
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: const BorderSide(color: accentColor, width: 1),
      ),
      errorBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: const BorderSide(color: errorColor, width: 1),
      ),
      contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
    ),
    elevatedButtonTheme: ElevatedButtonThemeData(
      style: ElevatedButton.styleFrom(
        backgroundColor: accentColor,
        foregroundColor: Colors.white,
        minimumSize: const Size(double.infinity, 56),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
    ),
    textButtonTheme: TextButtonThemeData(
      style: TextButton.styleFrom(foregroundColor: accentColor),
    ),
  );

  static final ThemeData darkTheme = ThemeData(
    textTheme: _createDarkTextTheme(),
    brightness: Brightness.dark,
    primaryColor: primaryColor,
    scaffoldBackgroundColor: backgroundColor,
    colorScheme: const ColorScheme.dark(
      primary: primaryColor,
      secondary: accentColor,
      error: errorColor,
      background: backgroundColor,
      surface: cardColor,
      onBackground: textPrimaryColor,
      onSurface: textPrimaryColor,
    ),
    // Остальные настройки без изменений...
    appBarTheme: const AppBarTheme(
      backgroundColor: backgroundColor,
      foregroundColor: textPrimaryColor,
      elevation: 0,
    ),
    cardTheme: CardTheme(
      color: cardColor,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      elevation: 0,
    ),
    inputDecorationTheme: InputDecorationTheme(
      filled: true,
      fillColor: cardColor,
      border: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: BorderSide.none,
      ),
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: BorderSide.none,
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: const BorderSide(color: accentColor, width: 1),
      ),
      errorBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: const BorderSide(color: errorColor, width: 1),
      ),
      hintStyle: const TextStyle(color: textSecondaryColor),
      contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
    ),
    elevatedButtonTheme: ElevatedButtonThemeData(
      style: ElevatedButton.styleFrom(
        backgroundColor: accentColor,
        foregroundColor: Colors.white,
        minimumSize: const Size(double.infinity, 56),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
    ),
    textButtonTheme: TextButtonThemeData(
      style: TextButton.styleFrom(foregroundColor: accentColor),
    ),
  );
}
