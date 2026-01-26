import 'package:flutter/material.dart';
import 'package:fl_chart/fl_chart.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../core/constants/api_constants.dart';
import '../providers/movie_statistics_provider.dart';

class MovieStatisticsScreen extends StatefulWidget {
  const MovieStatisticsScreen({Key? key}) : super(key: key);

  @override
  State<MovieStatisticsScreen> createState() => _MovieStatisticsScreenState();
}

class _MovieStatisticsScreenState extends State<MovieStatisticsScreen> {
  late MovieStatisticsProvider _statisticsProvider;
  final DateFormat _dateFormat = DateFormat('dd-MM-yyyy');

  @override
  void initState() {
    super.initState();
    _statisticsProvider =
        Provider.of<MovieStatisticsProvider>(context, listen: false);
    _loadData();
  }

  Future<void> _loadData() async {
    await _statisticsProvider.fetchMovieStatistics();
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
        title: const Text('Статистика фильмов'),
        backgroundColor: AppTheme.primaryColor,
        actions: [
          IconButton(
            icon: const Icon(Icons.date_range),
            onPressed: _selectDateRange,
          ),
          IconButton(icon: const Icon(Icons.refresh), onPressed: _loadData),
        ],
      ),
      body: Consumer<MovieStatisticsProvider>(
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

          return _buildRevenueTab(provider);
        },
      ),
    );
  }

  Widget _buildRevenueTab(MovieStatisticsProvider provider) {
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
            _buildRevenueChart(provider),
            const SizedBox(height: 24),
            _buildHighestRevenueMoviesList(provider),
          ],
        ),
      ),
    );
  }

  Widget _buildDateRangeCard(MovieStatisticsProvider provider) {
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

  Widget _buildRevenueChart(MovieStatisticsProvider provider) {
    final chartData = provider.revenueChartData;
    
    if (chartData.isEmpty) {
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

    // Find the maximum value for scaling
    final maxValue = chartData.fold<double>(
      0,
      (max, data) => data.value > max ? data.value : max,
    );

    // Reverse the list so that the highest revenue are on top
    final chartItems = chartData.reversed.toList();
    
    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Фильмы с наибольшей выручкой',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            SizedBox(
              height: 400,
              child: BarChart(
                BarChartData(
                  alignment: BarChartAlignment.center,
                  maxY: maxValue * 1.1,
                  barTouchData: BarTouchData(
                    enabled: true,
                    touchTooltipData: BarTouchTooltipData(
                      getTooltipItem: (group, groupIndex, rod, rodIndex) {
                        return BarTooltipItem(
                          '${chartItems[groupIndex].movie.title}\n${rod.toY.toStringAsFixed(2)} руб.',
                          const TextStyle(color: Colors.white),
                        );
                      },
                    ),
                  ),
                  titlesData: FlTitlesData(
                    show: true,
                    rightTitles: AxisTitles(
                      sideTitles: SideTitles(showTitles: false),
                    ),
                    topTitles: AxisTitles(
                      sideTitles: SideTitles(showTitles: false),
                    ),
                    bottomTitles: AxisTitles(
                      sideTitles: SideTitles(
                        showTitles: true,
                        getTitlesWidget: (value, meta) {
                          if (value.toInt() >= 0 && value.toInt() < chartItems.length) {
                            // Truncate movie title to prevent overlap
                            String title = chartItems[value.toInt()].movie.title;
                            if (title.length > 15) {
                              title = title.substring(0, 15) + '...';
                            }
                            return Padding(
                              padding: const EdgeInsets.only(top: 8.0),
                              child: RotatedBox(
                                quarterTurns: 3, // Rotate 270 degrees
                                child: Text(
                                  title,
                                  style: const TextStyle(
                                    fontSize: 10,
                                    fontWeight: FontWeight.bold,
                                  ),
                                  textAlign: TextAlign.center,
                                ),
                              ),
                            );
                          }
                          return const SizedBox();
                        },
                        reservedSize: 60, // Increased to accommodate vertical text
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
                  ),
                  borderData: FlBorderData(show: false),
                  gridData: FlGridData(
                    show: true,
                    horizontalInterval: maxValue / 5,
                    getDrawingHorizontalLine: (value) => FlLine(
                      color: Colors.grey.withOpacity(0.2),
                      strokeWidth: 1,
                    ),
                  ),
                  barGroups: List.generate(chartItems.length, (index) {
                    return BarChartGroupData(
                      x: index,
                      barRods: [
                        BarChartRodData(
                          toY: chartItems[index].value,
                          color: AppTheme.accentColor,
                          width: 20,
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
  
  Widget _buildHighestRevenueMoviesList(MovieStatisticsProvider provider) {
    final topMovies = provider.topMoviesByRevenue;
    
    if (topMovies.isEmpty) {
      return const Card(
        child: Padding(
          padding: EdgeInsets.all(16.0),
          child: Center(
            child: Text(
              'Нет данных о выручке фильмов',
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
              'Топ-10 фильмов по выручке',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            ...topMovies.take(10).map((movie) => _buildMovieItem(
              movie, 
              '${(provider.movieRevenue[movie.id] ?? 0).toStringAsFixed(2)} руб.',
              index: topMovies.indexOf(movie),
            )).toList(),
          ],
        ),
      ),
    );
  }

  Widget _buildMovieItem(dynamic movie, String value, {required int index}) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12.0),
      child: Row(
        children: [
          Container(
            width: 30,
            height: 30,
            decoration: BoxDecoration(
              color: AppTheme.accentColor,
              shape: BoxShape.circle,
            ),
            alignment: Alignment.center,
            child: Text(
              '${index + 1}',
              style: const TextStyle(
                color: Colors.white,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          const SizedBox(width: 12),
          // Movie poster
          ClipRRect(
            borderRadius: BorderRadius.circular(8),
            child: movie.poster.isNotEmpty
                ? Image.network(
                    '${ApiConstants.moviePoster}/${movie.poster}',
                    width: 40,
                    height: 60,
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stackTrace) {
                      return Container(
                        width: 40,
                        height: 60,
                        color: Colors.grey[300],
                        child: const Icon(Icons.movie, size: 20),
                      );
                    },
                  )
                : Container(
                    width: 40,
                    height: 60,
                    color: Colors.grey[300],
                    child: const Icon(Icons.movie, size: 20),
                  ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  movie.title,
                  style: const TextStyle(fontWeight: FontWeight.bold),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
                Text(
                  'Цена: ${movie.price} руб.',
                  style: TextStyle(color: Colors.grey[600], fontSize: 12),
                ),
              ],
            ),
          ),
          Text(
            value,
            style: const TextStyle(fontWeight: FontWeight.bold),
          ),
        ],
      ),
    );
  }
} 