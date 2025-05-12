// lib/presentation/widgets/seat_map_widget.dart
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:untitledCinema/domain/entities/session/selected_seat.dart';

import '../../../domain/entities/session/hall.dart';
import '../../../domain/entities/session/seat_type.dart';
import '../../providers/session_provider.dart';

class SeatMapWidget extends StatefulWidget {
  final Hall hall;
  final String sessionId;
  final List<SeatType> seatTypes;
  final Function(List<SelectedSeat>) onSeatsSelected;

  const SeatMapWidget({
    super.key,
    required this.hall,
    required this.sessionId,
    required this.seatTypes,
    required this.onSeatsSelected,
  });

  @override
  State<SeatMapWidget> createState() => _SeatMapWidgetState();
}
//
// class SeatPosition {
//   final int row;
//   final int seat;
//   final int type;
//   final double price;
//
//   SeatPosition({
//     required this.row,
//     required this.seat,
//     required this.type,
//     required this.price,
//   });
//
//   @override
//   String toString() {
//     return 'Ряд ${row + 1}, Место ${seat + 1}';
//   }
// }

class _SeatMapWidgetState extends State<SeatMapWidget> {
  late SessionProvider _sessionProvider;
  final List<SelectedSeat> _selectedSeats = [];
  double _basePrice = 250.0;
  bool _isLoading = false;
  bool _isInitialized = false;

  @override
  void initState() {
    super.initState();
    _sessionProvider = Provider.of<SessionProvider>(context, listen: false);
    _initializeSeats();
  }

  Future<void> _initializeSeats() async {
    try {
      await _sessionProvider.fetchReservedSeats(widget.sessionId);

      await _sessionProvider.startSeatsConnection(widget.sessionId);

      setState(() {
        _isInitialized = true;
      });
    } catch (e) {
      print('Ошибка при инициализации мест: $e');
    }
  }

