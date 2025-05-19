import 'package:equatable/equatable.dart';

import 'hall.dart';

class Session extends Equatable {
  final String id;
  final String movieId;
  final Hall hall;
  final String hallId;
  final String dayId;
  final double priceModifier;
  final DateTime startTime;
  final DateTime endTime;

  const Session({
    required this.id,
    required this.movieId,
    required this.hallId,
    required this.hall,
    required this.dayId,
    required this.priceModifier,
    required this.startTime,
    required this.endTime,
  });

  @override
  List<Object?> get props => [
    id,
    movieId,
    hallId,
    hall,
    dayId,
    priceModifier,
    startTime,
    endTime,
  ];
}
