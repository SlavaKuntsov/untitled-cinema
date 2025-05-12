// lib/presentation/widgets/hall_seat_map.dart
import 'package:flutter/material.dart';

import '../../../domain/entities/session/hall.dart';
import '../../../domain/entities/session/seat_type.dart';

class HallSeatMapWidget extends StatefulWidget {
  final Hall hall;
  final List<SeatType> seatTypes;
  final Map<String, bool> selectedSeats; // Map<"row-seat", isSelected>
  final Function(int row, int seat) onSeatTap;

  const HallSeatMapWidget({
    super.key,
    required this.hall,
    required this.seatTypes,
    required this.selectedSeats,
    required this.onSeatTap,
  });

  @override
  State<HallSeatMapWidget> createState() => _HallSeatMapWidgetState();
}

class _HallSeatMapWidgetState extends State<HallSeatMapWidget> {
  // Получает цвет для сиденья на основе его типа
  Color _getSeatColor(int seatType, bool isSelected) {
    if (isSelected) {
      return Colors.green; // Выбранное сиденье
    }

    // Находим тип сиденья в списке и используем его для определения цвета
    final typeId = seatType.toString();
    final type = widget.seatTypes.firstWhere(
      (type) => type.id == typeId,
      orElse: () => const SeatType(id: '0', name: 'Стандартное'),
    );

    // Определяем цвет в зависимости от модификатора цены
    if (type.priceModifier > 1.5) {
      return Colors.red; // VIP или премиум места
    } else if (type.priceModifier > 1.0) {
      return Colors.orange; // Улучшенные места
    } else {
      return Colors.blue; // Стандартные места
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // Заголовок с информацией о зале
        // Padding(
        //   padding: const EdgeInsets.all(16.0),
        //   child: Text(
        //     'Схема зала: ${widget.hall.name}',
        //     style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
        //   ),
        // ),

        // Экран (наверху схемы)
        Container(
          margin: const EdgeInsets.symmetric(vertical: 16),
          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 8),
          decoration: BoxDecoration(
            color: Colors.grey,
            borderRadius: BorderRadius.circular(4),
          ),
          child: const Text(
            'ЭКРАН',
            style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold),
          ),
        ),

        // Схема сидений
        Expanded(
          child: InteractiveViewer(
            constrained: false,
            minScale: 0.5,
            maxScale: 2.0,
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: List.generate(widget.hall.rowCount, (rowIndex) {
                  final row = widget.hall.seatsArray[rowIndex];

                  return Padding(
                    padding: const EdgeInsets.symmetric(vertical: 4.0),
                    child: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        // Номер ряда
                        Container(
                          width: 24,
                          alignment: Alignment.center,
                          child: Text(
                            '${rowIndex + 1}',
                            style: const TextStyle(fontWeight: FontWeight.bold),
                          ),
                        ),

                        // Сиденья в ряду
                        ...List.generate(row.length, (seatIndex) {
                          final seatValue = row[seatIndex];
                          final isValidSeat = seatValue > 0;
                          final seatKey = '$rowIndex-$seatIndex';
                          final isSelected =
                              widget.selectedSeats[seatKey] ?? false;

                          return Padding(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 2.0,
                            ),
                            child:
                                isValidSeat
                                    ? GestureDetector(
                                      onTap:
                                          () => widget.onSeatTap(
                                            rowIndex,
                                            seatIndex,
                                          ),
                                      child: Container(
                                        width: 32,
                                        height: 32,
                                        alignment: Alignment.center,
                                        decoration: BoxDecoration(
                                          color: _getSeatColor(
                                            seatValue,
                                            isSelected,
                                          ),
                                          borderRadius: BorderRadius.circular(
                                            4,
                                          ),
                                        ),
                                        child: Text(
                                          '${seatIndex + 1}',
                                          style: const TextStyle(
                                            color: Colors.white,
                                            fontWeight: FontWeight.bold,
                                          ),
                                        ),
                                      ),
                                    )
                                    : Container(width: 32, height: 32),
                          );
                        }),
                      ],
                    ),
                  );
                }),
              ),
            ),
          ),
        ),

        // Легенда типов сидений
        Padding(
          padding: const EdgeInsets.all(16.0),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: [
              _buildLegendItem(Colors.blue, 'Стандартное'),
              _buildLegendItem(Colors.orange, 'Улучшенное'),
              _buildLegendItem(Colors.red, 'VIP'),
              _buildLegendItem(Colors.green, 'Выбрано'),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildLegendItem(Color color, String text) {
    return Row(
      children: [
        Container(
          width: 16,
          height: 16,
          decoration: BoxDecoration(
            color: color,
            borderRadius: BorderRadius.circular(2),
          ),
        ),
        const SizedBox(width: 4),
        Text(text),
      ],
    );
  }
}
