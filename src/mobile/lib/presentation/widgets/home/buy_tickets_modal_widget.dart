// lib/presentation/widgets/buy_tickets_modal.dart
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:untitledCinema/presentation/screens/movie_session_screen.dart';

import '../../../config/theme.dart';
import '../../../domain/entities/movie/movie.dart';
import '../../../domain/entities/session/session.dart';
import '../../providers/session_provider.dart';

class BuyTicketsModalWidget extends StatefulWidget {
  final Movie movie;

  const BuyTicketsModalWidget({super.key, required this.movie});

  @override
  State<BuyTicketsModalWidget> createState() => _BuyTicketsModalWidgetState();
}

class _BuyTicketsModalWidgetState extends State<BuyTicketsModalWidget> {
  DateTime _selectedDate = DateTime.now();
  List<Session> _sessions = [];
  bool _isLoading = false;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    _loadSessions();
    _ensureSelectedDateIsValid();
  }

  void _ensureSelectedDateIsValid() {
    final DateTime minDate = DateTime.now().subtract(const Duration(days: 1));
    final DateTime maxDate = DateTime.now().add(const Duration(days: 10));

    final DateTime selectedDateNormalized = DateTime(
      _selectedDate.year,
      _selectedDate.month,
      _selectedDate.day,
    );

    final DateTime minDateNormalized = DateTime(
      minDate.year,
      minDate.month,
      minDate.day,
    );

    final DateTime maxDateNormalized = DateTime(
      maxDate.year,
      maxDate.month,
      maxDate.day,
    );

    if (selectedDateNormalized.isBefore(minDateNormalized)) {
      setState(() {
        _selectedDate = minDate;
      });
    }

    if (selectedDateNormalized.isAfter(maxDateNormalized)) {
      setState(() {
        _selectedDate = maxDate;
      });
    }
  }

  Future<void> _loadSessions() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      final provider = Provider.of<SessionProvider>(context, listen: false);
      // Форматируем дату в требуемый формат DD-MM-YYYY
      final formattedDate =
          '${_selectedDate.day.toString().padLeft(2, '0')}-'
          '${_selectedDate.month.toString().padLeft(2, '0')}-'
          '${_selectedDate.year}';

      _sessions = await provider.fetchSessionsByMovie(
        movieId: widget.movie.id,
        date: formattedDate,
      );

      final qweqwe = _sessions;

      setState(() {
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
        _errorMessage = 'Ошибка при загрузке сеансов: ${e.toString()}';
      });
    }
  }

  void navigateToMovieSession(BuildContext context, Session session) {
    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (context) => MovieSessionScreen(session: session),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: Theme.of(context).scaffoldBackgroundColor,
        borderRadius: const BorderRadius.only(
          topLeft: Radius.circular(16),
          topRight: Radius.circular(16),
        ),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Заголовок
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  'Выбор сеанса',
                  style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
                ),
                IconButton(
                  icon: const Icon(Icons.close),
                  onPressed: () => Navigator.of(context).pop(),
                ),
              ],
            ),
          ),

          const Divider(),

          // Выбор даты
          const Padding(
            padding: EdgeInsets.symmetric(horizontal: 16.0),
            child: Text(
              'Выберите дату',
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
            ),
          ),

          // Календарь с датами
          SizedBox(
            height: 80,
            child: ListView.builder(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 12.0),
              itemCount: 18,
              itemBuilder: (context, index) {
                final date = DateTime.now()
                    .subtract(const Duration(days: 1))
                    .add(Duration(days: index));
                final isSelected =
                    _selectedDate.year == date.year &&
                    _selectedDate.month == date.month &&
                    _selectedDate.day == date.day;
                final isToday =
                    DateTime.now().year == date.year &&
                    DateTime.now().month == date.month &&
                    DateTime.now().day == date.day;

                // Новое условие: если выбрана не сегодняшняя дата, а текущая ячейка - сегодня
                final isTodayWithOtherSelected =
                    isToday &&
                    !isSelected &&
                    (_selectedDate.year != DateTime.now().year ||
                        _selectedDate.month != DateTime.now().month ||
                        _selectedDate.day != DateTime.now().day);

                return GestureDetector(
                  onTap: () {
                    setState(() {
                      _selectedDate = date;
                    });
                    _loadSessions();
                  },
                  child: Container(
                    width: 70,
                    margin: const EdgeInsets.symmetric(
                      horizontal: 4,
                      vertical: 8,
                    ),
                    padding: const EdgeInsets.all(8),
                    decoration: BoxDecoration(
                      color:
                          isSelected
                              ? AppTheme.accentColor
                              : isToday
                              ? AppTheme.primaryColor.withOpacity(0.2)
                              : Colors.grey.shade200,
                      borderRadius: BorderRadius.circular(12),
                      border:
                          isToday && !isSelected
                              ? Border.all(
                                color: AppTheme.primaryColor,
                                width: 1,
                              )
                              : null,
                    ),
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Text(
                          _getDayOfWeek(date),
                          style: TextStyle(
                            color:
                                isSelected
                                    ? Colors.white
                                    : isTodayWithOtherSelected
                                    ? Colors.white
                                    : Colors.black87,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                        Text(
                          '${date.day}',
                          style: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color:
                                isSelected
                                    ? Colors.white
                                    : isTodayWithOtherSelected
                                    ? Colors.white
                                    : Colors.black87,
                          ),
                        ),
                      ],
                    ),
                  ),
                );
              },
            ),
          ),

          const Divider(),

          // Сессии на выбранную дату
          const Padding(
            padding: EdgeInsets.all(16.0),
            child: Text(
              'Доступные сеансы',
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
            ),
          ),

          // Содержимое в зависимости от состояния
          _buildSessionsContent(),
        ],
      ),
    );
  }

  Widget _buildSessionsContent() {
    if (_isLoading) {
      return const Center(
        child: Padding(
          padding: EdgeInsets.all(24.0),
          child: CircularProgressIndicator(),
        ),
      );
    }

    if (_errorMessage != null) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(24.0),
          child: Column(
            children: [
              Text(_errorMessage!),
              const SizedBox(height: 12),
              ElevatedButton(
                onPressed: _loadSessions,
                child: const Text('Попробовать снова'),
              ),
            ],
          ),
        ),
      );
    }

    if (_sessions.isEmpty) {
      return const Center(
        child: Padding(
          padding: EdgeInsets.all(24.0),
          child: Text('Нет доступных сеансов на выбранную дату'),
        ),
      );
    }

    // Группируем сессии по залам для более удобного отображения
    final sessionsByHall = <String, List<Session>>{};
    for (var session in _sessions) {
      if (!sessionsByHall.containsKey(session.hall.name)) {
        sessionsByHall[session.hall.name] = [];
      }
      sessionsByHall[session.hall.name]!.add(session);
    }
    final qwe = sessionsByHall;

    return Expanded(
      child: ListView.builder(
        itemCount: sessionsByHall.keys.length,
        itemBuilder: (context, index) {
          final hallName = sessionsByHall.keys.elementAt(index);
          final hallSessions = sessionsByHall[hallName]!;

          return Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: 16.0,
                  vertical: 8.0,
                ),
                child: Text(
                  hallName,
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                  ),
                ),
              ),
              SizedBox(
                height: 120,
                child: ListView.builder(
                  scrollDirection: Axis.horizontal,
                  padding: const EdgeInsets.symmetric(horizontal: 12.0),
                  itemCount: hallSessions.length,
                  itemBuilder: (context, idx) {
                    final session = hallSessions[idx];
                    // Вычисляем цену с учетом модификатора
                    final price = widget.movie.price * session.priceModifier;

                    return GestureDetector(
                      onTap: () {
                        // Действие при выборе сеанса, например переход к выбору мест
                        navigateToMovieSession(context, session);
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Text(
                              'Выбран сеанс в ${hallName} на ${_formatTime(session.startTime)}',
                            ),
                          ),
                        );
                      },
                      child: Container(
                        width: 140,
                        margin: const EdgeInsets.all(4),
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          border: Border.all(color: Colors.white),
                          borderRadius: BorderRadius.circular(12),
                          color: AppTheme.primaryColor,
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Text(
                              _formatTime(session.startTime),
                              style: const TextStyle(
                                fontSize: 18,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            const SizedBox(height: 4),
                            Text('до ${_formatTime(session.endTime)}'),
                            const Spacer(),
                            Text(
                              '${price.toStringAsFixed(0)} Br',
                              style: TextStyle(
                                fontWeight: FontWeight.bold,
                                color: Colors.white,
                              ),
                            ),
                          ],
                        ),
                      ),
                    );
                  },
                ),
              ),
              const Divider(),
            ],
          );
        },
      ),
    );
  }

  String _getDayOfWeek(DateTime date) {
    const daysOfWeek = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];
    // Учитываем, что в DateTime дни недели начинаются с 1 (понедельник) до 7 (воскресенье)
    return daysOfWeek[date.weekday - 1];
  }

  String _formatTime(DateTime dateTime) {
    return '${dateTime.hour.toString().padLeft(2, '0')}:${dateTime.minute.toString().padLeft(2, '0')}';
  }
}
