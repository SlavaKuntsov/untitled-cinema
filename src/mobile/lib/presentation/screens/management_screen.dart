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
            // TODO: Navigate to movie management
          }),
          const SizedBox(height: 16),
          _buildManagementCard(
            context,
            'Управление сеансами',
            Icons.event_seat,
            () {
              // TODO: Navigate to session management
            },
          ),
          const SizedBox(height: 16),
          _buildManagementCard(
            context,
            'Управление днями',
            Icons.calendar_today,
            () {
              Navigator.pushNamed(context, '/days_management');
            },
          ),
          const SizedBox(height: 16),
          _buildManagementCard(
            context,
            'Управление залами',
            Icons.calendar_today,
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
          _buildManagementCard(context, 'Статистика', Icons.bar_chart, () {
            // TODO: Navigate to statistics
          }),
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
