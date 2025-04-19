import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/utils/input_validator.dart';
import '../../providers/auth_provider.dart';
import '../../widgets/auth/auth_button.dart';
import '../../widgets/auth/auth_input_field.dart';
import '../../widgets/auth/google_auth_button.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _emailFocusNode = FocusNode();
  final _passwordFocusNode = FocusNode();

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

  bool _isGoogleSignIn = false;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    _emailFocusNode.dispose();
    _passwordFocusNode.dispose();
    super.dispose();
  }

  // TODO при загрузке логина вставлять savedEmail из AuthProviider если он есть

  void _onLoginPressed() async {
    if (_formKey.currentState?.validate() ?? false) {
      final email = _emailController.text.trim();
      final password = _passwordController.text;

      final authProvider = Provider.of<AuthProvider>(context, listen: false);
      final success = await authProvider.login(
        email: email,
        password: password,
      );

      if (success && mounted) {
        Navigator.of(context).pushReplacementNamed('/home');
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
                    const Text(
                      'Вход в CinemaApp',
                      style: TextStyle(
                        fontSize: 24,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 40),
                    AuthInputField(
                      label: 'Email',
                      hintText: 'Введите ваш email',
                      controller: _emailController,
                      keyboardType: TextInputType.emailAddress,
                      textInputAction: TextInputAction.next,
                      focusNode: _emailFocusNode,
                      validator: InputValidator.validateEmail,
                      onEditingComplete:
                          () => FocusScope.of(
                            context,
                          ).requestFocus(_passwordFocusNode),
                      prefixIcon: const Icon(Icons.email_outlined),
                    ),
                    const SizedBox(height: 8),
                    AuthInputField(
                      label: 'Пароль',
                      hintText: 'Введите ваш пароль',
                      controller: _passwordController,
                      isPassword: true,
                      focusNode: _passwordFocusNode,
                      validator: InputValidator.validatePassword,
                      textInputAction: TextInputAction.done,
                      onEditingComplete: _onLoginPressed,
                      prefixIcon: const Icon(Icons.lock_outline),
                    ),
                    const SizedBox(height: 8),
                    Align(
                      alignment: Alignment.centerRight,
                      child: TextButton(
                        onPressed: () {
                          // Навигация на экран восстановления пароля
                        },
                        child: const Text('Забыли пароль?'),
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
                      text: 'Войти',
                      onPressed: _onLoginPressed,
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
                    ),
                    const SizedBox(height: 24),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Text(
                          'Нет аккаунта?',
                          style: TextStyle(color: Colors.grey[600]),
                        ),
                        TextButton(
                          onPressed: () {
                            // Навигация на экран регистрации
                            Navigator.of(context).pushNamed('/register');
                            final email = _emailController.text.trim();
                            final authProvider = Provider.of<AuthProvider>(
                              context,
                              listen: false,
                            );
                            authProvider.updateSavedEmail(email);
                          },
                          child: const Text('Зарегистрироваться'),
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