  @override
  void dispose() {
    _sessionProvider.stopSeatsConnection(widget.sessionId);
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<SessionProvider>(
      builder: (context, provider, child) {
        if (!_isInitialized) {
          return const Center(child: CircularProgressIndicator());
        }

        return Column(
          children: [
            // Экран (вверху схемы)
            Container(
              width: double.infinity,
              padding: const EdgeInsets.symmetric(vertical: 8),
              margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 8),
              decoration: BoxDecoration(
                color: Colors.grey.shade800,
                borderRadius: BorderRadius.circular(4),
              ),
              child: const Center(
                child: Text(
                  'ЭКРАН',
                  style: TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ),

            // Схема зала
            Expanded(
              child: LayoutBuilder(
                builder: (context, constraints) {
                  // Определяем максимальное количество мест в ряду
                  int maxSeatsInRow = 0;
                  for (final row in widget.hall.seatsArray) {
                    if (row.length > maxSeatsInRow) {
                      maxSeatsInRow = row.length;
                    }
                  }

                  // Рассчитываем размер места на основе доступной ширины
                  final double availableWidth =
                      constraints.maxWidth -
                      40; // 40px для номера ряда и отступов
                  final double seatSize = (availableWidth / maxSeatsInRow)
                      .clamp(30.0, 50.0);

                  return SingleChildScrollView(
                    child: Column(
                      children: List.generate(widget.hall.seatsArray.length, (
                        rowIndex,
                      ) {
                        final List<int> row = widget.hall.seatsArray[rowIndex];

                        return Padding(
                          padding: const EdgeInsets.symmetric(vertical: 4),
                          child: Row(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              // Номер ряда
                              Container(
                                width: 30,
                                height: seatSize,
                                alignment: Alignment.center,
                                child: Text(
                                  '${rowIndex + 1}',
                                  style: const TextStyle(
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ),

                              // Места в ряду
                              Row(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: List.generate(row.length, (
                                  seatIndex,
                                ) {
                                  final int seatValue = row[seatIndex];

                                  // Если значение <= 0, это проход или пустое место
                                  if (seatValue <= 0) {
                                    return SizedBox(
                                      width: seatSize,
                                      height: seatSize,
                                    );
                                  }

                                  // Проверяем, выбрано ли это место
                                  final bool isSelected = provider.selectedSeats
                                      .any(
                                        (seat) =>
                                            seat.row == (rowIndex + 1) &&
                                            seat.column == (seatIndex + 1),
                                      );

                                  // Проверяем, забронировано ли это место
                                  final bool isReserved = provider.isReserved(
                                    rowIndex,
                                    seatIndex,
                                  );

                                  // Определяем цвет места
                                  Color seatColor;
                                  if (isReserved) {
                                    seatColor =
                                        Colors
                                            .grey; // Забронированные места серые
                                  } else if (isSelected) {
                                    seatColor =
                                        Colors.green; // Выбранные места зеленые
                                  } else {
                                    seatColor = provider.getSeatColor(
                                      seatValue,
                                    ); // По типу места
                                  }

                                  return GestureDetector(
                                    onTap:
                                        isReserved
                                            ? null // Забронированные места нельзя выбирать
                                            : () async {
                                              await provider
                                                  .toggleSeatSelection(
                                                    rowIndex,
                                                    seatIndex,
                                                    widget.sessionId,
                                                  );
                                              widget.onSeatsSelected(
                                                provider.selectedSeats,
                                              );
                                            },
                                    child: Container(
                                      width: seatSize - 4,
                                      height: seatSize - 4,
                                      margin: const EdgeInsets.all(2),
                                      decoration: BoxDecoration(
                                        color: seatColor,
                                        borderRadius: BorderRadius.circular(4),
                                        border: Border.all(
                                          color: Colors.white,
                                          width: 1,
                                        ),
                                      ),
                                      child: Center(
                                        child: Text(
                                          '${seatIndex + 1}',
                                          style: const TextStyle(
                                            color: Colors.white,
                                            fontWeight: FontWeight.bold,
                                            fontSize: 12,
                                          ),
                                        ),
                                      ),
                                    ),
                                  );
                                }),
                              ),
                            ],
                          ),
                        );
                      }),
                    ),
                  );
                },
              ),
            ),

            // Легенда типов мест
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
              child: Wrap(
                spacing: 16,
                runSpacing: 8,
                children: [
                  ...List.generate(widget.seatTypes.length, (index) {
                    // Список цветов
                    final List<Color> colors = [
                      Colors.blue, // Стандарт
                      Colors.orange, // Комфорт
                      Colors.red, // Премиум
                      Colors.purple, // VIP
                    ];

                    // Список названий
                    final List<String> names = [
                      'Стандарт',
                      'Комфорт',
                      'Премиум',
                      'VIP',
                    ];

                    // Используем индекс для выбора цвета и названия
                    final color =
                        index < colors.length ? colors[index] : Colors.grey;
                    final name =
                        index < names.length
                            ? names[index]
                            : 'Тип ${index + 1}';

                    return _buildLegendItem(color, name);
                  }),
                  _buildLegendItem(Colors.green, 'Выбрано'),
                ],
              ),
            ),
          ],
        );
      },
    );
  }

  // // Обработка выбора/отмены выбора места
  // void _toggleSeatSelection(int row, int seat, int type) {
  //   setState(() {
  //     // Проверяем, выбрано ли уже это место
  //     final int index = _selectedSeats.indexWhere(
  //       (s) => s.row == row && s.column == seat,
  //     );
  //
  //     if (index != -1) {
  //       // Если место уже выбрано, удаляем его
  //       _selectedSeats.removeAt(index);
  //     } else {
  //       // Иначе добавляем его в список выбранных
  //       final double price = _calculateSeatPrice(type);
  //       _selectedSeats.add(
  //         SelectedSeat(row: row, column: seat, seatType: type, price: price),
  //       );
  //     }
  //
  //     // Вызываем callback с обновленным списком выбранных мест
  //     widget.onSeatsSelected(_selectedSeats);
  //   });
  // }

  // Расчет цены места в зависимости от типа
  double _calculateSeatPrice(int type) {
    // Находим тип места в списке типов
    final SeatType? seatType = _findSeatType(type);

    if (seatType != null) {
      return _basePrice * seatType.priceModifier;
    }

    return _basePrice; // Базовая цена по умолчанию
  }

  // Поиск типа места по его id
  SeatType? _findSeatType(int type) {
    if (widget.seatTypes.isEmpty) return null;

    // Ищем соответствующий тип места в цикле
    for (var seatType in widget.seatTypes) {
      if (seatType.id == type.toString()) {
        return seatType;
      }
    }

    // Если не найдено, возвращаем null
    return null;
  }

  // Получение цвета в зависимости от типа места
  Color _getSeatColor(int type) {
    final SeatType? seatType = _findSeatType(type);

    if (seatType == null) {
      return Colors.blue; // Стандартный цвет по умолчанию
    }

    // Определяем цвет в зависимости от множителя цены
    if (seatType.priceModifier >= 1.5) {
      return Colors.purple; // VIP места
    } else if (seatType.priceModifier >= 1.3) {
      return Colors.red; // Премиум места
    } else if (seatType.priceModifier > 1.0) {
      return Colors.orange; // Комфорт места
    } else {
      return Colors.blue; // Стандартные места
    }
  }

  // Виджет элемента легенды
  Widget _buildLegendItem(Color color, String text) {
    return Row(
      mainAxisSize: MainAxisSize.min,
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
