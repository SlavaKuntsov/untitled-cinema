import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../../core/utils/input_validator.dart';
import '../../providers/auth_provider.dart';
import '../../widgets/auth/auth_button.dart';
import '../../widgets/auth/auth_input_field.dart';
import '../../widgets/auth/google_auth_button.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _formKey = GlobalKey<FormState>();
  final _firstNameController = TextEditingController();
  final _lastNameController = TextEditingController();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();
  final _birthDateController = TextEditingController();

  final _firstNameFocusNode = FocusNode();
  final _lastNameFocusNode = FocusNode();
  final _emailFocusNode = FocusNode();
  final _passwordFocusNode = FocusNode();
  final _confirmPasswordFocusNode = FocusNode();

  bool _isGoogleSignIn = false;
  DateTime? _selectedDate;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    final authProvider = Provider.of<AuthProvider>(context, listen: false);

    if (authProvider.savedEmail != null &&
        authProvider.savedEmail!.isNotEmpty) {
      _emailController.text = authProvider.savedEmail!;
      FocusScope.of(context).requestFocus(_passwordFocusNode);
    }
  }

  @override
  void dispose() {
    _firstNameController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();

    _firstNameFocusNode.dispose();
    _emailFocusNode.dispose();
    _passwordFocusNode.dispose();
    _confirmPasswordFocusNode.dispose();
    super.dispose();
  }

  void _onRegisterPressed() async {
    if (_formKey.currentState?.validate() ?? false) {
      final email = _emailController.text.trim();
      final firstName = _firstNameController.text.trim();
      final lastName = _lastNameController.text.trim();
      final password = _passwordController.text;
      final dateOfBirth = _birthDateController.text;

      final authProvider = Provider.of<AuthProvider>(context, listen: false);
      final success = await authProvider.registration(
        email: email,
        password: password,
        firstName: firstName,
        lastName: lastName,
        dateOfBirth: dateOfBirth,
      );

      if (mounted && success) {
        authProvider.updateSavedEmail(email);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Регистрация успешна! Пожалуйста, войдите.'),
            backgroundColor: Colors.green,
          ),
        );
        Navigator.of(context).pop();
      }
    }
  }

  void _onGoogleSignInPressed() async {
    setState(() {
      _isGoogleSignIn = true;
    });

    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    final success = await authProvider.signInWithGoogle();

    if (mounted) {
      setState(() {
        _isGoogleSignIn = false;
      });

      if (success) {
        // Навигация на главный экран
        Navigator.of(context).pushReplacementNamed('/home');
      } else {
        // Показываем сообщение об ошибке
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              authProvider.errorMessage ?? 'Ошибка входа через Google',
            ),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  String? _validateConfirmPassword(String? value) {
    return InputValidator.validateConfirmPassword(
      value,
      _passwordController.text,
    );
  }

  Future<void> _selectDate(BuildContext context) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime(1900),
      lastDate: DateTime.now(),
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: const ColorScheme.light(
              primary: Colors.blue, // Цвет выбранной даты
              onPrimary: Colors.white, // Цвет текста выбранной даты
              onSurface: Colors.black, // Цвет текста в календаре
            ),
            textButtonTheme: TextButtonThemeData(
              style: TextButton.styleFrom(
                foregroundColor: Colors.blue, // Цвет кнопок
              ),
            ),
          ),
          child: child!,
        );
      },
    );

    if (picked != null && picked != _selectedDate) {
      setState(() {
        _selectedDate = picked;
        _birthDateController.text = DateFormat('dd.MM.yyyy').format(picked);
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final authProvider = Provider.of<AuthProvider>(context);
    final errorMessage = authProvider.errorMessage;

    return Scaffold(
      body: SafeArea(
        child: Center(
          child: SingleChildScrollView(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Form(
                key: _formKey,
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    Padding(
                      padding: const EdgeInsets.only(top: 32),
                      child: const Text(
                        'Создайте аккаунт',
                        style: TextStyle(
                          fontSize: 24,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                    const SizedBox(height: 24),
                    AuthInputField(
                      label: 'Имя',
                      hintText: 'Введите ваше имя',
                      controller: _firstNameController,
                      focusNode: _firstNameFocusNode,
                      validator: InputValidator.validateName,
                      textInputAction: TextInputAction.next,
                      onEditingComplete:
                          () => FocusScope.of(
                            context,
                          ).requestFocus(_firstNameFocusNode),
                      prefixIcon: const Icon(Icons.person_outline),
                    ),
                    AuthInputField(
                      label: 'Имя',
                      hintText: 'Введите вашу фамилию',
                      controller: _lastNameController,
                      focusNode: _lastNameFocusNode,
                      validator: InputValidator.validateName,
                      textInputAction: TextInputAction.next,
                      onEditingComplete:
                          () => FocusScope.of(
                            context,
                          ).requestFocus(_lastNameFocusNode),
                      prefixIcon: const Icon(Icons.person_outline),
                    ),
                    AuthInputField(
                      label: 'Email',
                      hintText: 'Введите ваш email',
                      controller: _emailController,
                      keyboardType: TextInputType.emailAddress,
                      focusNode: _emailFocusNode,
                      validator: InputValidator.validateEmail,
                      textInputAction: TextInputAction.next,
                      onEditingComplete:
                          () => FocusScope.of(
                            context,
                          ).requestFocus(_passwordFocusNode),
                      prefixIcon: const Icon(Icons.email_outlined),
                    ),
                    AuthInputField(
                      label: 'Пароль',
                      hintText: 'Введите пароль',
                      controller: _passwordController,
                      isPassword: true,
                      focusNode: _passwordFocusNode,
                      validator: InputValidator.validatePassword,
                      textInputAction: TextInputAction.next,
                      onEditingComplete:
                          () => FocusScope.of(
                            context,
                          ).requestFocus(_confirmPasswordFocusNode),
                      prefixIcon: const Icon(Icons.lock_outline),
                    ),
                    AuthInputField(
                      label: 'Подтверждение пароля',
                      hintText: 'Подтвердите пароль',
                      controller: _confirmPasswordController,
                      isPassword: true,
                      focusNode: _confirmPasswordFocusNode,
                      validator: _validateConfirmPassword,
                      textInputAction: TextInputAction.done,
                      onEditingComplete: _onRegisterPressed,
                      prefixIcon: const Icon(Icons.lock_outline),
                    ),
                    const SizedBox(height: 4),
                    GestureDetector(
                      onTap: () => _selectDate(context),
                      child: AbsorbPointer(
                        child: AuthInputField(
                          label: 'Дата рождения',
                          hintText: 'Выберите дату',
                          controller: _birthDateController,
                          validator: (value) {
                            if (value == null || value.isEmpty) {
                              return 'Пожалуйста, выберите дату рождения';
                            }
                            return null;
                          },
                          prefixIcon: const Icon(Icons.calendar_today),
                        ),
                      ),
                    ),
                    const SizedBox(height: 24),
                    if (errorMessage != null) ...[
                      Container(
                        padding: const EdgeInsets.all(8),
                        decoration: BoxDecoration(
                          color: Colors.red.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: Text(
                          errorMessage,
                          style: const TextStyle(
                            color: Colors.red,
                            fontWeight: FontWeight.w500,
                          ),
                          textAlign: TextAlign.center,
                        ),
                      ),
                      const SizedBox(height: 16),
                    ],
                    AuthButton(
                      text: 'Зарегистрироваться',
                      onPressed: _onRegisterPressed,
                      isLoading: authProvider.isLoading,
                    ),
                    const SizedBox(height: 16),
                    Text(
                      'или',
                      style: TextStyle(
                        color: Colors.grey[600],
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                    const SizedBox(height: 16),
                    GoogleAuthButton(
                      onPressed: _onGoogleSignInPressed,
                      isLoading: authProvider.isLoading && _isGoogleSignIn,
                      text: 'Регистрация через Google',
                    ),
                    const SizedBox(height: 24),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Text(
                          'Уже есть аккаунт?',
                          style: TextStyle(color: Colors.grey[600]),
                        ),
                        TextButton(
                          onPressed: () {
                            // Возврат на экран входа
                            Navigator.of(context).pop();
                            final email = _emailController.text.trim();
                            final authProvider = Provider.of<AuthProvider>(
                              context,
                              listen: false,
                            );
                            authProvider.updateSavedEmail(email);
                          },
                          child: const Text('Войти'),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
