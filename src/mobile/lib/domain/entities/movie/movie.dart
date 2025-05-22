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

  factory Movie.fromJson(Map<String, dynamic> json) {
    return Movie(
      id: json['id'],
      title: json['title'],
      description: json['description'] ?? '',
      poster: json['poster'] ?? '',
      genres: (json['genres'] as List?)?.map((e) => e.toString()).toList() ?? [],
      price: (json['price'] is int) 
          ? (json['price'] as int).toDouble() 
          : json['price'] ?? 0.0,
      durationMinutes: json['durationMinutes'] ?? 0,
      producer: json['producer'] ?? '',
      inRoles: json['inRoles'] ?? '',
      ageLimit: json['ageLimit'] ?? 0,
      releaseDate: json['releaseDate'] != null
          ? DateTime.parse(json['releaseDate'])
          : DateTime.now(),
      createdAt: json['createdAt'] != null
          ? DateTime.parse(json['createdAt'])
          : DateTime.now(),
      updatedAt: json['updatedAt'] != null
          ? DateTime.parse(json['updatedAt'])
          : DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'title': title,
      'description': description,
      'poster': poster,
      'genres': genres,
      'price': price,
      'durationMinutes': durationMinutes,
      'producer': producer,
      'inRoles': inRoles,
      'ageLimit': ageLimit,
      'releaseDate': releaseDate.toIso8601String(),
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }

  Movie copyWith({
    String? id,
    String? title,
    String? description,
    String? poster,
    List<String>? genres,
    double? price,
    int? durationMinutes,
    String? producer,
    String? inRoles,
    int? ageLimit,
    DateTime? releaseDate,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) {
    return Movie(
      id: id ?? this.id,
      title: title ?? this.title,
      description: description ?? this.description,
      poster: poster ?? this.poster,
      genres: genres ?? this.genres,
      price: price ?? this.price,
      durationMinutes: durationMinutes ?? this.durationMinutes,
      producer: producer ?? this.producer,
      inRoles: inRoles ?? this.inRoles,
      ageLimit: ageLimit ?? this.ageLimit,
      releaseDate: releaseDate ?? this.releaseDate,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }
}
