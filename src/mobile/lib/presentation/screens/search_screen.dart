import 'package:flutter/material.dart';

class SearchScreen extends StatefulWidget {
  const SearchScreen({Key? key}) : super(key: key);

  @override
  State<SearchScreen> createState() => _SearchScreenState();
}

class _SearchScreenState extends State<SearchScreen> {
  final TextEditingController _searchController = TextEditingController();
  bool _isAdvancedSearch = false;
  String _selectedGenre = 'Любой';
  double _minRating = 0;

  final List<String> genres = [
    'Любой',
    'Боевики',
    'Комедии',
    'Драмы',
    'Триллеры',
    'Ужасы',
    'Фантастика',
  ];

  // Имитация списка фильмов (в реальном приложении данные будут приходить с API)
  final List<Map<String, dynamic>> allMovies = [
    {
      'title': 'Дюна: Часть вторая',
      'genre': 'Фантастика',
      'rating': 4.8,
      'imageUrl': 'assets/images/dune.jpg',
      'year': 2025,
    },
    {
      'title': 'Гладиатор 2',
      'genre': 'Боевики',
      'rating': 4.6,
      'imageUrl': 'assets/images/gladiator.jpg',
      'year': 2025,
    },
    {
      'title': 'Джокер: Безумие на двоих',
      'genre': 'Драмы',
      'rating': 4.5,
      'imageUrl': 'assets/images/joker.jpg',
      'year': 2024,
    },
    {
      'title': 'Мстители: Новая эра',
      'genre': 'Боевики',
      'rating': 4.7,
      'imageUrl': 'assets/images/avengers.jpg',
      'year': 2025,
    },
    {
      'title': 'Тихое место 3',
      'genre': 'Ужасы',
      'rating': 4.3,
      'imageUrl': 'assets/images/quiet_place.jpg',
      'year': 2024,
    },
    {
      'title': 'Дэдпул и Росомаха',
      'genre': 'Комедии',
      'rating': 4.9,
      'imageUrl': 'assets/images/deadpool.jpg',
      'year': 2024,
    },
    {
      'title': 'Бегущий по лезвию 2099',
      'genre': 'Фантастика',
      'rating': 4.4,
      'imageUrl': 'assets/images/blade_runner.jpg',
      'year': 2025,
    },
    {
      'title': 'Миссия невыполнима 8',
      'genre': 'Триллеры',
      'rating': 4.6,
      'imageUrl': 'assets/images/mission_impossible.jpg',
      'year': 2025,
    },
    {
      'title': 'Барби 2',
      'genre': 'Комедии',
      'rating': 4.2,
      'imageUrl': 'assets/images/barbie.jpg',
      'year': 2026,
    },
    {
      'title': 'Аватар 3',
      'genre': 'Фантастика',
      'rating': 4.7,
      'imageUrl': 'assets/images/avatar.jpg',
      'year': 2025,
    },
    {
      'title': 'Властелин колец: Война Рохана',
      'genre': 'Фантастика',
      'rating': 4.5,
      'imageUrl': 'assets/images/lord_of_the_rings.jpg',
      'year': 2024,
    },
  ];

  List<Map<String, dynamic>> _filteredMovies = [];
  bool _isSearching = false;

