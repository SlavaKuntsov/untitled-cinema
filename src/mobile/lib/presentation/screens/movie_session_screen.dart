import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:untitledCinema/config/theme.dart';
import 'package:untitledCinema/domain/entities/session/seat_type.dart';
import 'package:untitledCinema/domain/entities/session/session.dart';
import 'package:untitledCinema/presentation/providers/session_provider.dart';
import 'package:untitledCinema/presentation/screens/movie_detail_screen.dart';

import '../../../domain/entities/movie/movie.dart';
import '../../core/constants/api_constants.dart';
import '../../domain/entities/session/hall.dart';
import '../providers/movie_provider.dart';
import '../widgets/home/seat_map_widget.dart';
import 'navigation_screen.dart';

class MovieSessionScreen extends StatefulWidget {
  final Session session;

  const MovieSessionScreen({super.key, required this.session});

  @override
  State<MovieSessionScreen> createState() => _MovieSessionScreenState();
}

class _MovieSessionScreenState extends State<MovieSessionScreen> {
  bool _isLoading = true;
  Movie? _movie;
  Hall? _hall;
  List<SeatType>? _seatTypes;
  String? _errorMessage;

  // Добавьте эти переменные в начало класса _MovieSessionScreenState
  List<SeatPosition> _selectedSeats = [];
  double _totalPrice = 0;

  // Добавьте этот метод для расчета общей стоимости
  void _updateTotalPrice() {
    double total = 0;
    for (var seat in _selectedSeats) {
      // Базовая цена фильма * модификатор сеанса * модификатор типа места
      final seatType = _seatTypes!.firstWhere(
        (type) => type.id == seat.type.toString(),
        orElse: () => SeatType(id: '1', name: 'Стандарт', priceModifier: 1.0),
      );

      total +=
          _movie!.price * widget.session.priceModifier * seatType.priceModifier;
    }
    _totalPrice = total;
  }

  @override
  void initState() {
    super.initState();
    _loadMovieDetails();
  }

