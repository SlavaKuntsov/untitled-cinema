// lib/presentation/screens/booking/history_screen.dart
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../../core/constants/booking_constants.dart';
import '../providers/auth_provider.dart';
import '../providers/booking_provider.dart';
import '../widgets/bookings/booking_list_item.dart';
import '../widgets/bookings/bookings_pagination_widget.dart';

class HistoryScreen extends StatefulWidget {
  const HistoryScreen({super.key});

  @override
  State<HistoryScreen> createState() => _HistoryScreenState();
}

class _HistoryScreenState extends State<HistoryScreen> {
  late BookingProvider _bookingProvider;
  late AuthProvider _authProvider;
  late final String userId;

  final ScrollController _scrollController = ScrollController();

  BookingStatus? _selectedStatus;
  DateTime? _selectedDate;

  final List<int> _pageSizes = [1, 5, 10, 20];
  int selectedPageSize = 10;

  @override
  void initState() {
    super.initState();
    _bookingProvider = Provider.of<BookingProvider>(context, listen: false);
    _authProvider = Provider.of<AuthProvider>(context, listen: false);

    userId = _authProvider.currentUser!.id;

    _loadBookingHistory();
  }

  void _loadBookingHistory() {
    final userId = _authProvider.currentUser?.id;
    if (userId != null) {
      List<String>? filters;
      List<String>? filterValues;

      if (_selectedStatus != null || _selectedDate != null) {
        filters = [];
        filterValues = [];

        if (_selectedStatus != null) {
          filters.add('status');
          filterValues.add(_selectedStatus!.name);
        }

        // if (_selectedDate != null) {
        //   filters.add('date');
        //   filterValues.add(DateFormat('dd-MM-yyyy').format(_selectedDate!));
        // }
      }

      // Загрузка истории бронирований
      _bookingProvider.fetchBookingHistory(
        userId: userId,
        limit: selectedPageSize,
        page: 1,
        filters: filters,
        filterValues: filterValues,
        sortBy: 'date',
        sortDirection: 'desc',
        date:
            _selectedDate != null
                ? DateFormat('dd-MM-yyyy').format(_selectedDate!)
                : null,
      );
    }
  }

  void _scrollToTop() {
    if (_scrollController.hasClients) {
      _scrollController.animateTo(
        0,
        duration: const Duration(milliseconds: 500), // Скорость анимации
        curve: Curves.easeOut,
      );
    } else {
      debugPrint('Скролл-контроллер не прикреплен к виджету');
    }
  }

  void _updatePageSize(int? newSize) {
    if (newSize != null && newSize != selectedPageSize) {
      setState(() {
        selectedPageSize = newSize;
      });

      Provider.of<BookingProvider>(context, listen: false).fetchBookingHistory(
        userId: _authProvider.currentUser!.id,
        limit: newSize,
        filters: _selectedStatus != null ? ['status'] : null,
        filterValues: _selectedStatus != null ? [_selectedStatus!.name] : null,
        date:
            _selectedDate != null
                ? DateFormat('yyyy-MM-dd').format(_selectedDate!)
                : null,
      );

      _scrollToTop();
    }
  }

  void _clearFilters() {
    setState(() {
      _selectedStatus = null;
      _selectedDate = null;
    });

    _loadBookingHistory();

    _scrollToTop();
  }

