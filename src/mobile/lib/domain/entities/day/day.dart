import 'package:equatable/equatable.dart';
import 'package:intl/intl.dart';

class Day extends Equatable {
  final String id;
  final DateTime startTime;
  final DateTime endTime;

  const Day({
    required this.id,
    required this.startTime,
    required this.endTime,
  });

  factory Day.fromJson(Map<String, dynamic> json) {
    // Parse date format 'dd-MM-yyyy HH:mm'
    DateTime parseDateTime(String? dateString) {
      if (dateString == null || dateString.isEmpty) {
        return DateTime.now();
      }
      
      try {
        // First try to parse as standard ISO format
        return DateTime.parse(dateString);
      } catch (e) {
        try {
          // If ISO parsing fails, try the custom format
          return DateFormat('dd-MM-yyyy HH:mm').parse(dateString);
        } catch (e) {
          // If all parsing fails, return current date/time
          return DateTime.now();
        }
      }
    }
    
    return Day(
      id: json['id']?.toString() ?? '',
      startTime: parseDateTime(json['startTime']?.toString()),
      endTime: parseDateTime(json['endTime']?.toString()),
    );
  }

  Map<String, dynamic> toJson() {
    final DateFormat formatter = DateFormat('dd-MM-yyyy HH:mm');
    return {
      'id': id,
      'startTime': formatter.format(startTime),
      'endTime': formatter.format(endTime),
    };
  }

  Day copyWith({
    String? id,
    DateTime? startTime,
    DateTime? endTime,
  }) {
    return Day(
      id: id ?? this.id,
      startTime: startTime ?? this.startTime,
      endTime: endTime ?? this.endTime,
    );
  }

  @override
  List<Object?> get props => [id, startTime, endTime];
} 