  Future<void> _loadMovieDetails() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      final movieProvider = Provider.of<MovieProvider>(context, listen: false);
      final sessionProvider = Provider.of<SessionProvider>(
        context,
        listen: false,
      );
      _movie = await movieProvider.fetchMovieById(id: widget.session.movieId);
      _hall = await sessionProvider.fetchHallById(id: widget.session.hall.id);
      _seatTypes = await sessionProvider.fetchSeatTypesByHallId(
        hallId: widget.session.hall.id,
      );
      setState(() {
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
        _errorMessage = 'Ошибка при загрузке данных: ${e.toString()}';
      });
    }
  }

  void navigateToMovieDetails(BuildContext context, String movieId) {
    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (context) => MovieDetailsScreen(movieId: movieId),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(_movie?.title ?? 'Загрузка фильма...'),
        leading: IconButton(
          icon: const Icon(Icons.home),
          onPressed:
              () => Navigator.of(context).pushAndRemoveUntil(
                MaterialPageRoute(builder: (context) => NavigationScreen()),
                (route) => false, // Удаляет все экраны до NavigationScreen
              ),
        ),
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_errorMessage != null) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(_errorMessage!),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _loadMovieDetails,
              child: const Text('Попробовать снова'),
            ),
          ],
        ),
      );
    }

    if (_movie == null) {
      return const Center(child: Text('Информация о фильме не найдена'));
    }

    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Верхний блок с градиентом
          _buildHeaderWithGradient(),
          // Нижний блок с информацией на обычном фоне
          // _buildMovieInfoSection(),
        ],
      ),
    );
  }

  Widget _buildInfoRow(IconData icon, String text) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Row(
        children: [
          Icon(icon, size: 20, color: Colors.grey),
          const SizedBox(width: 8),
          Text(text, style: const TextStyle(fontSize: 16)),
        ],
      ),
    );
  }

  Widget _buildHeaderWithGradient() {
    // Форматируем дату релиза
    String formattedReleaseDate = "";
    if (_movie!.releaseDate != null) {
      // Проверяем тип данных releaseDate и обрабатываем соответственно
      DateTime dateTime;
      if (_movie!.releaseDate is DateTime) {
        dateTime = _movie!.releaseDate! as DateTime;
      } else if (_movie!.releaseDate is String) {
        dateTime = DateTime.parse(_movie!.releaseDate! as String);
      } else {
        // Если тип неизвестен, просто не показываем дату
        dateTime = DateTime.now();
        formattedReleaseDate = "";
      }

      if (dateTime != null) {
        formattedReleaseDate =
            "${dateTime.day}.${dateTime.month}.${dateTime.year}";
      }
    }

    return Container(
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topCenter,
          end: Alignment.bottomCenter,
          colors: [
            Theme.of(context).scaffoldBackgroundColor,
            Color(0x18000000),
          ],
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Постер фильма
          Center(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: ClipRRect(
                borderRadius: BorderRadius.circular(12),
                child:
                    _movie!.poster.isNotEmpty
                        ? Image.network(
                          '${ApiConstants.moviePoster}/${_movie!.poster}',
                          height: 300,
                          fit: BoxFit.cover,
                          errorBuilder: (context, error, stackTrace) {
                            return Container(
                              height: 300,
                              width: 200,
                              color: Colors.grey.shade300,
                              child: const Icon(
                                Icons.movie,
                                size: 50,
                                color: Colors.white,
                              ),
                            );
                          },
                        )
                        : Container(
                          height: 300,
                          width: 200,
                          color: Colors.grey.shade300,
                          child: const Icon(
                            Icons.movie,
                            size: 50,
                            color: Colors.white,
                          ),
                        ),
              ),
            ),
          ),

          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Название фильма
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      _movie!.title,
                      style: TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.w500,
                        fontSize: 22,
                      ),
                    ),
                    if (_movie!.ageLimit > 0)
                      Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 8,
                          vertical: 4,
                        ),
                        decoration: BoxDecoration(
                          color: _getAgeLimitColor(_movie!.ageLimit),
                          borderRadius: BorderRadius.circular(4),
                        ),
                        child: Text(
                          '${_movie!.ageLimit}+',
                          style: const TextStyle(
                            color: Colors.white,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                  ],
                ),

                const SizedBox(height: 8),

                // Секция с информацией о сеансе (дата, время, зал)
                Container(
                  margin: const EdgeInsets.only(top: 16),
                  decoration: BoxDecoration(
                    color: Colors.black38,
                    borderRadius: BorderRadius.circular(8),
                    border: Border.all(color: AppTheme.primaryColor),
                  ),
                  child: Row(
                    children: [
                      // Дата
                      Expanded(
                        child: Container(
                          padding: const EdgeInsets.symmetric(vertical: 16),
                          decoration: BoxDecoration(
                            border: Border(
                              right: BorderSide(
                                color: Colors.grey.shade800,
                                width: 1,
                              ),
                            ),
                          ),
                          child: Column(
                            children: [
                              const Text(
                                'Дата',
                                style: TextStyle(
                                  color: Colors.grey,
                                  fontSize: 14,
                                ),
                              ),
                              const SizedBox(height: 8),
                              Text(
                                _formatDate(widget.session.startTime),
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontSize: 20,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),

                      // Время сеанса
                      Expanded(
                        child: Container(
                          padding: const EdgeInsets.symmetric(vertical: 16),
                          decoration: BoxDecoration(
                            border: Border(
                              right: BorderSide(
                                color: Colors.grey.shade800,
                                width: 1,
                              ),
                            ),
                          ),
                          child: Column(
                            children: [
                              const Text(
                                'Сеанс',
                                style: TextStyle(
                                  color: Colors.grey,
                                  fontSize: 14,
                                ),
                              ),
                              const SizedBox(height: 8),
                              Text(
                                _formatTime(widget.session.startTime),
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontSize: 20,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),

                      // Зал
                      Expanded(
                        flex: 2,
                        child: Container(
                          padding: const EdgeInsets.symmetric(vertical: 16),
                          child: Column(
                            children: [
                              const Text(
                                'Зал',
                                style: TextStyle(
                                  color: Colors.grey,
                                  fontSize: 14,
                                ),
                              ),
                              const SizedBox(height: 8),
                              Text(
                                widget.session.hall.name,
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontSize: 16,
                                  fontWeight: FontWeight.bold,
                                ),
                                textAlign: TextAlign.center,
                              ),
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
                ),

                const SizedBox(height: 24),

                // Кнопка "Подробнее"
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    onPressed: () {
                      navigateToMovieDetails(context, _movie!.id);
                    },
                    style: ElevatedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 12),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                    ),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Text(
                          'Подробнее',
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(width: 8),
                        Icon(
                          Icons.arrow_forward,
                          color: Colors.white,
                          size: 20,
                        ),
                      ],
                    ),
                  ),
                ),

                const SizedBox(height: 16),
              ],
            ),
          ),

          // Разделитель внизу секции
          const Divider(color: Colors.white30, thickness: 1, height: 1),
        ],
      ),
    );
  }

        ),
      ),
    );
  }

  Color _getAgeLimitColor(int ageLimit) {
    if (ageLimit >= 18) {
      return Colors.red;
    } else if (ageLimit >= 16) {
      return Colors.orange;
    } else if (ageLimit >= 12) {
      return Colors.yellow.shade800;
    } else if (ageLimit >= 6) {
      return Colors.green;
    } else {
      return Colors.blue;
    }
  }

  // Форматирует дату в формат ДД.ММ
  String _formatDate(DateTime dateTime) {
    return '${dateTime.day.toString().padLeft(2, '0')}.${dateTime.month.toString().padLeft(2, '0')}';
  }

  // Форматирует время в формат ЧЧ:ММ
  String _formatTime(DateTime dateTime) {
    return '${dateTime.hour.toString().padLeft(2, '0')}:${dateTime.minute.toString().padLeft(2, '0')}';
  }
}
