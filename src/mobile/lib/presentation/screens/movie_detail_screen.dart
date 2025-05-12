import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:untitledCinema/presentation/widgets/home/buy_tickets_modal_widget.dart';

import '../../../core/constants/api_constants.dart';
import '../../../domain/entities/movie/movie.dart';
import '../providers/movie_provider.dart';
import '../providers/session_provider.dart';

class MovieDetailsScreen extends StatefulWidget {
  final String movieId;

  const MovieDetailsScreen({super.key, required this.movieId});

  @override
  State<MovieDetailsScreen> createState() => _MovieDetailsScreenState();
}

class _MovieDetailsScreenState extends State<MovieDetailsScreen> {
  bool _isLoading = true;
  Movie? _movie;
  List<String> _frames = [];
  String? _errorMessage;

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
      final provider = Provider.of<MovieProvider>(context, listen: false);
      // Получаем фильм по ID
      _movie = await provider.fetchMovieById(id: widget.movieId);
      _frames = await provider.fetchMoviesFrames(id: widget.movieId);
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

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(_movie?.title ?? 'Загрузка фильма...'),
        leading: IconButton(
          icon: const Icon(Icons.home),
          onPressed: () => Navigator.of(context).pop(),
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
          _buildMovieInfoSection(),
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
                Text(
                  _movie!.title,
                  style: TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.w500,
                    fontSize: 22,
                  ),
                ),

                const SizedBox(height: 8),

                // Дата релиза и возрастное ограничение в одну строку
                Row(
                  children: [
                    if (formattedReleaseDate.isNotEmpty)
                      Expanded(
                        child: Text(
                          'В кино с $formattedReleaseDate',
                          style: const TextStyle(
                            fontSize: 14,
                            color: Colors.white70,
                          ),
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

                const SizedBox(height: 16),

                // Жанры
                Wrap(
                  spacing: 8,
                  children:
                      _movie!.genres
                          .map(
                            (genre) => Chip(
                              label: Text(genre),
                              backgroundColor: Theme.of(
                                context,
                              ).primaryColor.withOpacity(0.6),
                            ),
                          )
                          .toList(),
                ),

                const SizedBox(height: 16),

                // Замените существующий блок кнопки "Купить билеты" в методе _buildHeaderWithGradient() класса MovieDetailsPage

                // Кнопка "Купить билеты"
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    onPressed: () {
                      // Получаем доступ к провайдеру сессий перед открытием модального окна
                      final sessionProvider = Provider.of<SessionProvider>(
                        context,
                        listen: false,
                      );

                      showModalBottomSheet(
                        context: context,
                        isScrollControlled: true,
                        backgroundColor: Colors.transparent,
                        builder:
                            (context) =>
                                ChangeNotifierProvider<SessionProvider>.value(
                                  // Передаем существующий экземпляр провайдера
                                  value: sessionProvider,
                                  child: DraggableScrollableSheet(
                                    initialChildSize: 0.6,
                                    minChildSize: 0.5,
                                    maxChildSize: 0.9,
                                    builder:
                                        (context, scrollController) =>
                                            BuyTicketsModalWidget(
                                              movie: _movie!,
                                            ),
                                  ),
                                ),
                      );
                    },
                    style: ElevatedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 12),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                    ),
                    child: const Text(
                      'Купить билеты',
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                      ),
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

  Widget _buildMovieInfoSection() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Базовая информация о фильме
          _buildInfoRow(
            Icons.access_time,
            'Длительность: ${_movie!.durationMinutes} минут',
          ),
          _buildInfoRow(
            Icons.monetization_on,
            'Цена: ${_movie!.price.toStringAsFixed(0)} Br',
          ),
          _buildInfoRow(Icons.person, 'Режиссер: ${_movie!.producer}'),

          if (_movie!.inRoles != null && _movie!.inRoles!.isNotEmpty) ...[
            const SizedBox(height: 16),
            const Text(
              'В ролях',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            Text(_movie!.inRoles),
          ],

          // Можно добавить другие поля из модели фильма здесь
          const SizedBox(height: 24),

          // Описание
          const Text(
            'Описание',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 8),
          Text(
            _movie!.description.isNotEmpty
                ? _movie!.description
                : 'Описание отсутствует',
            style: const TextStyle(fontSize: 16),
          ),

          const SizedBox(height: 16),

          // Галерея кадров из фильма
          _buildMovieFramesGallery(),

          const SizedBox(height: 24), // Отступ внизу страницы
        ],
      ),
    );
  }

  // Добавим метод для построения галереи кадров из фильма
  Widget _buildMovieFramesGallery() {
    if (_frames.isEmpty) {
      return const SizedBox.shrink(); // Если кадров нет, не отображаем секцию
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Padding(
          padding: EdgeInsets.symmetric(vertical: 8.0),
          child: Text(
            'Кадры из фильма',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
        ),
        SizedBox(
          height: 150, // Фиксированная высота для галереи
          child: ListView.builder(
            scrollDirection: Axis.horizontal,
            itemCount: _frames.length,
            itemBuilder: (context, index) {
              final fileName = _frames[index];
              return Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: 4.0,
                  vertical: 8.0,
                ),
                child: GestureDetector(
                  onTap: () => _showFullScreenImage(index),
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(8),
                    child: Image.network(
                      '${ApiConstants.movies}/frames/$fileName',
                      width: 200,
                      height: 134,
                      fit: BoxFit.cover,
                      errorBuilder: (context, error, stackTrace) {
                        return Container(
                          width: 200,
                          height: 134,
                          color: Colors.grey.shade300,
                          child: const Center(
                            child: Icon(Icons.broken_image, color: Colors.grey),
                          ),
                        );
                      },
                      loadingBuilder: (context, child, loadingProgress) {
                        if (loadingProgress == null) return child;
                        return Container(
                          width: 200,
                          height: 134,
                          color: Colors.grey.shade100,
                          child: const Center(
                            child: CircularProgressIndicator(),
                          ),
                        );
                      },
                    ),
                  ),
                ),
              );
            },
          ),
        ),
      ],
    );
  }

  // Метод для отображения кадра на полный экран
  void _showFullScreenImage(int initialIndex) {
    Navigator.of(context).push(
      MaterialPageRoute(
        builder:
            (context) => Scaffold(
              backgroundColor: Colors.black,
              appBar: AppBar(
                backgroundColor: Colors.black,
                elevation: 0,
                iconTheme: const IconThemeData(color: Colors.white),
              ),
              body: PageView.builder(
                controller: PageController(initialPage: initialIndex),
                itemCount: _frames.length,
                itemBuilder: (context, index) {
                  final fileName = _frames[index];
                  return Center(
                    child: InteractiveViewer(
                      minScale: 0.5,
                      maxScale: 3.0,
                      child: Image.network(
                        '${ApiConstants.movies}/frames/$fileName',
                        fit: BoxFit.contain,
                        errorBuilder: (context, error, stackTrace) {
                          return Container(
                            color: Colors.black,
                            child: const Center(
                              child: Icon(
                                Icons.broken_image,
                                color: Colors.white,
                                size: 50,
                              ),
                            ),
                          );
                        },
                        loadingBuilder: (context, child, loadingProgress) {
                          if (loadingProgress == null) return child;
                          return const Center(
                            child: CircularProgressIndicator(
                              color: Colors.white,
                            ),
                          );
                        },
                      ),
                    ),
                  );
                },
              ),
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
}
