// lib/domain/entities/session/seat.dart
import 'package:equatable/equatable.dart';

class Seat extends Equatable {
  final String id;
  final int row;
  final int column;

  const Seat({required this.id, required this.row, required this.column});

  @override
  List<Object?> get props => [id, row, column];
}
