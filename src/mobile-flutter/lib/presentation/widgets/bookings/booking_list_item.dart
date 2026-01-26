// lib/presentation/widgets/booking/booking_list_item.dart
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import 'package:untitledCinema/config/theme.dart';
import 'package:untitledCinema/domain/entities/session/hall.dart';
import 'package:untitledCinema/presentation/providers/movie_provider.dart';

import '../../../core/constants/booking_constants.dart';
import '../../../domain/entities/bookings/bookings.dart';
import '../../../domain/entities/movie/movie.dart';
import '../../../domain/entities/session/session.dart';
import '../../providers/session_provider.dart';
import '../../screens/movie_session_screen.dart';

class BookingListItem extends StatefulWidget {
  final Booking booking;
  final VoidCallback onCancel;
  final VoidCallback onPay;

  const BookingListItem({
    super.key,
    required this.booking,
    required this.onCancel,
    required this.onPay,
  });

  @override
  State<BookingListItem> createState() => _BookingListItemState();
}

class _BookingListItemState extends State<BookingListItem> {
  late SessionProvider _sessionProvider;
  late MovieProvider _movieProvider;
  Session? _session;
  Hall? _hall;
  Movie? _movie;
  bool _isLoading = true;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    _loadSessionData();
  }

  Future<void> _loadSessionData() async {
    try {
      setState(() {
        _isLoading = true;
        _errorMessage = null;
      });

      _sessionProvider = Provider.of<SessionProvider>(context, listen: false);
      _movieProvider = Provider.of<MovieProvider>(context, listen: false);

      final session = await _sessionProvider.fetchSessionById(
        id: widget.booking.sessionId,
      );

      final movie = await _movieProvider.fetchMovieById(id: session.movieId);
      final hall = await _sessionProvider.fetchHallById(id: session.hallId);

      setState(() {
        _session = session;
        _movie = movie;
        _hall = hall;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _errorMessage = 'Не удалось загрузить информацию о сеансе';
        _isLoading = false;
      });
    }
  }

  // Получение статуса бронирования из строки
  BookingStatus _getBookingStatus() {
    return BookingStatus.fromString(widget.booking.status);
  }

  @override
  Widget build(BuildContext context) {
    final status = _getBookingStatus();
    final dateFormat = DateFormat('dd.MM.yyyy HH:mm');

    return GestureDetector(
      onTap: () => navigateToMovieSession(context, _session!),
      child: Card(
        margin: const EdgeInsets.only(bottom: 16),
        elevation: 2,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        child: InkWell(
          borderRadius: BorderRadius.circular(12),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // if (status == BookingStatus.reserved) Text("reserved"),
              //
              // if (status == BookingStatus.cancelled) Text("cancelled"),
              _buildHeaderSection(status, dateFormat),

              _buildSessionSection(),

              _buildSeatsSection(),

              _buildFooterSection(status),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildHeaderSection(BookingStatus status, DateFormat dateFormat) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppTheme.accentColor,
        borderRadius: const BorderRadius.only(
          topLeft: Radius.circular(12),
          topRight: Radius.circular(12),
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Дата бронирования',
                    style: TextStyle(fontSize: 12, color: Colors.grey),
                  ),
                  Text(
                    dateFormat.format(widget.booking.createdAt),
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                ],
              ),
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 6,
                ),
                decoration: BoxDecoration(
                  color: status.color.withOpacity(0.3),
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(color: status.color),
                ),
                child: Text(
                  status.label,
                  style: TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ],
          ),
          if (status == BookingStatus.reserved) ...[
            const SizedBox(height: 8),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
              decoration: BoxDecoration(
                color: Colors.red.withOpacity(0.3),
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: Colors.red),
              ),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const Icon(
                    Icons.warning_amber_rounded,
                    color: Colors.white,
                    size: 16,
                  ),
                  const SizedBox(width: 8),
                  Text(
                    'Будет отменено через 1 минуту',
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 12,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildSessionSection() {
    if (_isLoading) {
      return const Padding(
        padding: EdgeInsets.all(16),
        child: Center(child: CircularProgressIndicator()),
      );
    }

    if (_errorMessage != null) {
      return Padding(
        padding: const EdgeInsets.all(16),
        child: Center(
          child: Column(
            children: [
              Text(
                _errorMessage!,
                style: TextStyle(color: Colors.red.shade800),
              ),
              TextButton(
                onPressed: _loadSessionData,
                child: const Text('Повторить'),
              ),
            ],
          ),
        ),
      );
    }

    if (_session == null) {
      return const Padding(
        padding: EdgeInsets.all(16),
        child: Text('Информация о сеансе недоступна'),
      );
    }

    // Формат даты для отображения времени сеанса
    final timeFormat = DateFormat('dd-MM-yyyy HH:mm');

    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 18, horizontal: 0),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Постер фильма (если есть)
          // if (_movie!.poster.isNotEmpty)
          //   ClipRRect(
          //     borderRadius: BorderRadius.circular(8),
          //     child: Image.network(
          //       _movie!.poster,
          //       width: 80,
          //       height: 120,
          //       fit: BoxFit.cover,
          //       errorBuilder:
          //           (_, __, ___) => Container(
          //             width: 80,
          //             height: 120,
          //             color: Colors.grey.shade300,
          //             child: const Icon(Icons.movie, color: Colors.white),
          //           ),
          //     ),
          //   ),
          // _movie!.poster.isNotEmpty
          //     ? ClipRRect(
          //       borderRadius: BorderRadius.circular(8),
          //       child: CachedNetworkImage(
          //         imageUrl: '${ApiConstants.moviePoster}/${_movie!.poster}',
          //         width: 130,
          //         height: 160,
          //         fit: BoxFit.cover,
          //         placeholder:
          //             (context, url) => Container(
          //               width: 130,
          //               height: 160,
          //               color: Colors.grey[300],
          //               child: const Center(
          //                 child: SizedBox(
          //                   width: 30,
          //                   height: 30,
          //                   child: CircularProgressIndicator(strokeWidth: 2),
          //                 ),
          //               ),
          //             ),
          //         errorWidget: (context, url, error) {
          //           debugPrint('Error loading image: $error');
          //           return const _PlaceholderPoster();
          //         },
          //         fadeInDuration: const Duration(milliseconds: 100),
          //       ),
          //     )
          //     : const _PlaceholderPoster(),
          const SizedBox(width: 16),

          // GestureDetector(
          //   onTap: () => navigateToMovieSession(context, _session!),
          //   child:
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                _movie!.title,
                style: const TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 14),
              Row(
                children: [
                  const Icon(Icons.theater_comedy, size: 16),
                  const SizedBox(width: 8),
                  Text(_hall!.name, style: TextStyle(fontSize: 16)),
                ],
              ),
              const SizedBox(height: 10),
              Row(
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  const Icon(Icons.access_time, size: 16),
                  const SizedBox(width: 8),
                  Text(
                    "${timeFormat.format(_session!.startTime)}\t — \t"
                    "${timeFormat.format(_session!.endTime)}",
                    style: TextStyle(fontSize: 16),
                  ),
                ],
              ),
              const SizedBox(height: 10),
              Row(
                children: [
                  const Icon(Icons.timelapse, size: 16),
                  const SizedBox(width: 8),
                  Text(
                    '${_movie!.durationMinutes} мин',
                    style: TextStyle(fontSize: 16),
                  ),
                ],
              ),
            ],
          ),
          // ),
        ],
      ),
    );
  }

  void navigateToMovieSession(BuildContext context, Session session) {
    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (context) => MovieSessionScreen(session: session),
      ),
    );
  }

  Widget _buildSeatsSection() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Места',
            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 8),
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children:
                widget.booking.seats.map((seat) {
                  return Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 8,
                      vertical: 4,
                    ),
                    decoration: BoxDecoration(
                      color: Colors.blue.withOpacity(0.1),
                      borderRadius: BorderRadius.circular(4),
                      border: Border.all(color: Colors.blue),
                    ),
                    child: Text(
                      'Ряд ${seat.row}, Место ${seat.column}',
                      style: const TextStyle(fontSize: 12),
                    ),
                  );
                }).toList(),
          ),
        ],
      ),
    );
  }

  Widget _buildFooterSection(BookingStatus status) {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Итоговая цена
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                'Итого',
                style: TextStyle(fontSize: 12, color: Colors.grey),
              ),
              Text(
                '${widget.booking.totalPrice.toStringAsFixed(2)} Br',
                style: const TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),

          const SizedBox(height: 16),

          // Кнопки действий
          if (status == BookingStatus.reserved)
            Row(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                // Кнопка отмены (если статус "Reserved")
                SizedBox(
                  width: 150, // Фиксированная ширина кнопки
                  child: ElevatedButton(
                    onPressed: widget.onCancel,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.red,
                      foregroundColor: Colors.white,
                    ),
                    child: const Text('Отменить'),
                  ),
                ),

                const SizedBox(width: 8),

                // Кнопка оплаты (если статус "Reserved")
                SizedBox(
                  width: 150, // Фиксированная ширина кнопки
                  child: ElevatedButton(
                    onPressed: widget.onPay,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.green,
                      foregroundColor: Colors.white,
                    ),
                    child: const Text('Оплатить'),
                  ),
                ),
              ],
            ),
        ],
      ),
    );
  }
}
