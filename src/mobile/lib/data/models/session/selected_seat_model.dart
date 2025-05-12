// lib/data/models/session/selected_seat_model.dart
import '../../../domain/entities/session/seat.dart';
import '../../../domain/entities/session/seat_type.dart';
import '../../../domain/entities/session/selected_seat.dart';
import 'seat_type_model.dart';

class SelectedSeatModel extends SelectedSeat {
  const SelectedSeatModel({
    required String id,
    required int row,
    required int column,
    required SeatType seatType,
    required double price,
  }) : super(
         id: id,
         row: row,
         column: column,
         seatType: seatType,
         price: price,
       );

  factory SelectedSeatModel.fromJson(Map<String, dynamic> json) {
    // Парсим SeatType из вложенного JSON
    final seatTypeJson = json['seatType'];
    final SeatType seatType =
        seatTypeJson is Map<String, dynamic>
            ? SeatTypeModel.fromJson(seatTypeJson)
            : SeatTypeModel(
              id: json['seatTypeId'] ?? '1',
              name: 'Стандарт',
              priceModifier: 1.0,
            );

    return SelectedSeatModel(
      id: json['id'],
      row: json['row'],
      column: json['column'],
      seatType: seatType,
      price: (json['price'] as num?)?.toDouble() ?? 0.0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'row': row,
      'column': column,
      'seatType':
          seatType is SeatTypeModel
              ? (seatType as SeatTypeModel).toJson()
              : {
                'id': seatType.id,
                'name': seatType.name,
                'priceModifier': seatType.priceModifier,
              },
      'price': price,
    };
  }

  // Создаем копию с обновленными полями
  SelectedSeatModel copyWith({
    String? id,
    int? row,
    int? column,
    SeatType? seatType,
    double? price,
  }) {
    return SelectedSeatModel(
      id: id ?? this.id,
      row: row ?? this.row,
      column: column ?? this.column,
      seatType: seatType ?? this.seatType,
      price: price ?? this.price,
    );
  }

  // Фабричный метод для создания из Seat и дополнительных данных
  factory SelectedSeatModel.fromSeat({
    required Seat seat,
    required SeatType seatType,
    required double price,
  }) {
    return SelectedSeatModel(
      id: seat.id,
      row: seat.row,
      column: seat.column,
      seatType: seatType,
      price: price,
    );
  }
}
