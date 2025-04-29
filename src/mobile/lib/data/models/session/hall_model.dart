import '../../../domain/entities/session/hall.dart';

class HallModel extends Hall {
  const HallModel({required super.id, required super.name});

  factory HallModel.fromJson(Map<String, dynamic> json) {
    return HallModel(
      id: json['hallId']?.toString() ?? '',
      name: json['hallName']?.toString() ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {'hallId': id, 'hallName': name};
  }

  factory HallModel.fromEntity(Hall hall) {
    return HallModel(id: hall.id, name: hall.name);
  }

  HallModel copyWith({String? id, String? name}) {
    return HallModel(id: id ?? this.id, name: name ?? this.name);
  }
}
