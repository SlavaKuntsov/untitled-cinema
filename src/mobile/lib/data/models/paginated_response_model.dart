class PaginatedResponse<T> {
  final List<T> items;
  final int limit;
  final int offset;
  final int total;
  final String nextRef;
  final String prevRef;

  PaginatedResponse({
    required this.items,
    required this.limit,
    required this.offset,
    required this.total,
    this.nextRef = '',
    this.prevRef = '',
  });

  factory PaginatedResponse.fromJson(
    Map<String, dynamic> json,
    T Function(Map<String, dynamic>) fromJsonT,
  ) {
    return PaginatedResponse<T>(
      items:
          (json['items'] as List)
              .map((item) => fromJsonT(item as Map<String, dynamic>))
              .toList(),
      limit: json['limit'] ?? 10,
      offset: json['offset'] ?? 0,
      total: json['total'] ?? 0,
      nextRef: json['nextRef'] ?? '',
      prevRef: json['prevRef'] ?? '',
    );
  }
}
