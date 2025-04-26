import 'package:equatable/equatable.dart';

class Movie extends Equatable {
  final String id;
  final String title;
  final String description;
  final String poster;
  final List<String> genres;
  final double price;
  final int durationMinutes;
  final String producer;
  final String inRoles;
  final int ageLimit;
  final DateTime releaseDate;
  final DateTime createdAt;
  final DateTime updatedAt;

  const Movie({
    required this.id,
    required this.title,
    required this.description,
    this.poster = '',
    required this.genres,
    required this.price,
    required this.durationMinutes,
    required this.producer,
    required this.inRoles,
    required this.ageLimit,
    required this.releaseDate,
    required this.createdAt,
    required this.updatedAt,
  });

  @override
  List<Object?> get props => [
    id,
    title,
    description,
    poster,
    genres,
    price,
    durationMinutes,
    producer,
    inRoles,
    ageLimit,
    releaseDate,
    createdAt,
    updatedAt,
  ];
}
