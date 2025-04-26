import '../../../domain/entities/paginated_items.dart';

class PaginatedResponseModel<T> extends PaginatedItems<T> {
  const PaginatedResponseModel({
    required super.items,
    required super.limit,
    required super.offset,
    required super.total,
    super.nextRef,
    super.prevRef,
  });

  factory PaginatedResponseModel.fromJson(
    Map<String, dynamic> json,
    T Function(Map<String, dynamic>) fromJsonT,
  ) {
    return PaginatedResponseModel<T>(
      items:
          (json['items'] as List?)
              ?.map((item) => fromJsonT(item as Map<String, dynamic>))
              .toList() ??
          [],
      limit: json['limit'] ?? 10,
      offset: json['offset'] ?? 0,
      total: json['total'] ?? 0,
      nextRef: json['nextRef'] ?? '',
      prevRef: json['prevRef'] ?? '',
    );
  }

  Map<String, dynamic> toJson(Map<String, dynamic> Function(T) toJsonT) {
    return {
      'items': items.map((item) => toJsonT(item)).toList(),
      'limit': limit,
      'offset': offset,
      'total': total,
      'nextRef': nextRef,
      'prevRef': prevRef,
    };
  }

  factory PaginatedResponseModel.fromEntity(PaginatedItems<T> entity) {
    return PaginatedResponseModel<T>(
      items: entity.items,
      limit: entity.limit,
      offset: entity.offset,
      total: entity.total,
      nextRef: entity.nextRef,
      prevRef: entity.prevRef,
    );
  }

  PaginatedResponseModel<T> copyWith({
    List<T>? items,
    int? limit,
    int? offset,
    int? total,
    String? nextRef,
    String? prevRef,
  }) {
    return PaginatedResponseModel<T>(
      items: items ?? this.items,
      limit: limit ?? this.limit,
      offset: offset ?? this.offset,
      total: total ?? this.total,
      nextRef: nextRef ?? this.nextRef,
      prevRef: prevRef ?? this.prevRef,
    );
  }
}
