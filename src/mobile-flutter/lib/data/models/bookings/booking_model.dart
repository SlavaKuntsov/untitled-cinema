// lib/data/models/booking/booking_model.dart
import 'package:intl/intl.dart';

import '../../../domain/entities/bookings/bookings.dart';
import '../session/seat_model.dart';

class BookingModel extends Booking {
  const BookingModel({
    required super.id,
    required super.userId,
    required super.sessionId,
    required super.seats,
    required super.totalPrice,
    required super.status,
    required super.createdAt,
    required super.updatedAt,
  });

  factory BookingModel.fromJson(Map<String, dynamic> json) {
    return BookingModel(
      id: json['id'],
      userId: json['userId'],
      sessionId: json['sessionId'],
      seats:
          (json['seats'] as List)
              .map((seat) => SeatModel.fromJson(seat))
              .toList(),
      totalPrice: json['totalPrice'].toDouble(),
      status: json['status'],
      createdAt: _parseDateTime(json['createdAt']),
      updatedAt: _parseDateTime(json['updatedAt']),
    );
  }

  static DateTime _parseDateTime(dynamic dateString) {
    if (dateString == null) return DateTime.now();

    try {
      // Пробуем стандартный ISO формат
      return DateTime.parse(dateString.toString());
    } catch (_) {
      try {
        // Пробуем формат dd-MM-yyyy HH:mm
        // Используем DateFormat из пакета intl
        final DateFormat format = DateFormat('dd-MM-yyyy HH:mm');
        return format.parse(dateString.toString());
      } catch (_) {
        try {
          // Пробуем еще один распространенный формат MM/dd/yyyy HH:mm
          final DateFormat altFormat = DateFormat('MM/dd/yyyy HH:mm');
          return altFormat.parse(dateString.toString());
        } catch (_) {
          // Возвращаем текущую дату, если ничего не сработало
          return DateTime.now();
        }
      }
    }
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'userId': userId,
      'sessionId': sessionId,
      'seats': seats.map((seat) => (seat as SeatModel).toJson()).toList(),
      'totalPrice': totalPrice,
      'status': status,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }
}
