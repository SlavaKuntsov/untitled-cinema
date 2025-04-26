import 'package:equatable/equatable.dart';

class PaginatedItems<T> extends Equatable {
  final List<T> items;
  final int limit;
  final int offset;
  final int total;
  final String nextRef;
  final String prevRef;

  const PaginatedItems({
    required this.items,
    required this.limit,
    required this.offset,
    required this.total,
    this.nextRef = '',
    this.prevRef = '',
  });

  @override
  List<Object?> get props => [items, limit, offset, total, nextRef, prevRef];
}
