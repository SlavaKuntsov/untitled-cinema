import 'package:flutter/material.dart';

import '../../config/theme.dart';

class ManagementScreen extends StatelessWidget {
  const ManagementScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Управление'),
        backgroundColor: AppTheme.primaryColor,
      ),
      body: ListView(
        padding: const EdgeInsets.all(16.0),
        children: [
          _buildManagementCard(context, 'Управление фильмами', Icons.movie, () {
            Navigator.pushNamed(context, '/movies_management');
          }),
          const SizedBox(height: 16),
          _buildManagementCard(
            context,
            'Управление сеансами',
            Icons.event_seat,
            () {
              Navigator.pushNamed(context, '/days_management');
            },
          ),
          const SizedBox(height: 16),
          _buildManagementCard(
            context,
            'Управление залами',
            Icons.meeting_room,
            () {
              Navigator.pushNamed(context, '/halls_management');
            },
          ),
          const SizedBox(height: 16),
          _buildManagementCard(
            context,
            'Управление пользователями',
            Icons.people,
            () {
              Navigator.pushNamed(context, '/users_management');
            },
          ),
          const SizedBox(height: 16),
          _buildManagementCard(
            context,
            'Статистика продаж',
            Icons.bar_chart,
            () {
              Navigator.pushNamed(context, '/booking_management');
            },
          ),
          const SizedBox(height: 16),
          _buildManagementCard(
            context,
            'Статистика фильмов',
            Icons.movie_filter,
            () {
              Navigator.pushNamed(context, '/movie_statistics');
            },
          ),
        ],
      ),
    );
  }

  Widget _buildManagementCard(
    BuildContext context,
    String title,
    IconData icon,
    VoidCallback onTap,
  ) {
    return Card(
      elevation: 4,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Row(
            children: [
              Icon(icon, size: 28, color: Colors.white),
              const SizedBox(width: 16),
              Text(
                title,
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const Spacer(),
              const Icon(Icons.arrow_forward_ios, color: Colors.grey),
            ],
          ),
        ),
      ),
    );
  }
}
