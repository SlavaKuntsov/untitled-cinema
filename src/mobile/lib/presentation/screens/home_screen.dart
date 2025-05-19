import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../domain/entities/movie/movie.dart';
import '../providers/movie_provider.dart';
import '../widgets/home/movie_list_widget.dart';
import '../widgets/home/movie_pagination_widget.dart';
import 'movie_detail_screen.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({Key? key}) : super(key: key);

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final List<String> genres = [
    'Все',
    'Боевики',
    'Комедии',
    'Драмы',
    'Триллеры',
    'Ужасы',
    'Фантастика',
  ];

  String selectedGenre = 'Все';
  final ScrollController _scrollController = ScrollController();

  // Добавляем списки доступных размеров страниц
  final List<int> pageSizes = [1, 5, 10, 20, 50];
  int selectedPageSize = 10; // Значение по умолчанию

  // Имитация списка фильмов
  final List<Map<String, dynamic>> featuredMovies = [
    {
      'title': 'Дюна: Часть вторая',
      'genre': 'Фантастика',
      'rating': 4.8,
      'imageUrl': 'assets/images/dune.jpg',
      'showTimes': ['10:00', '13:30', '16:45', '20:00'],
    },
    {
      'title': 'Гладиатор 2',
      'genre': 'Боевики',
      'rating': 4.6,
      'imageUrl': 'assets/images/gladiator.jpg',
      'showTimes': ['11:15', '14:30', '17:45', '21:00'],
    },
    {
      'title': 'Джокер: Безумие на двоих',
      'genre': 'Драмы',
      'rating': 4.5,
      'imageUrl': 'assets/images/joker.jpg',
      'showTimes': ['12:00', '15:15', '18:30', '21:45'],
    },
    {
      'title': 'Мстители: Новая эра',
      'genre': 'Боевики',
      'rating': 4.7,
      'imageUrl': 'assets/images/avengers.jpg',
      'showTimes': ['10:30', '13:45', '17:00', '20:15'],
    },
  ];

  @override
  void initState() {
    super.initState();

    // Загружаем первую страницу при инициализации
    Future.microtask(() {
      List<String>? filters;
      List<String>? filterValues;

      if (selectedGenre != 'Все') {
        filters = ['genre'];
        filterValues = [selectedGenre];
      }

      Provider.of<MovieProvider>(context, listen: false).fetchMovies(
        page: 1,
        limit: selectedPageSize,
        filters: filters,
        filterValues: filterValues,
      );
    });

    _scrollController.addListener(_onScroll);
  }

  @override
  void dispose() {
    _scrollController.removeListener(_onScroll);
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_isBottom) {
      List<String>? filters;
      List<String>? filterValues;

      if (selectedGenre != 'Все') {
        filters = ['genre'];
        filterValues = [selectedGenre];
      }

      Provider.of<MovieProvider>(context, listen: false).nextPage();
    }
  }

  bool get _isBottom {
    if (!_scrollController.hasClients) return false;
    final maxScroll = _scrollController.position.maxScrollExtent;
    final currentScroll = _scrollController.offset;
    return currentScroll >= (maxScroll * 0.9);
  }

  void _updateGenreFilter(String genre) {
    setState(() {
      selectedGenre = genre;
    });

    List<String>? filters;
    List<String>? filterValues;

    if (genre != 'Все') {
      filters = ['genre'];
      filterValues = [genre];
    }

    Provider.of<MovieProvider>(context, listen: false).fetchMovies(
      page: 1,
      limit: selectedPageSize,
      filters: filters,
      filterValues: filterValues,
    );
  }

  void _updatePageSize(int? newSize) {
    if (newSize != null && newSize != selectedPageSize) {
      setState(() {
        selectedPageSize = newSize;
      });

      List<String>? filters;
      List<String>? filterValues;

      if (selectedGenre != 'Все') {
        filters = ['genre'];
        filterValues = [selectedGenre];
      }

      Provider.of<MovieProvider>(context, listen: false).fetchMovies(
        limit: newSize,
        filters: filters,
        filterValues: filterValues,
      );
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
    final movieProvider = Provider.of<MovieProvider>(context);
    final moviesState = movieProvider.moviesState;

    return SingleChildScrollView(
      controller: _scrollController,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Баннер с новинками
          Container(
            height: 200,
            width: double.infinity,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                colors: [Colors.blue.shade800, Colors.blue.shade600],
              ),
            ),
            child: Stack(
              children: [
                Positioned.fill(
                  child: Opacity(
                    opacity: 0.3,
                    child: Icon(Icons.icecream_outlined),
                  ),
                ),
                Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Text(
                        'Новинки этой недели',
                        style: TextStyle(
                          color: Colors.white,
                          fontSize: 24,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 16),
                      ElevatedButton.icon(
                        onPressed: () {},
                        icon: const Icon(Icons.local_activity),
                        label: const Text('Купить билет'),
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.amber,
                          foregroundColor: Colors.black,
                          padding: const EdgeInsets.symmetric(
                            horizontal: 24,
                            vertical: 12,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),

          const SizedBox(height: 16),

          // Фильтр по жанрам
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Жанры',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 8),
                SizedBox(
                  height: 40,
                  child: ListView.builder(
                    scrollDirection: Axis.horizontal,
                    itemCount: genres.length,
                    itemBuilder: (context, index) {
                      final genre = genres[index];
                      final isSelected = genre == selectedGenre;

                      return Padding(
                        padding: const EdgeInsets.only(right: 8),
                        child: FilterChip(
                          label: Text(genre),
                          selected: isSelected,
                          onSelected: (selected) {
                            _updateGenreFilter(genre);
                          },
                          backgroundColor: Colors.grey.shade200,
                          selectedColor: Colors.blue.shade100,
                          checkmarkColor: Colors.blue,
                        ),
                      );
                    },
                  ),
                ),
              ],
            ),
          ),

          const SizedBox(height: 16),

          // Популярные сейчас (горизонтальный список)
          Padding(
            padding: const EdgeInsets.only(left: 16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Популярно сейчас',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 8),
                SizedBox(
                  height: 230,
                  child: ListView.builder(
                    scrollDirection: Axis.horizontal,
                    itemCount: featuredMovies.length,
                    itemBuilder: (context, index) {
                      final movie = featuredMovies[index];

                      return GestureDetector(
                        onTap: () {},
                        child: Container(
                          width: 140,
                          margin: const EdgeInsets.only(right: 12),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Container(
                                height: 160,
                                decoration: BoxDecoration(
                                  borderRadius: BorderRadius.circular(8),
                                  color: Colors.grey.shade300,
                                ),
                              ),
                              const SizedBox(height: 8),
                              Text(
                                movie['title'],
                                maxLines: 2,
                                overflow: TextOverflow.ellipsis,
                                style: const TextStyle(
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              Row(
                                children: [
                                  Icon(
                                    Icons.star,
                                    size: 16,
                                    color: Colors.amber.shade700,
                                  ),
                                  const SizedBox(width: 4),
                                  Text(
                                    '${movie['rating']}',
                                    style: TextStyle(
                                      color: Colors.grey.shade700,
                                      fontSize: 12,
                                    ),
                                  ),
                                ],
                              ),
                            ],
                          ),
                        ),
                      );
                    },
                  ),
                ),
              ],
            ),
          ),

          const SizedBox(height: 16),

          // Список всех фильмов с пагинацией
          Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Row(
                      children: [
                        Text(
                          selectedGenre == 'Все' ? 'Все фильмы' : selectedGenre,
                          style: const TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ],
                    ),
                    if (moviesState.status == MovieStatus.loaded)
                      Text(
                        'Всего: ${moviesState.total}',
                        style: TextStyle(color: Colors.grey.shade600),
                      ),
                  ],
                ),
                const SizedBox(height: 16),

                // Управление состояниями
                if (moviesState.status == MovieStatus.initial ||
                    (moviesState.status == MovieStatus.loading &&
                        moviesState.movies.isEmpty))
                  const Center(
                    child: Padding(
                      padding: EdgeInsets.all(32.0),
                      child: CircularProgressIndicator(),
                    ),
                  )
                else if (moviesState.status == MovieStatus.error &&
                    moviesState.movies.isEmpty)
                  Center(
                    child: Padding(
                      padding: const EdgeInsets.all(32.0),
                      child: Column(
                        children: [
                          Text(
                            moviesState.errorMessage ??
                                'Произошла ошибка при загрузке фильмов',
                            textAlign: TextAlign.center,
                            style: TextStyle(color: Colors.red.shade800),
                          ),
                          const SizedBox(height: 16),
                          ElevatedButton(
                            onPressed: () => movieProvider.refreshMovies(),
                            child: const Text('Попробовать снова'),
                          ),
                        ],
                      ),
                    ),
                  )
                else if (moviesState.movies.isEmpty)
                  const Center(
                    child: Padding(
                      padding: EdgeInsets.all(32.0),
                      child: Text(
                        'Фильмы не найдены',
                        style: TextStyle(fontSize: 16),
                      ),
                    ),
                  )
                else
                  Column(
                    // mainAxisSize: MainAxisSize.max,
                    children: [
                      // Список фильмов
                      MovieListWidget(
                        moviesState: moviesState,
                        onMovieTap: (movieId) {
                          navigateToMovieDetails(context, movieId);
                        },
                        getShowTimes: _getShowTimes,
                        getAgeLimitColor: _getAgeLimitColor,
                      ),

                      MoviePaginationWidget(
                        moviesState: moviesState,
                        selectedPageSize: selectedPageSize,
                        pageSizes: pageSizes,
                        onPageSizeChanged: _updatePageSize,
                        onNextPage: () => movieProvider.nextPage(),
                        onPrevPage: () => movieProvider.prevPage(),
                      ),
                    ],
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  List<String> _getShowTimes(Movie movie) {
    final int numberOfTimes = (movie.id.hashCode % 4) + 1;
    final List<String> times = [];

    final baseHour = 10 + (movie.id.hashCode % 6);
    for (int i = 0; i < numberOfTimes; i++) {
      final hour = (baseHour + i * 3) % 24;
      final minute = (movie.id.hashCode * (i + 1)) % 60;
      times.add(
        '${hour.toString().padLeft(2, '0')}:${minute.toString().padLeft(2, '0')}',
      );
    }

    return times;
  }

  Color _getAgeLimitColor(int ageLimit) {
    if (ageLimit >= 18) return Colors.red;
    if (ageLimit >= 16) return Colors.orange;
    if (ageLimit >= 12) return Colors.amber.shade700;
    if (ageLimit >= 6) return Colors.green;
    return Colors.blue;
  }
}