  @override
  void initState() {
    super.initState();
    // Инициализируем фильтрованный список всеми фильмами
    _filteredMovies = List.from(allMovies);
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _performSearch(String query) {
    setState(() {
      _isSearching = query.isNotEmpty;

      if (query.isEmpty && _selectedGenre == 'Любой' && _minRating == 0) {
        _filteredMovies = List.from(allMovies);
      } else {
        _filteredMovies =
            allMovies.where((movie) {
              // Фильтр по названию
              bool matchesQuery =
                  query.isEmpty ||
                  movie['title'].toLowerCase().contains(query.toLowerCase());

              // Фильтр по жанру
              bool matchesGenre =
                  _selectedGenre == 'Любой' || movie['genre'] == _selectedGenre;

              // Фильтр по рейтингу
              bool matchesRating = movie['rating'] >= _minRating;

              return matchesQuery && matchesGenre && matchesRating;
            }).toList();
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        children: [
          // Поле поиска
          TextField(
            controller: _searchController,
            decoration: InputDecoration(
              hintText: 'Поиск фильмов...',
              prefixIcon: const Icon(Icons.search),
              suffixIcon:
                  _searchController.text.isNotEmpty
                      ? IconButton(
                        icon: const Icon(Icons.clear),
                        onPressed: () {
                          _searchController.clear();
                          _performSearch('');
                        },
                      )
                      : null,
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(10),
              ),
            ),
            onChanged: _performSearch,
          ),

          const SizedBox(height: 16),

          // Кнопка расширенного поиска
          Row(
            children: [
              OutlinedButton.icon(
                onPressed: () {
                  setState(() {
                    _isAdvancedSearch = !_isAdvancedSearch;
                  });
                },
                icon: Icon(
                  _isAdvancedSearch
                      ? Icons.keyboard_arrow_up
                      : Icons.keyboard_arrow_down,
                ),
                label: Text(
                  _isAdvancedSearch ? 'Скрыть фильтры' : 'Показать фильтры',
                ),
              ),
              const Spacer(),
              if (_searchController.text.isNotEmpty ||
                  _selectedGenre != 'Любой' ||
                  _minRating > 0)
                TextButton(
                  onPressed: () {
                    setState(() {
                      _searchController.clear();
                      _selectedGenre = 'Любой';
                      _minRating = 0;
                      _performSearch('');
                    });
                  },
                  child: const Text('Сбросить'),
                ),
            ],
          ),

          // Панель расширенного поиска
          if (_isAdvancedSearch)
            Container(
              margin: const EdgeInsets.only(top: 16),
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: Colors.grey.shade100,
                borderRadius: BorderRadius.circular(10),
                border: Border.all(color: Colors.grey.shade300),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Выбор жанра
                  const Text(
                    'Жанр:',
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 8),
                  SizedBox(
                    height: 40,
                    child: ListView.builder(
                      scrollDirection: Axis.horizontal,
                      itemCount: genres.length,
                      itemBuilder: (context, index) {
                        final genre = genres[index];
                        final isSelected = genre == _selectedGenre;

                        return Padding(
                          padding: const EdgeInsets.only(right: 8),
                          child: FilterChip(
                            label: Text(genre),
                            selected: isSelected,
                            onSelected: (selected) {
                              setState(() {
                                _selectedGenre = genre;
                                _performSearch(_searchController.text);
                              });
                            },
                            backgroundColor: Colors.white,
                            selectedColor: Colors.blue.shade100,
                          ),
                        );
                      },
                    ),
                  ),

                  const SizedBox(height: 16),

                  // Минимальный рейтинг
                  Text(
                    'Минимальный рейтинг: ${_minRating.toStringAsFixed(1)}',
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 8),
                  Slider(
                    value: _minRating,
                    min: 0,
                    max: 5,
                    divisions: 10,
                    label: _minRating.toStringAsFixed(1),
                    onChanged: (value) {
                      setState(() {
                        _minRating = value;
                      });
                    },
                    onChangeEnd: (value) {
                      _performSearch(_searchController.text);
                    },
                  ),
                ],
              ),
            ),

          const SizedBox(height: 16),

          // Результаты поиска
          Expanded(
            child:
                _filteredMovies.isEmpty
                    ? Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(
                            Icons.movie_edit,
                            size: 64,
                            color: Colors.grey.shade400,
                          ),
                          const SizedBox(height: 16),
                          Text(
                            'Фильмы не найдены',
                            style: TextStyle(
                              fontSize: 18,
                              color: Colors.grey.shade600,
                            ),
                          ),
                          const SizedBox(height: 8),
                          Text(
                            'Попробуйте изменить параметры поиска',
                            style: TextStyle(color: Colors.grey.shade500),
                          ),
                        ],
                      ),
                    )
                    : ListView.builder(
                      itemCount: _filteredMovies.length,
                      itemBuilder: (context, index) {
                        final movie = _filteredMovies[index];

                        return Card(
                          margin: const EdgeInsets.only(bottom: 12),
                          child: ListTile(
                            contentPadding: const EdgeInsets.all(12),
                            leading: ClipRRect(
                              borderRadius: BorderRadius.circular(8),
                              child: Image.network(
                                'https://via.placeholder.com/300x450?text=${Uri.encodeComponent(movie['title'])}',
                                width: 60,
                                height: 90,
                                fit: BoxFit.cover,
                              ),
                            ),
                            title: Text(
                              movie['title'],
                              style: const TextStyle(
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            subtitle: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                const SizedBox(height: 4),
                                Text('${movie['genre']} • ${movie['year']}'),
                                const SizedBox(height: 4),
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
                                      style: const TextStyle(
                                        fontWeight: FontWeight.bold,
                                      ),
                                    ),
                                  ],
                                ),
                              ],
                            ),
                            trailing: IconButton(
                              icon: const Icon(
                                Icons.arrow_forward_ios,
                                size: 16,
                              ),
                              onPressed: () {
                                // Переход на страницу с деталями фильма
                              },
                            ),
                            onTap: () {
                              // Переход на страницу с деталями фильма
                            },
                          ),
                        );
                      },
                    ),
          ),
        ],
      ),
    );
  }
}
