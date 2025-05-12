// lib/domain/entities/session/session_seats.dart
import 'package:equatable/equatable.dart';

import 'seat.dart';

class SessionSeats extends Equatable {
  final String sessionId;
  final List<Seat> reservedSeats;

  const SessionSeats({required this.sessionId, required this.reservedSeats});

  @override
  List<Object?> get props => [sessionId, reservedSeats];
}
