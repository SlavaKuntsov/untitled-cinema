import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../providers/auth_provider.dart';

class ProfileScreen extends StatelessWidget {
  const ProfileScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final authProvider = Provider.of<AuthProvider>(context);

    return SingleChildScrollView(
      child: Column(
        children: [
          const SizedBox(height: 20),
          const CircleAvatar(
            radius: 50,
            backgroundColor: Colors.orange,
            child: Icon(Icons.person, size: 50, color: Colors.white),
          ),
          const SizedBox(height: 16),
          const Text(
            'Имя Пользователя',
            style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
          ),
          const Text(
            'user@example.com',
            style: TextStyle(color: Colors.grey, fontSize: 16),
          ),
          const SizedBox(height: 20),

          // Статистика
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 20),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                _buildStatItem('Подписки', '120'),
                _buildStatItem('Подписчики', '1,250'),
                _buildStatItem('Публикации', '35'),
              ],
            ),
          ),

          const SizedBox(height: 20),
          const Divider(),

          // Настройки профиля
          _buildProfileMenuItem(
            context,
            'Редактировать профиль',
            Icons.edit,
            () {
              // Действие при нажатии
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Редактирование профиля'),
                  duration: Duration(seconds: 1),
                ),
              );
            },
          ),
          _buildProfileMenuItem(context, 'Избранное', Icons.favorite, () {
            // Действие при нажатии
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('Избранное'),
                duration: Duration(seconds: 1),
              ),
            );
          }),
          _buildProfileMenuItem(
            context,
            'Уведомления',
            Icons.notifications,
            () {
              // Действие при нажатии
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Уведомления'),
                  duration: Duration(seconds: 1),
                ),
              );
            },
          ),
          _buildProfileMenuItem(context, 'Безопасность', Icons.security, () {
            // Действие при нажатии
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('Безопасность'),
                duration: Duration(seconds: 1),
              ),
            );
          }),
          _buildProfileMenuItem(context, 'Помощь и поддержка', Icons.help, () {
            // Действие при нажатии
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('Помощь и поддержка'),
                duration: Duration(seconds: 1),
              ),
            );
          }),

          const SizedBox(height: 10),
          const Divider(),

          // Кнопка выхода
          Padding(
            padding: const EdgeInsets.all(16),
            child: ElevatedButton(
              onPressed: () {
                authProvider.logout();
                Navigator.of(context).pushReplacementNamed('/login');
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(
                    content: Text('Выход из аккаунта'),
                    duration: Duration(seconds: 1),
                  ),
                );
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.red,
                foregroundColor: Colors.white,
                minimumSize: const Size(double.infinity, 50),
              ),
              child: const Text('Выйти из аккаунта'),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildStatItem(String label, String value) {
    return Column(
      children: [
        Text(
          value,
          style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
        ),
        Text(label, style: TextStyle(color: Colors.grey[600])),
      ],
    );
  }

  Widget _buildProfileMenuItem(
    BuildContext context,
    String title,
    IconData icon,
    VoidCallback onTap,
  ) {
    return ListTile(
      leading: Icon(icon),
      title: Text(title),
      trailing: const Icon(Icons.arrow_forward_ios, size: 16),
      onTap: onTap,
    );
  }
}
