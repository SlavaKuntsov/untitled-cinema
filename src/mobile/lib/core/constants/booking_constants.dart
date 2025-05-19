// lib/core/constants/booking_constants.dart
import 'package:flutter/material.dart';

enum BookingStatus {
  cancelled(-1, 'Отменено'),
  reserved(0, 'Забронировано'),
  paid(1, 'Оплачено');

  final int value;
  final String label;

  const BookingStatus(this.value, this.label);

  static BookingStatus fromString(String status) {
    return BookingStatus.values.firstWhere(
      (e) => e.name == status.toLowerCase(),
      orElse: () => BookingStatus.reserved,
    );
  }

  Color get color {
    switch (this) {
      case BookingStatus.cancelled:
        return Colors.red;
      case BookingStatus.reserved:
        return Colors.orange;
      case BookingStatus.paid:
        return Colors.green;
    }
  }
}
