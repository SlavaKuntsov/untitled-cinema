import 'package:intl/intl.dart';

class BookingSeat {
  final String id;
  final int row;
  final int column;

  BookingSeat({
    required this.id,
    required this.row,
    required this.column,
  });

  factory BookingSeat.fromJson(Map<String, dynamic> json) {
    return BookingSeat(
      id: json['id'],
      row: json['row'],
      column: json['column'],
    );
  }
}

class BookingHistory {
  final String id;
  final String userId;
  final String sessionId;
  final List<BookingSeat> seats;
  final double totalPrice;
  final String status;
  final DateTime createdAt;
  final DateTime updatedAt;

  BookingHistory({
    required this.id,
    required this.userId,
    required this.sessionId,
    required this.seats,
    required this.totalPrice,
    required this.status,
    required this.createdAt,
    required this.updatedAt,
  });

  factory BookingHistory.fromJson(Map<String, dynamic> json) {
    return BookingHistory(
      id: json['id'],
      userId: json['userId'],
      sessionId: json['sessionId'],
      seats: (json['seats'] as List)
          .map((seat) => BookingSeat.fromJson(seat))
          .toList(),
      totalPrice: json['totalPrice'].toDouble(),
      status: json['status'],
      createdAt: DateFormat('dd-MM-yyyy HH:mm').parse(json['createdAt']),
      updatedAt: DateFormat('dd-MM-yyyy HH:mm').parse(json['updatedAt']),
    );
  }

  bool get isPaid => status == 'paid';
  bool get isCancelled => status == 'cancelled';
}

class BookingHistoryResponse {
  final List<BookingHistory> items;
  final int limit;
  final int offset;
  final int total;
  final String nextRef;
  final String prevRef;

  BookingHistoryResponse({
    required this.items,
    required this.limit,
    required this.offset,
    required this.total,
    required this.nextRef,
    required this.prevRef,
  });

  factory BookingHistoryResponse.fromJson(Map<String, dynamic> json) {
    return BookingHistoryResponse(
      items: (json['items'] as List)
          .map((item) => BookingHistory.fromJson(item))
          .toList(),
      limit: json['limit'],
      offset: json['offset'],
      total: json['total'],
      nextRef: json['nextRef'] ?? '',
      prevRef: json['prevRef'] ?? '',
    );
  }
} 