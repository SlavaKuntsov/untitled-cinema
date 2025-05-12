// lib/data/models/session/session_seats_model.dart
import '../../../domain/entities/session/session_seats.dart';
import 'seat_model.dart';

class SessionSeatsModel extends SessionSeats {
  const SessionSeatsModel({
    required String sessionId,
    required List<SeatModel> reservedSeats,
  }) : super(sessionId: sessionId, reservedSeats: reservedSeats);

  factory SessionSeatsModel.fromJson(Map<String, dynamic> json) {
    final List<dynamic> seatsJson = json['reservedSeats'] ?? [];
    final List<SeatModel> seats =
        seatsJson.map((seat) => SeatModel.fromJson(seat)).toList();

    return SessionSeatsModel(
      sessionId: json['sessionId'],
      reservedSeats: seats,
    );
  }
}
