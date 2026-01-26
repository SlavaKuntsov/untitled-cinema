// lib/domain/entities/session/selected_seat.dart
import 'seat.dart';
import 'seat_type.dart';

class SelectedSeat extends Seat {
  final SeatType seatType;
  final double price;

  const SelectedSeat({
    required super.id,
    required super.row,
    required super.column,
    required this.seatType,
    required this.price,
  });

  @override
  List<Object?> get props => [...super.props, seatType, price];
}
