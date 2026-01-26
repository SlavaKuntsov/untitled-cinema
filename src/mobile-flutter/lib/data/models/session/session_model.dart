// lib/data/models/session/session_model.dart
import 'package:untitledCinema/data/models/session/hall_dto.dart';

import '../../../domain/entities/session/hall.dart';
import '../../../domain/entities/session/session.dart';

class SessionModel extends Session {
  const SessionModel({
    required super.id,
    required super.movieId,
    required super.hall,
    required super.hallId,
    required super.dayId,
    required super.priceModifier,
    required super.startTime,
    required super.endTime,
  });

  factory SessionModel.fromJson(Map<String, dynamic> json) {
    double parseDouble(dynamic value) {
      if (value == null) return 0.0;
      if (value is int) return value.toDouble();
      if (value is double) return value;
      if (value is String) {
        try {
          return double.parse(value);
        } catch (e) {
          return 0.0;
        }
      }
      return 0.0;
    }

    try {
      return SessionModel(
        id: json['id']?.toString() ?? '',
        movieId: json['movieId']?.toString() ?? '',
        hallId: json['hallId']?.toString() ?? '',
        hall:
            json['hall'] != null
                ? HallDto.fromJson(json['hall'])
                : const HallDto(id: '', name: ''),
        dayId: json['dayId']?.toString() ?? '',
        priceModifier: parseDouble(json['priceModifier']),
        startTime:
            json['startTime'] != null
                ? DateTime.parse(json['startTime'].toString())
                : DateTime.now(),
        endTime:
            json['endTime'] != null
                ? DateTime.parse(json['endTime'].toString())
                : DateTime.now(),
      );
    } catch (e) {
      print('Ошибка при создании SessionModel из JSON: $e');
      // Возвращаем модель с минимально необходимыми данными
      return SessionModel(
        id: json['id']?.toString() ?? '',
        movieId: json['movieId']?.toString() ?? '',
        hallId: json['hallId']?.toString() ?? '',
        hall: const HallDto(id: '', name: 'Ошибка загрузки зала'),
        dayId: json['dayId']?.toString() ?? '',
        priceModifier: 1.0,
        startTime: DateTime.now(),
        endTime: DateTime.now(),
      );
    }
  }

  Map<String, dynamic> toJson() {
    final hall = this.hall;
    final hallJson =
        hall is HallDto
            ? hall.toJson()
            : {'hallId': hall.id, 'hallName': hall.name};

    return {
      'id': id,
      'movieId': movieId,
      'hallId': hallId,
      'hall': hallJson,
      'dayId': dayId,
      'priceModifier': priceModifier,
      'startTime': startTime.toIso8601String(),
      'endTime': endTime.toIso8601String(),
    };
  }

  factory SessionModel.fromEntity(Session session) {
    return SessionModel(
      id: session.id,
      movieId: session.movieId,
      hallId: session.hallId,
      hall:
          session.hall is HallDto
              ? session.hall
              : HallDto.fromEntity(session.hall),
      dayId: session.dayId,
      priceModifier: session.priceModifier,
      startTime: session.startTime,
      endTime: session.endTime,
    );
  }

  SessionModel copyWith({
    String? id,
    String? movieId,
    Hall? hall,
    String? hallId,
    String? dayId,
    double? priceModifier,
    DateTime? startTime,
    DateTime? endTime,
  }) {
    return SessionModel(
      id: id ?? this.id,
      movieId: movieId ?? this.movieId,
      hallId: hallId ?? this.movieId,
      hall: hall ?? this.hall,
      dayId: dayId ?? this.dayId,
      priceModifier: priceModifier ?? this.priceModifier,
      startTime: startTime ?? this.startTime,
      endTime: endTime ?? this.endTime,
    );
  }
}
