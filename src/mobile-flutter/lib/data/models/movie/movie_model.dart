import '../../../domain/entities/movie/movie.dart';

class MovieModel extends Movie {
  const MovieModel({
    required super.id,
    required super.title,
    required super.description,
    super.poster,
    required super.genres,
    required super.price,
    required super.durationMinutes,
    required super.producer,
    required super.inRoles,
    required super.ageLimit,
    required super.releaseDate,
    required super.createdAt,
    required super.updatedAt,
  });

  factory MovieModel.fromJson(Map<String, dynamic> json) {
    // Функция для безопасного преобразования числовых типов в double
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

    return MovieModel(
      id: json['id']?.toString() ?? '',
      title: json['title']?.toString() ?? '',
      description: json['description']?.toString() ?? '',
      poster: json['poster']?.toString() ?? '',
      genres:
          (json['genres'] as List?)?.map((e) => e.toString()).toList() ?? [],
      price: parseDouble(json['price']),
      durationMinutes: parseInt(json['durationMinutes']),
      producer: json['producer']?.toString() ?? '',
      inRoles: json['inRoles']?.toString() ?? '',
      ageLimit: parseInt(json['ageLimit']),
      releaseDate:
          json['releaseDate'] != null
              ? DateTime.parse(json['releaseDate'].toString())
              : DateTime.now(),
      createdAt:
          json['createdAt'] != null
              ? DateTime.parse(json['createdAt'].toString())
              : DateTime.now(),
      updatedAt:
          json['updatedAt'] != null
              ? DateTime.parse(json['updatedAt'].toString())
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

  factory MovieModel.fromEntity(Movie movie) {
    return MovieModel(
      id: movie.id,
      title: movie.title,
      description: movie.description,
      poster: movie.poster,
      genres: movie.genres,
      price: movie.price,
      durationMinutes: movie.durationMinutes,
      producer: movie.producer,
      inRoles: movie.inRoles,
      ageLimit: movie.ageLimit,
      releaseDate: movie.releaseDate,
      createdAt: movie.createdAt,
      updatedAt: movie.updatedAt,
    );
  }

  MovieModel copyWith({
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
    return MovieModel(
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
