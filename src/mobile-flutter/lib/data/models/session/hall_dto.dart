import '../../../domain/entities/session/hall.dart';

class HallDto extends Hall {
  const HallDto({required super.id, required super.name});

  factory HallDto.fromJson(Map<String, dynamic> json) {
    return HallDto(
      id: json['hallId']?.toString() ?? '',
      name: json['hallName']?.toString() ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {'hallId': id, 'hallName': name};
  }

  factory HallDto.fromEntity(Hall hall) {
    return HallDto(id: hall.id, name: hall.name);
  }

  HallDto copyWith({String? id, String? name}) {
    return HallDto(id: id ?? this.id, name: name ?? this.name);
  }
}
