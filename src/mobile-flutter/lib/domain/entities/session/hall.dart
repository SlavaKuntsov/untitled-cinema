// lib/domain/entities/session/hall.dart
import 'package:equatable/equatable.dart';

class Hall extends Equatable {
  final String id;
  final String name;
  final int totalSeats;
  final List<List<int>> seatsArray;

  const Hall({
    required this.id,
    required this.name,
    this.totalSeats = 0,
    this.seatsArray = const [],
  });

  /// Получает количество рядов в зале
  int get rowCount => seatsArray.length;

  /// Получает максимальное количество мест в ряду
  int get maxSeatsInRow =>
      seatsArray.isEmpty
          ? 0
          : seatsArray
              .map((row) => row.length)
              .reduce((max, length) => length > max ? length : max);

  /// Проверяет, является ли позиция действительным сиденьем
  /// Возвращает true, если позиция содержит сиденье (значение > 0)
  bool isSeat(int row, int seat) {
    if (row < 0 || row >= seatsArray.length) return false;
    if (seat < 0 || seat >= seatsArray[row].length) return false;
    return seatsArray[row][seat] > 0;
  }

  /// Проверяет, является ли позиция пустым местом
  /// Возвращает true, если позиция не содержит сиденье (значение <= 0)
  bool isEmpty(int row, int seat) {
    if (row < 0 || row >= seatsArray.length) return true;
    if (seat < 0 || seat >= seatsArray[row].length) return true;
    return seatsArray[row][seat] <= 0;
  }

  /// Получает тип сиденья для указанной позиции
  /// Возвращает значение из массива или 0, если позиция недействительна
  int getSeatType(int row, int seat) {
    if (row < 0 || row >= seatsArray.length) return 0;
    if (seat < 0 || seat >= seatsArray[row].length) return 0;
    return seatsArray[row][seat];
  }

  @override
  List<Object?> get props => [id, name, totalSeats, seatsArray];
}
