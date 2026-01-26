// lib/data/models/session/hall_model.dart
import '../../../domain/entities/session/hall.dart';

class HallModel extends Hall {
  const HallModel({
    required super.id,
    required super.name,
    super.totalSeats = 0,
    super.seatsArray = const [],
  });

  factory HallModel.fromJson(Map<String, dynamic> json) {
    // Функция для безопасного преобразования в int
    int parseInt(dynamic value) {
      if (value == null) return 0;
      if (value is int) return value;
      if (value is double) return value.toInt();
      if (value is String) {
        try {
          return int.parse(value);
        } catch (e) {
          return 0;
        }
      }
      return 0;
    }

    // Обработка seatsArray из JSON
    List<List<int>> parseSeatsArray(dynamic seats) {
      if (seats == null) return [];
      if (seats is List) {
        return seats.map<List<int>>((row) {
          if (row is List) {
            return row.map<int>((seat) => parseInt(seat)).toList();
          }
          return [];
        }).toList();
      }
      return [];
    }

    return HallModel(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      totalSeats: parseInt(json['totalSeats']),
      seatsArray: parseSeatsArray(json['seatsArray']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'totalSeats': totalSeats,
      'seatsArray': seatsArray,
    };
  }

  factory HallModel.fromEntity(Hall hall) {
    return HallModel(
      id: hall.id,
      name: hall.name,
      totalSeats: hall.totalSeats,
      seatsArray: hall.seatsArray,
    );
  }

  HallModel copyWith({
    String? id,
    String? name,
    int? totalSeats,
    List<List<int>>? seatsArray,
  }) {
    return HallModel(
      id: id ?? this.id,
      name: name ?? this.name,
      totalSeats: totalSeats ?? this.totalSeats,
      seatsArray: seatsArray ?? this.seatsArray,
    );
  }
}
