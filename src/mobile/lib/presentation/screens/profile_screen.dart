import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../providers/auth_provider.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({Key? key}) : super(key: key);

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  bool _isEditing = false;

  // Контроллеры для текстовых полей
  late TextEditingController _emailController;
  late TextEditingController _firstNameController;
  late TextEditingController _lastNameController;
  late TextEditingController _dateOfBirthController;

  @override
  void initState() {
    super.initState();
    _emailController = TextEditingController();
    _firstNameController = TextEditingController();
    _lastNameController = TextEditingController();
    _dateOfBirthController = TextEditingController();

    // Инициализация полей после рендеринга
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _initFields();
    });
  }

  @override
  void dispose() {
    _emailController.dispose();
    _firstNameController.dispose();
    _lastNameController.dispose();
    _dateOfBirthController.dispose();
    super.dispose();
  }

  void _initFields() {
    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    authProvider.checkUser();
    final user = authProvider.currentUser;

    if (user != null) {
      _emailController.text = user.email;
      _firstNameController.text = user.firstName;
      _lastNameController.text = user.lastName;
      _dateOfBirthController.text = user.dateOfBirth;
    }
  }

  void _saveProfile() async {
    final authProvider = Provider.of<AuthProvider>(context, listen: false);

    // Validate that firstName is not empty
    if (_firstNameController.text.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Имя не может быть пустым',
            style: TextStyle(color: Colors.white),
          ),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    final success = await authProvider.updateUserProfile(
      firstName: _firstNameController.text,
      lastName: _lastNameController.text,
      dateOfBirth: _dateOfBirthController.text,
    );

    if (success) {
      setState(() {
        _isEditing = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Профиль успешно обновлен'),
          backgroundColor: Colors.green,
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            'Ошибка: ${authProvider.errorMessage ?? "Не удалось обновить профиль"}',
          ),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _showDeleteConfirmation() async {
    return showDialog<void>(
      context: context,
      barrierDismissible: false,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Подтверждение'),
          content: const SingleChildScrollView(
            child: Text(
              'Вы действительно хотите удалить свой аккаунт? Это '
              'действие нельзя отменить.',
            ),
          ),
          actions: <Widget>[
            TextButton(
              child: const Text('Отмена'),
              onPressed: () {
                Navigator.of(context).pop();
              },
            ),
            TextButton(
              child: const Text('Удалить', style: TextStyle(color: Colors.red)),
              onPressed: () {
                final authProvider = Provider.of<AuthProvider>(
                  context,
                  listen: false,
                );
                authProvider.deleteAccount();
                Navigator.of(context).pop();
                Navigator.of(context).pushReplacementNamed('/login');
              },
            ),
          ],
        );
      },
    );
  }

  Future<void> _showLogoutConfirmation() async {
    return showDialog<void>(
      context: context,
      barrierDismissible: false,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Подтверждение'),
          content: const SingleChildScrollView(
            child: Text('Вы действительно хотите выйти из аккаунта?'),
          ),
          actions: <Widget>[
            TextButton(
              child: const Text('Отмена'),
              onPressed: () {
                Navigator.of(context).pop();
              },
            ),
            TextButton(
              child: const Text('Выйти'),
              onPressed: () {
                final authProvider = Provider.of<AuthProvider>(
                  context,
                  listen: false,
                );
                authProvider.logout();
                Navigator.of(context).pop();
                Navigator.of(context).pushReplacementNamed('/login');
              },
            ),
          ],
        );
      },
    );
  }

  Future<void> _selectDate() async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate:
          _dateOfBirthController.text.isNotEmpty
              ? DateFormat('dd-MM-yyyy').parse(_dateOfBirthController.text)
              : DateTime.now().subtract(const Duration(days: 365 * 18)),
      firstDate: DateTime(1900),
      lastDate: DateTime.now(),
      builder: (context, child) {
        return Theme(
          data: ThemeData.dark().copyWith(
            colorScheme: const ColorScheme.dark(
              primary: Colors.blue,
              onPrimary: Colors.white,
              onSurface: Colors.white,
            ),
          ),
          child: child!,
        );
      },
    );

    if (picked != null) {
      setState(() {
        _dateOfBirthController.text = DateFormat('dd-MM-yyyy').format(picked);
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final authProvider = Provider.of<AuthProvider>(context);
    final user = authProvider.currentUser;
    final isAdmin = user?.role == 'Admin';

    if (user == null) {
      return const Center(child: CircularProgressIndicator());
    }

    return Scaffold(
      // appBar: AppBar(
      //   title: const Text('Профиль'),
      //   backgroundColor: AppTheme.primaryColor,
      // ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.only(
          top: 48,
          right: 16,
          left: 16,
          bottom: 16,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header with user name - only show for non-admin users
            if (!isAdmin) ...[
              Center(
                child: Row(
                  children: [
                    Text(
                      user.firstName,
                      style: const TextStyle(
                        fontSize: 48,
                        fontWeight: FontWeight.bold,
                        color: Colors.white,
                      ),
                    ),
                    SizedBox(width: 16),
                    Text(
                      user.lastName,
                      style: const TextStyle(
                        fontSize: 48,
                        fontWeight: FontWeight.bold,
                        color: Colors.white,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 40),
            ],

            // Admin role indicator
            if (isAdmin) ...[
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: AppTheme.primaryColor.withOpacity(0.3),
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: AppTheme.primaryColor),
                ),
                child: Column(
                  children: [
                    const Icon(
                      Icons.admin_panel_settings,
                      color: Colors.white,
                      size: 48,
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Администратор',
                      style: TextStyle(
                        fontSize: 24,
                        fontWeight: FontWeight.bold,
                        color: Colors.white,
                      ),
                    ),
                    Text(
                      user.email,
                      style: TextStyle(fontSize: 16, color: Colors.white70),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 24),
            ],

            // Edit toggle - only for non-admin
            if (!isAdmin) ...[
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Text(
                    'Изменить данные профиля:',
                    style: TextStyle(fontSize: 19, color: Colors.white),
                  ),
                  Switch(
                    value: _isEditing,
                    onChanged: (value) {
                      setState(() {
                        _isEditing = value;
                      });
                    },
                    activeColor: AppTheme.accentColor,
                  ),
                ],
              ),
              const SizedBox(height: 20),
            ],

            // Email - always show
            const Text(
              'Email',
              style: TextStyle(fontSize: 18, color: Colors.white),
            ),
            const SizedBox(height: 8),
            TextField(
              controller: _emailController,
              style: const TextStyle(color: Colors.white),
              enabled: false, // Email cannot be edited
              decoration: InputDecoration(
                filled: true,
                fillColor: Colors.grey[900],
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                  borderSide: BorderSide.none,
                ),
              ),
            ),

            // Only show these fields for non-admin users
            if (!isAdmin) ...[
              const SizedBox(height: 20),

              // FirstName
              const Text(
                'Имя',
                style: TextStyle(fontSize: 18, color: Colors.white),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: _firstNameController,
                style: const TextStyle(color: Colors.white),
                enabled: _isEditing,
                decoration: InputDecoration(
                  filled: true,
                  fillColor: _isEditing ? Colors.grey[800] : Colors.grey[900],
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(8),
                    borderSide: BorderSide.none,
                  ),
                ),
              ),

              const SizedBox(height: 20),

              // LastName
              const Text(
                'Фамилия',
                style: TextStyle(fontSize: 18, color: Colors.white),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: _lastNameController,
                style: const TextStyle(color: Colors.white),
                enabled: _isEditing,
                decoration: InputDecoration(
                  filled: true,
                  fillColor: _isEditing ? Colors.grey[800] : Colors.grey[900],
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(8),
                    borderSide: BorderSide.none,
                  ),
                ),
              ),

              const SizedBox(height: 20),

              // Date of Birth
              const Text(
                'Деь Рождение',
                style: TextStyle(fontSize: 18, color: Colors.white),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: _dateOfBirthController,
                style: const TextStyle(color: Colors.white),
                enabled: _isEditing,
                readOnly: true, // Readonly because we use date picker
                onTap: _isEditing ? _selectDate : null,
                decoration: InputDecoration(
                  filled: true,
                  fillColor: _isEditing ? Colors.grey[800] : Colors.grey[900],
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(8),
                    borderSide: BorderSide.none,
                  ),
                  suffixIcon:
                      _isEditing
                          ? const Icon(
                            Icons.calendar_today,
                            color: Colors.white,
                          )
                          : null,
                ),
              ),
            ],

            const SizedBox(height: 16),

            if (!isAdmin) ...[
              if (_isEditing) ...[
                ElevatedButton(
                  onPressed: _saveProfile,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: AppTheme.accentColor,
                    minimumSize: const Size(double.infinity, 50),
                  ),
                  child: const Text(
                    'Сохранить',
                    style: TextStyle(fontSize: 16),
                  ),
                ),
                const SizedBox(height: 16),
              ],
              // else
              //   ElevatedButton(
              //     onPressed: () {
              //       setState(() {
              //         _isEditing = true;
              //       });
              //     },
              //     style: ElevatedButton.styleFrom(
              //       backgroundColor: AppTheme.accentColor,
              //       minimumSize: const Size(double.infinity, 50),
              //     ),
              //     child: const Text(
              //       'Редактировать',
              //       style: TextStyle(fontSize: 16),
              //     ),
              //   ),
              ElevatedButton(
                onPressed: _showDeleteConfirmation,
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.red,
                  minimumSize: const Size(double.infinity, 50),
                ),
                child: const Text(
                  'Удалить аккаунт',
                  style: TextStyle(fontSize: 16),
                ),
              ),
            ],
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _showLogoutConfirmation,
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.grey,
                minimumSize: const Size(double.infinity, 50),
              ),
              child: const Text('Выйти', style: TextStyle(fontSize: 16)),
            ),
          ],
        ),
      ),
    );
  }
}
