// lib/data/models/session/seat_model.dart
import '../../../domain/entities/session/seat.dart';

class SeatModel extends Seat {
  const SeatModel({required String id, required int row, required int column})
    : super(id: id, row: row, column: column);

  factory SeatModel.fromJson(Map<String, dynamic> json) {
    return SeatModel(id: json['id'], row: json['row'], column: json['column']);
  }

  Map<String, dynamic> toJson() {
    return {'id': id, 'row': row, 'column': column};
  }
}
