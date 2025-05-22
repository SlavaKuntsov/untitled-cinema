import 'dart:convert';

import 'package:flutter/material.dart';

import '../../core/constants/api_constants.dart';
import '../../core/network/api_client.dart';
import '../../domain/entities/hall/hall.dart';

class HallManagementProvider extends ChangeNotifier {
  final ApiClient _apiClient;

  HallManagementProvider({required ApiClient apiClient})
    : _apiClient = apiClient;

  List<Hall> _halls = [];
  bool _isLoading = false;
  String? _errorMessage;

  List<Hall> get halls => _halls;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  Future<void> fetchAllHalls() async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final response = await _apiClient.get(ApiConstants.getAllHalls());

      if (response != null) {
        final List<Hall> loadedHalls = [];

        if (response is List) {
          for (var item in response) {
            if (item is Map<String, dynamic>) {
              loadedHalls.add(Hall.fromJson(item));
            }
          }
        } else if (response is Map<String, dynamic> &&
            response.containsKey('halls')) {
          final hallsList = response['halls'] as List;
          for (var item in hallsList) {
            if (item is Map<String, dynamic>) {
              loadedHalls.add(Hall.fromJson(item));
            }
          }
        }

        _halls = loadedHalls;
      } else {
        _halls = [];
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

  Future<Hall?> createSimpleHall(String name, int rows, int seatsPerRow) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      final payload = {'name': name, 'rows': rows, 'seatsPerRow': seatsPerRow};

      final response = await _apiClient.post(
        ApiConstants.createSimpleHall(),
        data: payload,
      );

      if (response != null && response is Map<String, dynamic>) {
        final newHall = Hall.fromJson(response);
        _halls.add(newHall);

        _isLoading = false;
        notifyListeners();

        return newHall;
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

  Future<Hall?> createCustomHall(String name, String seatsArrayJson) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      // Parse the JSON string to validate it
      final seatsArray = json.decode(seatsArrayJson);
      
      // Calculate total seats (excluding -1 values)
      int totalSeats = 0;
      for (var row in seatsArray) {
        for (var seat in row) {
          if (seat != -1) {
            totalSeats++;
          }
        }
      }

      final payload = {
        'name': name, 
        'seats': seatsArray,
        'totalSeats': totalSeats
      };

      final response = await _apiClient.post(
        ApiConstants.createCustomHall(),
        data: payload,
      );

      if (response != null && response is Map<String, dynamic>) {
        final newHall = Hall.fromJson(response);
        _halls.add(newHall);

        _isLoading = false;
        notifyListeners();

        return newHall;
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

  Future<Hall?> updateHall(
    String hallId,
    String name,
    String seatsArrayJson,
  ) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      // Parse the JSON string to validate it
      final seatsArray = json.decode(seatsArrayJson);

      final payload = {'id': hallId, 'name': name, 'seatsArray': seatsArray};

      final response = await _apiClient.patch(
        ApiConstants.updateHall(),
        data: payload,
      );

      if (response != null && response is Map<String, dynamic>) {
        final updatedHall = Hall.fromJson(response);
        final index = _halls.indexWhere((hall) => hall.id == hallId);
        if (index != -1) {
          _halls[index] = updatedHall;
        }

        _isLoading = false;
        notifyListeners();

        return updatedHall;
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

  Future<void> deleteHall(String hallId) async {
    try {
      _isLoading = true;
      _errorMessage = null;
      notifyListeners();

      try {
        await _apiClient.delete(ApiConstants.deleteHall(hallId));
      } catch (e) {
        // Ignore 204 No Content status code as it's a successful response for DELETE
        if (e.toString().contains('204')) {
          // This is a successful response, do nothing
        } else {
          // For other errors, rethrow
          rethrow;
        }
      }

      // Remove hall from local list even if the API returned 204
      _halls.removeWhere((hall) => hall.id == hallId);

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
