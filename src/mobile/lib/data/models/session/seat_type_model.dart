// lib/data/models/session/seat_type_model.dart
import '../../../domain/entities/session/seat_type.dart';

class SeatTypeModel extends SeatType {
  const SeatTypeModel({
    required super.id,
    required super.name,
    super.priceModifier = 1.0,
  });

  factory SeatTypeModel.fromJson(Map<String, dynamic> json) {
    // Функция для безопасного преобразования числовых типов в double
    double parseDouble(dynamic value) {
      if (value == null) return 1.0;
      if (value is int) return value.toDouble();
      if (value is double) return value;
      if (value is String) {
        try {
          return double.parse(value);
        } catch (e) {
          return 1.0;
        }
      }
      return 1.0;
    }

    return SeatTypeModel(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      priceModifier: parseDouble(json['priceModifier']),
    );
  }

  Map<String, dynamic> toJson() {
    return {'id': id, 'name': name, 'priceModifier': priceModifier};
  }

  factory SeatTypeModel.fromEntity(SeatType seatType) {
    return SeatTypeModel(
      id: seatType.id,
      name: seatType.name,
      priceModifier: seatType.priceModifier,
    );
  }

  SeatTypeModel copyWith({String? id, String? name, double? priceModifier}) {
    return SeatTypeModel(
      id: id ?? this.id,
      name: name ?? this.name,
      priceModifier: priceModifier ?? this.priceModifier,
    );
  }
}
