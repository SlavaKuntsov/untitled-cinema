import 'package:equatable/equatable.dart';

import 'movie.dart';

class PaginatedMovies extends Equatable {
  final List<Movie> items;
  final int total;
  final int limit;
  final int offset;
  final String nextRef;
  final String prevRef;

  const PaginatedMovies({
    required this.items,
    required this.total,
    required this.limit,
    required this.offset,
    this.nextRef = '',
    this.prevRef = '',
  });

  @override
  List<Object?> get props => [items, total, limit, offset, nextRef, prevRef];
}
