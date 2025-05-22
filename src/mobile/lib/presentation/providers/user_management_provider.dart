import 'package:flutter/material.dart';

import '../../core/network/api_client.dart';
import '../../core/constants/api_constants.dart';
import '../../domain/entities/auth/user.dart';

class UserManagementProvider extends ChangeNotifier {
  final ApiClient _apiClient;
  
  UserManagementProvider({
    required ApiClient apiClient,
  }) : _apiClient = apiClient;

  List<User> _users = [];
  bool _isLoading = false;
  String? _errorMessage;

  List<User> get users => _users.where((user) => user.role != 'Admin').toList();
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  Future<void> fetchAllUsers() async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();
      
      final response = await _apiClient.get(ApiConstants.getAllUsers());
      
      if (response != null) {
        final List<User> loadedUsers = [];
        
        if (response is List) {
          for (var item in response) {
            if (item is Map<String, dynamic>) {
              loadedUsers.add(User.fromJson(item));
            }
          }
        } else if (response is Map<String, dynamic> && response.containsKey('users')) {
          final usersList = response['users'] as List;
          for (var item in usersList) {
            if (item is Map<String, dynamic>) {
              loadedUsers.add(User.fromJson(item));
            }
          }
        }
        
        _users = loadedUsers;
      } else {
        _users = [];
      }
      
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<void> deleteUser(String userId) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();
      
      await _apiClient.delete(ApiConstants.deleteUser(userId));
      
      // Remove user from local list
      _users.removeWhere((user) => user.id == userId);
      
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }
} 