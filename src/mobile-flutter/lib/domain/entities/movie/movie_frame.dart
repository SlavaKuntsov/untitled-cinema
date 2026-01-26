class MovieFrame {
  final String id;
  final String movieId;
  final String frameName;
  final int order;

  MovieFrame({
    required this.id,
    required this.movieId,
    required this.frameName,
    required this.order,
  });

  factory MovieFrame.fromJson(Map<String, dynamic> json) {
    return MovieFrame(
      id: json['id'],
      movieId: json['movieId'],
      frameName: json['frameName'],
      order: json['order'] ?? 0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'movieId': movieId,
      'frameName': frameName,
      'order': order,
    };
  }

  MovieFrame copyWith({
    String? id,
    String? movieId,
    String? frameName,
    int? order,
  }) {
    return MovieFrame(
      id: id ?? this.id,
      movieId: movieId ?? this.movieId,
      frameName: frameName ?? this.frameName,
      order: order ?? this.order,
    );
  }
} 