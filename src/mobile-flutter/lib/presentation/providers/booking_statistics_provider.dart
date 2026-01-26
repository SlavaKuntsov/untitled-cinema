import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:intl/intl.dart';
import 'dart:convert';

import '../../core/constants/api_constants.dart';
import '../../domain/entities/booking/booking_history.dart';

class BookingStatisticsProvider extends ChangeNotifier {
  bool _isLoading = false;
  String? _errorMessage;
  List<BookingHistory> _bookingHistory = [];
  Map<String, List<BookingHistory>> _bookingsByDay = {};
  Map<String, double> _revenueByDay = {};
  DateTime _startDate = DateTime.now().subtract(const Duration(days: 30));
  DateTime _endDate = DateTime.now();

  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;
  List<BookingHistory> get bookingHistory => _bookingHistory;
  Map<String, List<BookingHistory>> get bookingsByDay => _bookingsByDay;
  Map<String, double> get revenueByDay => _revenueByDay;
  DateTime get startDate => _startDate;
  DateTime get endDate => _endDate;

  Future<void> fetchBookingHistory() async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.bookingsHistory}?Limit=100&Offset=1&SortBy=date&SortDirection=desc'),
      );

      if (response.statusCode == 200) {
        final jsonData = json.decode(response.body);
        final bookingResponse = BookingHistoryResponse.fromJson(jsonData);
        _bookingHistory = bookingResponse.items;
        
        // Process the data
        _processBookingData();
        
        _isLoading = false;
        notifyListeners();
      } else {
        _errorMessage = 'Failed to load booking history: ${response.statusCode}';
        _isLoading = false;
        notifyListeners();
      }
    } catch (e) {
      _errorMessage = 'Error: ${e.toString()}';
      _isLoading = false;
      notifyListeners();
    }
  }

  void setDateRange(DateTime start, DateTime end) {
    _startDate = start;
    _endDate = end;
    _processBookingData();
    notifyListeners();
  }

  void _processBookingData() {
    // Reset data
    _bookingsByDay = {};
    _revenueByDay = {};

    // Filter bookings by date range
    final filteredBookings = _bookingHistory.where((booking) {
      return booking.createdAt.isAfter(_startDate) && 
             booking.createdAt.isBefore(_endDate.add(const Duration(days: 1)));
    }).toList();

    // Group bookings by day
    for (var booking in filteredBookings) {
      final dayKey = DateFormat('dd-MM-yyyy').format(booking.createdAt);
      
      if (!_bookingsByDay.containsKey(dayKey)) {
        _bookingsByDay[dayKey] = [];
      }
      
      _bookingsByDay[dayKey]!.add(booking);
      
      // Calculate revenue for paid bookings
      if (booking.isPaid) {
        if (!_revenueByDay.containsKey(dayKey)) {
          _revenueByDay[dayKey] = 0;
        }
        
        _revenueByDay[dayKey] = (_revenueByDay[dayKey] ?? 0) + booking.totalPrice;
      }
    }
  }

  // Get total revenue for the current date range
  double get totalRevenue {
    return _revenueByDay.values.fold(0, (sum, value) => sum + value);
  }
  
  // Get total number of tickets sold
  int get totalTicketsSold {
    return _bookingHistory
        .where((booking) => booking.isPaid)
        .fold(0, (sum, booking) => sum + booking.seats.length);
  }
  
  // Get list of days for chart X-axis
  List<String> get chartDays {
    List<String> days = [];
    DateTime current = _startDate;
    
    while (current.isBefore(_endDate) || current.isAtSameMomentAs(_endDate)) {
      days.add(DateFormat('dd').format(current));
      current = current.add(const Duration(days: 1));
    }
    
    return days;
  }
  
  // Get full dates for chart tooltip
  List<String> get chartFullDates {
    List<String> dates = [];
    DateTime current = _startDate;
    
    while (current.isBefore(_endDate) || current.isAtSameMomentAs(_endDate)) {
      dates.add(DateFormat('dd-MM-yyyy').format(current));
      current = current.add(const Duration(days: 1));
    }
    
    return dates;
  }
  
  // Get revenue data for chart
  List<double> get chartRevenue {
    final days = chartDays;
    final fullDates = chartFullDates;
    return List.generate(days.length, (index) {
      final fullDay = fullDates[index];
      return _revenueByDay[fullDay] ?? 0.0;
    });
  }
} 