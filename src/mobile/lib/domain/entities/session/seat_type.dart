// lib/domain/entities/session/seat_type.dart
import 'package:equatable/equatable.dart';

class SeatType extends Equatable {
  final String id;
  final String name;
  final double priceModifier;

  const SeatType({
    required this.id,
    required this.name,
    this.priceModifier = 1.0,
  });

  @override
  List<Object?> get props => [id, name, priceModifier];
}
