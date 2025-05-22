import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../providers/booking_statistics_provider.dart';

class BookingStatisticsScreen extends StatefulWidget {
  const BookingStatisticsScreen({Key? key}) : super(key: key);

  @override
  State<BookingStatisticsScreen> createState() =>
      _BookingStatisticsScreenState();
}

class _BookingStatisticsScreenState extends State<BookingStatisticsScreen> {
  late BookingStatisticsProvider _statisticsProvider;
  final DateFormat _dateFormat = DateFormat('dd-MM-yyyy');

  @override
  void initState() {
    super.initState();
    _statisticsProvider = Provider.of<BookingStatisticsProvider>(
      context,
      listen: false,
    );
    _loadData();
  }

  Future<void> _loadData() async {
    await _statisticsProvider.fetchBookingHistory();
  }

  Future<void> _selectDateRange() async {
    final DateTimeRange? picked = await showDateRangePicker(
      context: context,
      initialDateRange: DateTimeRange(
        start: _statisticsProvider.startDate,
        end: _statisticsProvider.endDate,
      ),
      firstDate: DateTime(2020),
      lastDate: DateTime.now(),
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: ColorScheme.dark(
              primary: AppTheme.accentColor,
              onPrimary: Colors.white,
              onSurface: Colors.white,
              surface: AppTheme.primaryColor,
              secondary: AppTheme.accentColor,
              onSecondary: Colors.white,
            ),
            textButtonTheme: TextButtonThemeData(
              style: TextButton.styleFrom(foregroundColor: Colors.white),
            ),
            dialogBackgroundColor: AppTheme.primaryColor,
            datePickerTheme: DatePickerThemeData(
              headerBackgroundColor: AppTheme.primaryColor,
              rangePickerBackgroundColor: AppTheme.primaryColor,
              rangePickerHeaderBackgroundColor: AppTheme.primaryColor,
              rangeSelectionBackgroundColor: AppTheme.accentColor.withOpacity(
                0.3,
              ),
              dayBackgroundColor: MaterialStateProperty.resolveWith<Color>((
                states,
              ) {
                if (states.contains(MaterialState.selected)) {
                  return AppTheme.accentColor;
                }
                return Colors.transparent;
              }),
              todayBackgroundColor: MaterialStateProperty.resolveWith<Color>((
                states,
              ) {
                if (states.contains(MaterialState.selected)) {
                  return AppTheme.accentColor;
                }
                return Colors.transparent;
              }),
              // rangeSelectionColor: AppTheme.accentColor.withOpacity(0.3),
              todayForegroundColor: MaterialStateProperty.all(
                AppTheme.accentColor,
              ),
              dayForegroundColor: MaterialStateProperty.resolveWith<Color>((
                states,
              ) {
                if (states.contains(MaterialState.selected)) {
                  return Colors.white;
                }
                return Colors.white;
              }),
            ),
          ),
          child: child!,
        );
      },
    );

    if (picked != null) {
      _statisticsProvider.setDateRange(picked.start, picked.end);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Статистика продаж'),
        backgroundColor: AppTheme.primaryColor,
        actions: [
          IconButton(
            icon: const Icon(Icons.date_range),
            onPressed: _selectDateRange,
          ),
          IconButton(icon: const Icon(Icons.refresh), onPressed: _loadData),
        ],
      ),
      body: Consumer<BookingStatisticsProvider>(
        builder: (context, provider, child) {
          if (provider.isLoading) {
            return const Center(child: CircularProgressIndicator());
          }

          if (provider.errorMessage != null) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text(
                    provider.errorMessage!,
                    style: const TextStyle(color: Colors.red),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: _loadData,
                    child: const Text('Повторить'),
                  ),
                ],
              ),
            );
          }

          return _buildStatisticsContent(provider);
        },
      ),
    );
  }

  Widget _buildStatisticsContent(BookingStatisticsProvider provider) {
    return RefreshIndicator(
      onRefresh: _loadData,
      child: SingleChildScrollView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildDateRangeCard(provider),
            const SizedBox(height: 16),
            _buildSummaryCards(provider),
            const SizedBox(height: 24),
            _buildRevenueChart(provider),
            const SizedBox(height: 24),
            _buildBookingsList(provider),
          ],
        ),
      ),
    );
  }

  Widget _buildDateRangeCard(BookingStatisticsProvider provider) {
    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Период',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                Expanded(
                  child: Text(
                    'С ${_dateFormat.format(provider.startDate)} по ${_dateFormat.format(provider.endDate)}',
                    style: const TextStyle(fontSize: 16),
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.calendar_month),
                  onPressed: _selectDateRange,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSummaryCards(BookingStatisticsProvider provider) {
    return Row(
      children: [
        Expanded(
          child: _buildSummaryCard(
            title: 'Выручка',
            value: '${provider.totalRevenue.toStringAsFixed(2)} руб.',
            icon: Icons.paid,
            color: Colors.green,
          ),
        ),
        const SizedBox(width: 16),
        Expanded(
          child: _buildSummaryCard(
            title: 'Продано билетов',
            value: provider.totalTicketsSold.toString(),
            icon: Icons.confirmation_number,
            color: Colors.orange,
          ),
        ),
      ],
    );
  }

  Widget _buildSummaryCard({
    required String title,
    required String value,
    required IconData icon,
    required Color color,
  }) {
    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(icon, color: color),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    title,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              value,
              style: TextStyle(
                fontSize: 22,
                fontWeight: FontWeight.bold,
                color: color,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildRevenueChart(BookingStatisticsProvider provider) {
    final days = provider.chartDays;
    final revenueData = provider.chartRevenue;

    if (days.isEmpty || revenueData.isEmpty) {
      return const Card(
        child: Padding(
          padding: EdgeInsets.all(16.0),
          child: Center(
            child: Text(
              'Недостаточно данных для построения графика',
              style: TextStyle(fontSize: 16),
            ),
          ),
        ),
      );
    }

    // For displaying full date in tooltip
    final fullDates = provider.chartFullDates;

    // Find the maximum value to set the max Y value
    final maxRevenue = revenueData.fold(
      0.0,
      (max, value) => value > max ? value : max,
    );

    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Динамика продаж',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            SizedBox(
              height: 250,
              child: BarChart(
                BarChartData(
                  alignment: BarChartAlignment.spaceAround,
                  maxY: maxRevenue > 0 ? maxRevenue * 1.2 : 100,
                  barTouchData: BarTouchData(
                    enabled: true,
                    touchTooltipData: BarTouchTooltipData(
                      // tooltipBgColor: Colors.black54,
                      getTooltipItem: (group, groupIndex, rod, rodIndex) {
                        return BarTooltipItem(
                          '${fullDates[groupIndex]}\n${rod.toY.toStringAsFixed(2)} руб.',
                          const TextStyle(color: Colors.white),
                        );
                      },
                    ),
                  ),
                  titlesData: FlTitlesData(
                    show: true,
                    bottomTitles: AxisTitles(
                      sideTitles: SideTitles(
                        showTitles: true,
                        getTitlesWidget: (value, meta) {
                          if (value.toInt() >= 0 &&
                              value.toInt() < days.length) {
                            // Show fewer labels if we have many days
                            if (days.length > 10 && value.toInt() % 3 != 0) {
                              return const SizedBox();
                            }
                            return Padding(
                              padding: const EdgeInsets.only(top: 8.0),
                              child: Text(
                                days[value.toInt()],
                                style: const TextStyle(
                                  fontSize: 10,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            );
                          }
                          return const SizedBox();
                        },
                      ),
                    ),
                    leftTitles: AxisTitles(
                      sideTitles: SideTitles(
                        showTitles: true,
                        getTitlesWidget: (value, meta) {
                          return Text(
                            value.toInt().toString(),
                            style: const TextStyle(
                              fontSize: 10,
                              fontWeight: FontWeight.bold,
                            ),
                          );
                        },
                        reservedSize: 40,
                      ),
                    ),
                    topTitles: AxisTitles(
                      sideTitles: SideTitles(showTitles: false),
                    ),
                    rightTitles: AxisTitles(
                      sideTitles: SideTitles(showTitles: false),
                    ),
                  ),
                  borderData: FlBorderData(show: false),
                  gridData: FlGridData(
                    show: true,
                    horizontalInterval: maxRevenue > 0 ? maxRevenue / 5 : 20,
                    getDrawingHorizontalLine:
                        (value) => FlLine(
                          color: Colors.grey.withOpacity(0.2),
                          strokeWidth: 1,
                        ),
                  ),
                  barGroups: List.generate(days.length, (index) {
                    return BarChartGroupData(
                      x: index,
                      barRods: [
                        BarChartRodData(
                          toY: revenueData[index],
                          color: AppTheme.accentColor,
                          width: days.length > 15 ? 8 : 16,
                          borderRadius: const BorderRadius.only(
                            topLeft: Radius.circular(4),
                            topRight: Radius.circular(4),
                          ),
                        ),
                      ],
                    );
                  }),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildBookingsList(BookingStatisticsProvider provider) {
    final bookings =
        provider.bookingHistory.where((booking) => booking.isPaid).toList();

    if (bookings.isEmpty) {
      return const Card(
        child: Padding(
          padding: EdgeInsets.all(16.0),
          child: Center(
            child: Text(
              'Нет данных о продажах за выбранный период',
              style: TextStyle(fontSize: 16),
            ),
          ),
        ),
      );
    }

    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Последние продажи',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            ...bookings
                .take(5)
                .map((booking) => _buildBookingItem(booking))
                .toList(),
          ],
        ),
      ),
    );
  }

  Widget _buildBookingItem(dynamic booking) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8.0),
      child: Row(
        children: [
          Container(
            width: 8,
            height: 8,
            decoration: BoxDecoration(
              color: booking.isPaid ? Colors.green : Colors.red,
              shape: BoxShape.circle,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              'Бронирование #${booking.id.substring(0, 8)}',
              style: const TextStyle(fontWeight: FontWeight.bold),
            ),
          ),
          Text(
            '${booking.totalPrice} руб.',
            style: const TextStyle(fontWeight: FontWeight.bold),
          ),
          const SizedBox(width: 8),
          Text(
            _dateFormat.format(booking.createdAt),
            style: TextStyle(color: Colors.grey[600]),
          ),
        ],
      ),
    );
  }
}
