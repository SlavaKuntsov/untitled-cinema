import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../../core/constants/api_constants.dart';
import '../../core/network/api_client.dart';
import '../../domain/entities/day/day.dart';

class DayManagementProvider extends ChangeNotifier {
  final ApiClient _apiClient;

  DayManagementProvider({required ApiClient apiClient})
    : _apiClient = apiClient;

  List<Day> _days = [];
  bool _isLoading = false;
  String? _errorMessage;

  List<Day> get days => _days;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  Future<void> fetchAllDays() async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final response = await _apiClient.get(ApiConstants.getAllDays());
      
      if (response != null) {
        final List<Day> loadedDays = [];
        
        if (response is List) {
          for (var item in response) {
            if (item is Map<String, dynamic>) {
              loadedDays.add(Day.fromJson(item));
            }
          }
        } else if (response is Map<String, dynamic> &&
            response.containsKey('days')) {
          final daysList = response['days'] as List;
          for (var item in daysList) {
            if (item is Map<String, dynamic>) {
              loadedDays.add(Day.fromJson(item));
            }
          }
        }
        
        _days = loadedDays;
      } else {
        _days = [];
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

  Future<Day?> createDay(DateTime startTime, DateTime endTime) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();
      
      // Format dates as dd-MM-yyyy HH:mm
      final DateFormat formatter = DateFormat('dd-MM-yyyy HH:mm');
      
      final payload = {
        'startTime': formatter.format(startTime),
        'endTime': formatter.format(endTime),
      };
      
      final response = await _apiClient.post(
        ApiConstants.createDay(),
        data: payload,
      );
      
      if (response != null && response is Map<String, dynamic>) {
        final newDay = Day.fromJson(response);
        _days.add(newDay);
        
        _isLoading = false;
        notifyListeners();
        
        return newDay;
      }
      
      _isLoading = false;
      notifyListeners();
      return null;
    } catch (e) {
      _errorMessage = e.toString();
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<void> deleteDay(String dayId) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();
      
      try {
        await _apiClient.delete(ApiConstants.deleteDay(dayId));
      } catch (e) {
        // Ignore 204 No Content status code as it's a successful response for DELETE
        if (e.toString().contains('204')) {
          // This is a successful response, do nothing
        } else {
          // For other errors, rethrow
          rethrow;
        }
      }
      
      // Remove day from local list even if the API returned 204
      _days.removeWhere((day) => day.id == dayId);
      
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
