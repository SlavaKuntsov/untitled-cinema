import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../providers/auth_provider.dart';
import 'history_screen.dart';
import 'home_screen.dart';
import 'management_screen.dart';
import 'profile_screen.dart';

class NavigationScreen extends StatefulWidget {
  final int initialIndex;

  const NavigationScreen({super.key, this.initialIndex = 0});

  @override
  State<NavigationScreen> createState() => _NavigationScreenState();
}

class _NavigationScreenState extends State<NavigationScreen> {
  late int _selectedIndex;
  bool _isInitialized = false;

  @override
  void initState() {
    super.initState();
    _selectedIndex = 0;
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    if (!_isInitialized) {
      final authProvider = Provider.of<AuthProvider>(context, listen: false);
      final isAdmin = authProvider.currentUser?.role == 'Admin';

      if (isAdmin) {
        // Для админа: Management (0) и Profile (1)
        _selectedIndex =
            widget.initialIndex >= 2 ? 1 : (widget.initialIndex == 1 ? 1 : 0);
      } else {
        // Для пользователя: Home (0), History (1), Profile (2)
        _selectedIndex = widget.initialIndex.clamp(0, 2);
      }

      _isInitialized = true;
    }
  }

  void _onItemTapped(int index) {
    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    final authProvider = Provider.of<AuthProvider>(context);
    final isAdmin = authProvider.currentUser?.role == 'Admin';

    // Если еще не инициализированы, показываем загрузку
    if (!_isInitialized) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    final pages = _getPages(isAdmin);
    final icons = _getIcons(isAdmin);
    final labels = _getLabels(isAdmin);

    // Проверяем корректность индекса для текущей роли
    final maxIndex = pages.length - 1;
    if (_selectedIndex > maxIndex) {
      // Сбрасываем на первую страницу, если индекс некорректен
      WidgetsBinding.instance.addPostFrameCallback((_) {
        setState(() {
          _selectedIndex = 0;
        });
      });
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    return Scaffold(
      body:
          pages[_selectedIndex], // Используем прямой доступ вместо IndexedStack
      bottomNavigationBar: _buildFloatingNavBar(icons, labels),
    );
  }

  List<Widget> _getPages(bool isAdmin) {
    if (isAdmin) {
      return const <Widget>[ManagementScreen(), ProfileScreen()];
    }

    return const <Widget>[HomeScreen(), HistoryScreen(), ProfileScreen()];
  }

  List<IconData> _getIcons(bool isAdmin) {
    if (isAdmin) {
      return [Icons.admin_panel_settings, Icons.person_rounded];
    }

    return [Icons.home_rounded, Icons.history, Icons.person_rounded];
  }

  List<String> _getLabels(bool isAdmin) {
    if (isAdmin) {
      return ['Управление', 'Профиль'];
    }

    return ['Главная', 'История', 'Профиль'];
  }

  Widget _buildFloatingNavBar(List<IconData> icons, List<String> labels) {
    return Padding(
      padding: const EdgeInsets.all(14.0),
      child: Container(
        height: 70,
        decoration: BoxDecoration(
          color: AppTheme.primaryColor,
          borderRadius: BorderRadius.circular(35),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.15),
              blurRadius: 15,
              offset: const Offset(0, 5),
            ),
          ],
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          children: List.generate(icons.length, (index) {
            return _buildFloatingNavItem(index, icons[index], labels[index]);
          }),
        ),
      ),
    );
  }

  Widget _buildFloatingNavItem(int index, IconData icon, String label) {
    final isSelected = _selectedIndex == index;
    final activeColor = AppTheme.accentColor;
    final inactiveColor = Colors.grey;

    return InkWell(
      onTap: () => _onItemTapped(index),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 300),
        padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 14),
        decoration: BoxDecoration(
          color: isSelected ? activeColor : Colors.transparent,
          borderRadius: BorderRadius.circular(25),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              icon,
              color: isSelected ? Colors.white : inactiveColor,
              size: 24,
            ),
            if (isSelected) ...[
              const SizedBox(width: 8),
              Text(
                label,
                style: const TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
