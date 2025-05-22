import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../providers/auth_provider.dart';
import 'history_screen.dart';
import 'home_screen.dart';
import 'management_screen.dart';
import 'profile_screen.dart';

// NavigationScreen
class NavigationScreen extends StatefulWidget {
  final int initialIndex;

  const NavigationScreen({super.key, this.initialIndex = 0});

  @override
  State<NavigationScreen> createState() => _NavigationScreenState();
}

class _NavigationScreenState extends State<NavigationScreen> {
  late int _selectedIndex;

  @override
  void initState() {
    super.initState();
    _selectedIndex = widget.initialIndex;
  }

  List<Widget> get _pages {
    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    final isAdmin = authProvider.currentUser?.role == 'ADMIN';

    if (isAdmin) {
      return const <Widget>[
        HomeScreen(),
        HistoryScreen(),
        ManagementScreen(),
        ProfileScreen(),
      ];
    }

    return const <Widget>[
      HomeScreen(),
      HistoryScreen(),
      ProfileScreen(),
    ];
  }

  List<IconData> get _icons {
    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    final isAdmin = authProvider.currentUser?.role == 'ADMIN';

    if (isAdmin) {
      return [
        Icons.home_rounded,
        Icons.history,
        Icons.admin_panel_settings,
        Icons.person_rounded,
      ];
    }

    return [
      Icons.home_rounded,
      Icons.history,
      Icons.person_rounded,
    ];
  }

  List<String> get _labels {
    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    final isAdmin = authProvider.currentUser?.role == 'ADMIN';

    if (isAdmin) {
      return ['Главная', 'История', 'Управление', 'Профиль'];
    }

    return ['Главная', 'История', 'Профиль'];
  }

  void _onItemTapped(int index) {
    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      // appBar: AppBar(
      //   title: Text("Добро пожаловать", style: TextStyle(fontSize: 18)),
      //   actions: [
      //     IconButton(
      //       icon: const Icon(Icons.notifications),
      //       onPressed: () {
      //         ScaffoldMessenger.of(context).showSnackBar(
      //           const SnackBar(
      //             content: Text('Уведомления'),
      //             duration: Duration(seconds: 1),
      //           ),
      //         );
      //       },
      //     ),
      //   ],
      // ),
      body: _pages.elementAt(_selectedIndex),
      bottomNavigationBar: _buildFloatingNavBar(),
    );
  }

  Widget _buildFloatingNavBar() {
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
          children: List.generate(_icons.length, (index) {
            return _buildFloatingNavItem(index);
          }),
        ),
      ),
    );
  }

  Widget _buildFloatingNavItem(int index) {
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
              _icons[index],
              color: isSelected ? Colors.white : inactiveColor,
              size: 24,
            ),
            if (isSelected) ...[
              const SizedBox(width: 8),
              Text(
                _labels[index],
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
