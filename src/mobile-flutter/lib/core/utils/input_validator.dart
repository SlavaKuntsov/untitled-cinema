class InputValidator {
  // Валидация email
  static String? validateEmail(String? value) {
    if (value == null || value.isEmpty) {
      return 'Email обязателен для заполнения';
    }

    final RegExp emailRegExp = RegExp(
      r'^[a-zA-Z0-9.]+@[a-zA-Z0-9]+\.[a-zA-Z]+',
    );

    if (!emailRegExp.hasMatch(value)) {
      return 'Пожалуйста, введите корректный email';
    }

    return null;
  }

  // Валидация пароля
  static String? validatePassword(String? value) {
    if (value == null || value.isEmpty) {
      return 'Пароль обязателен для заполнения';
    }

    if (value.length < 6) {
      return 'Пароль должен содержать минимум 6 символов';
    }

    return null;
  }

  // Валидация имени
  static String? validateName(String? value) {
    if (value == null || value.isEmpty) {
      return 'Имя обязательно для заполнения';
    }

    if (value.length < 2) {
      return 'Имя должно содержать минимум 2 символа';
    }

    return null;
  }

  // Подтверждение пароля
  static String? validateConfirmPassword(String? value, String password) {
    if (value == null || value.isEmpty) {
      return 'Необходимо подтвердить пароль';
    }

    if (value != password) {
      return 'Пароли не совпадают';
    }

    return null;
  }

  // Общая валидация обязательных полей
  static String? validateRequired(String? value, String fieldName) {
    if (value == null || value.isEmpty) {
      return '$fieldName обязателен для заполнения';
    }

    return null;
  }
}
