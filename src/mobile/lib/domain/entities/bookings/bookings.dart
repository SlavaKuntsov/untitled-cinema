// lib/domain/entities/booking/booking.dart
import 'package:equatable/equatable.dart';

import '../session/seat.dart';

class Booking extends Equatable {
  final String id;
  final String userId;
  final String sessionId;
  final List<Seat> seats;
  final double totalPrice;
  final String status;
  final DateTime createdAt;
  final DateTime updatedAt;

  const Booking({
    required this.id,
    required this.userId,
    required this.sessionId,
    required this.seats,
    required this.totalPrice,
    required this.status,
    required this.createdAt,
    required this.updatedAt,
  });

  @override
  List<Object?> get props => [
    id,
    userId,
    sessionId,
    seats,
    totalPrice,
    status,
    createdAt,
    updatedAt,
  ];
}