  @override
  void dispose() {
    // Не забываем освободить ресурсы при уничтожении виджета
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Consumer<BookingProvider>(
        builder: (context, provider, child) {
          final bookingState = provider.bookingState;

          // Управление состояниями загрузки для всего экрана
          if (bookingState.status == BookingLoadingStatus.initial ||
              (bookingState.status == BookingLoadingStatus.loading &&
                  bookingState.bookings.isEmpty)) {
            return const Center(child: CircularProgressIndicator());
          } else if (bookingState.status == BookingLoadingStatus.error &&
              bookingState.bookings.isEmpty) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text(
                    bookingState.errorMessage ??
                        'Произошла ошибка при загрузке бронирований',
                    textAlign: TextAlign.center,
                    style: TextStyle(color: Colors.red.shade800),
                  ),
                  SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: _loadBookingHistory,
                    child: const Text('Попробовать снова'),
                  ),
                ],
              ),
            );
          }

          // Единая прокрутка для всего контента
          return SingleChildScrollView(
            controller: _scrollController,
            child: Padding(
              padding: const EdgeInsets.only(
                top: 42,
                left: 16,
                right: 16,
                bottom: 16,
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _buildFiltersSection(),
                  const SizedBox(height: 16),
                  _buildHeaderSection(),
                  const SizedBox(height: 16),

                  // Если нет бронирований, показываем сообщение
                  if (bookingState.bookings.isEmpty)
                    const SizedBox(
                      height: 200, // Минимальная высота для сообщения
                      child: Center(
                        child: Text(
                          'У вас еще нет бронирований',
                          style: TextStyle(fontSize: 16),
                        ),
                      ),
                    )
                  else
                    // Список бронирований (без собственной прокрутки)
                    Column(
                      children:
                          bookingState.bookings.map((booking) {
                            return BookingListItem(
                              booking: booking,
                              onCancel:
                                  () => _handleCancelBooking(
                                    booking.id,
                                    provider,
                                  ),
                              onPay:
                                  () => _handlePayBooking(booking.id, provider),
                            );
                          }).toList(),
                    ),

                  // Пагинация остается внизу страницы
                  if (bookingState.bookings.isNotEmpty)
                    BookingsPaginationWidget(
                      bookingsState: bookingState,
                      selectedPageSize: selectedPageSize,
                      pageSizes: _pageSizes,
                      onPageSizeChanged: _updatePageSize,
                      onNextPage: () {
                        provider.nextPage(userId, selectedPageSize);
                        _scrollToTop();
                      },
                      onPrevPage: () {
                        provider.prevPage(userId, selectedPageSize);
                        _scrollToTop();
                      },
                    ),

                  // Добавляем небольшой отступ внизу страницы
                  const SizedBox(height: 20),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Future<void> _handleCancelBooking(
    String bookingId,
    BookingProvider provider,
  ) async {
    // Показываем диалог подтверждения
    final confirmed = await showDialog<bool>(
      context: context,
      builder:
          (context) => AlertDialog(
            title: const Text('Отмена бронирования'),
            content: const Text(
              'Вы уверены, что хотите отменить бронирование?',
            ),
            actions: [
              TextButton(
                onPressed: () => Navigator.of(context).pop(false),
                child: const Text('Отмена'),
              ),
              TextButton(
                onPressed: () => Navigator.of(context).pop(true),
                child: const Text('Подтвердить'),
              ),
            ],
          ),
    );

    if (confirmed == true) {
      final success = await provider.cancelBooking(bookingId);
      _loadBookingHistory();
      if (success) {
        _loadBookingHistory(); // Обновляем список

        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Бронирование успешно отменено'),
              backgroundColor: Colors.green,
            ),
          );
        }
      } else if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              provider.operationError ?? 'Ошибка при отмене бронирования',
            ),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  Future<void> _handlePayBooking(
    String bookingId,
    BookingProvider provider,
  ) async {
    // Показываем диалог подтверждения
    final confirmed = await showDialog<bool>(
      context: context,
      builder:
          (context) => AlertDialog(
            title: const Text('Оплата бронирования'),
            content: const Text(
              'Вы уверены, что хотите оплатить бронирование?',
            ),
            actions: [
              TextButton(
                onPressed: () => Navigator.of(context).pop(false),
                child: const Text('Отмена'),
              ),
              TextButton(
                onPressed: () => Navigator.of(context).pop(true),
                child: const Text('Подтвердить'),
              ),
            ],
          ),
    );

    if (confirmed == true) {
      final success = await provider.payBooking(bookingId, userId);
      _loadBookingHistory();
      if (success) {
        _loadBookingHistory(); // Обновляем список

        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Бронирование успешно оплачено'),
              backgroundColor: Colors.green,
            ),
          );
        }
      } else if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              provider.operationError ?? 'Ошибка при оплате бронирования',
            ),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  Widget _buildFiltersSection() {
    return Card(
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Фильтры',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 12),

            // Фильтр по статусу
            Row(
              children: [
                const Text('Статус: '),
                const SizedBox(width: 8),
                DropdownButton<BookingStatus?>(
                  value: _selectedStatus,
                  hint: const Text('Все статусы'),
                  onChanged: (BookingStatus? value) {
                    setState(() {
                      _selectedStatus = value;
                    });
                    _loadBookingHistory();
                  },
                  items: [
                    const DropdownMenuItem<BookingStatus?>(
                      value: null,
                      child: Text('Все статусы'),
                    ),
                    ...BookingStatus.values.map((status) {
                      return DropdownMenuItem<BookingStatus>(
                        value: status,
                        child: Row(
                          children: [
                            Container(
                              width: 12,
                              height: 12,
                              decoration: BoxDecoration(
                                color: status.color,
                                shape: BoxShape.circle,
                              ),
                            ),
                            const SizedBox(width: 8),
                            Text(status.label),
                          ],
                        ),
                      );
                    }).toList(),
                  ],
                ),
              ],
            ),

            const SizedBox(height: 12),

            // Фильтр по дате
            Row(
              children: [
                const Text('Дата: '),
                const SizedBox(width: 8),
                TextButton(
                  onPressed: () async {
                    final DateTime? pickedDate = await showDatePicker(
                      context: context,
                      initialDate: _selectedDate ?? DateTime.now(),
                      firstDate: DateTime(2020),
                      lastDate: DateTime.now().add(const Duration(days: 365)),
                    );

                    if (pickedDate != null && pickedDate != _selectedDate) {
                      setState(() {
                        _selectedDate = pickedDate;
                      });
                      _loadBookingHistory();
                    }
                  },
                  child: Text(
                    _selectedDate != null
                        ? DateFormat('dd.MM.yyyy').format(_selectedDate!)
                        : 'Выбрать дату',
                  ),
                ),

                if (_selectedDate != null)
                  IconButton(
                    icon: const Icon(Icons.clear),
                    onPressed: () {
                      setState(() {
                        _selectedDate = null;
                      });
                      _loadBookingHistory();
                    },
                  ),
              ],
            ),

            // Кнопка сброса фильтров
            if (_selectedStatus != null || _selectedDate != null)
              Column(
                children: [
                  const SizedBox(height: 12),
                  ElevatedButton(
                    onPressed: _clearFilters,
                    child: const Text('Сбросить фильтры'),
                  ),
                ],
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildHeaderSection() {
    return Consumer<BookingProvider>(
      builder: (context, provider, child) {
        final bookingState = provider.bookingState;

        return Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text(
              'Мои бронирования',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            if (bookingState.status == BookingLoadingStatus.loaded)
              Text(
                'Всего: ${bookingState.total}',
                style: TextStyle(color: Colors.grey.shade600),
              ),
          ],
        );
      },
    );
  }
}
