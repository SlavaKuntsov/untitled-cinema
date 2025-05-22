import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
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
  // Search controller
  final TextEditingController _searchController = TextEditingController();

  // Selected filters
  List<String> _selectedGenres = ['Все'];
  DateTime? _selectedDate = DateTime(
    DateTime.now().year,
    DateTime.now().month,
    DateTime.now().day,
  );

  // Sorting options
  String _sortBy = 'title';
  String _sortDirection = 'asc';

  // Sorting display options
  final Map<String, String> _sortOptions = {
    'title_asc': 'Название (А-Я)',
    'title_desc': 'Название (Я-А)',
    'price_asc': 'Цена (низкая-высокая)',
    'price_desc': 'Цена (высокая-низкая)',
  };

  String get _currentSortOption => '${_sortBy}_$_sortDirection';

  final ScrollController _scrollController = ScrollController();

  // Добавляем списки доступных размеров страниц
  final List<int> pageSizes = [1, 5, 10, 20, 50];
  int selectedPageSize = 10; // Значение по умолчанию

  @override
  void initState() {
    super.initState();

    // Загружаем первую страницу при инициализации
    Future.microtask(() {
      // Make sure we use the date in filtering
      _applyFilters();
    });

    _scrollController.addListener(_onScroll);
  }

  @override
  void dispose() {
    _scrollController.removeListener(_onScroll);
    _scrollController.dispose();
    _searchController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_isBottom) {
      Provider.of<MovieProvider>(context, listen: false).nextPage();
    }
  }

  bool get _isBottom {
    if (!_scrollController.hasClients) return false;
    final maxScroll = _scrollController.position.maxScrollExtent;
    final currentScroll = _scrollController.offset;
    return currentScroll >= (maxScroll * 0.9);
  }

  void _toggleGenreSelection(String genre) {
    setState(() {
      if (genre == 'Все') {
        // If "All" is selected, clear other selections
        _selectedGenres = ['Все'];
      } else {
        // Remove "All" from selection if it's there
        _selectedGenres.remove('Все');

        // Toggle the selected genre
        if (_selectedGenres.contains(genre)) {
          _selectedGenres.remove(genre);
          // If no genres are selected, default to "All"
          if (_selectedGenres.isEmpty) {
            _selectedGenres = ['Все'];
          }
        } else {
          _selectedGenres.add(genre);
        }
      }
    });
  }

  void _clearFilters() {
    setState(() {
      _selectedGenres = ['Все'];
      _selectedDate = DateTime(
        DateTime.now().year,
        DateTime.now().month,
        DateTime.now().day,
      );
      _searchController.clear();
    });
    _applyFilters();
    Navigator.pop(context);
  }

  void _applyFilters() {
    List<String>? filters;
    List<String>? filterValues;

    // Add genre filters
    if (!_selectedGenres.contains('Все')) {
      filters = [];
      filterValues = [];

      for (var genre in _selectedGenres) {
        filters.add('genre');
        filterValues.add(genre);
      }
    }

    // Add search term if present
    if (_searchController.text.isNotEmpty) {
      filters ??= [];
      filterValues ??= [];
      filters.add('title');
      filterValues.add(_searchController.text);
    }

    // Debug output
    print(
      'Applying filters with date: $_selectedDate, sort: $_sortBy $_sortDirection',
    );

    // Fetch movies with filters
    Provider.of<MovieProvider>(context, listen: false).fetchMovies(
      page: 1,
      limit: selectedPageSize,
      filters: filters,
      filterValues: filterValues,
      date: _selectedDate,
      sortBy: _sortBy,
      sortDirection: _sortDirection,
    );
  }

  void _updatePageSize(int? newSize) {
    if (newSize != null && newSize != selectedPageSize) {
      setState(() {
        selectedPageSize = newSize;
      });

      _applyFilters();
    }
  }

  void _showFilterBottomSheet() {
    showModalBottomSheet(
      context: context,
      isScrollControlled:
          true, // Make the bottom sheet take up the full screen height
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (context) {
        return StatefulBuilder(
          builder: (context, setState) {
            final movieProvider = Provider.of<MovieProvider>(context);
            final genres = movieProvider.genres;
            final isLoadingGenres = movieProvider.loadingGenres;

            return Padding(
              padding: EdgeInsets.only(
                left: 16,
                right: 16,
                top: 16,
                bottom: MediaQuery.of(context).viewInsets.bottom + 16,
              ),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Header
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text(
                        'Фильтры',
                        style: TextStyle(
                          fontSize: 20,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      IconButton(
                        icon: const Icon(Icons.close),
                        onPressed: () => Navigator.pop(context),
                      ),
                    ],
                  ),
                  const Divider(),

                  // Date filter
                  const Text(
                    'Дата сеанса:',
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      OutlinedButton.icon(
                        onPressed: () async {
                          final now = DateTime.now();
                          // Use as initial date either the currently selected date
                          // or today, ensuring it's not in the past
                          final initialDate =
                              _selectedDate != null &&
                                      _selectedDate!.isAfter(
                                        DateTime(now.year, now.month, now.day),
                                      )
                                  ? _selectedDate!
                                  : DateTime(now.year, now.month, now.day);

                          final DateTime? pickedDate = await showDatePicker(
                            context: context,
                            initialDate: initialDate,
                            firstDate: DateTime(
                              now.year,
                              now.month,
                              now.day,
                            ), // Today
                            lastDate: DateTime.now().add(
                              const Duration(days: 365),
                            ),
                          );

                          if (pickedDate != null) {
                            setState(() {
                              _selectedDate = pickedDate;
                            });
                          }
                        },
                        icon: const Icon(
                          Icons.calendar_today,
                          color: Colors.white,
                        ),
                        label: Text(
                          _selectedDate != null
                              ? DateFormat('dd-MM-yyyy').format(_selectedDate!)
                              : 'Выбрать дату',
                          style: const TextStyle(color: Colors.white),
                        ),
                        style: OutlinedButton.styleFrom(
                          side: BorderSide(color: AppTheme.accentColor),
                          foregroundColor: Colors.white,
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(8),
                          ),
                        ),
                      ),
                      if (_selectedDate != null)
                        IconButton(
                          icon: const Icon(Icons.clear),
                          onPressed: () {
                            setState(() {
                              _selectedDate = DateTime(
                                DateTime.now().year,
                                DateTime.now().month,
                                DateTime.now().day,
                              );
                            });
                          },
                        ),
                    ],
                  ),
                  const SizedBox(height: 16),

                  // Sorting section
                  const Text(
                    'Сортировка:',
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 8),
                  Wrap(
                    spacing: 8.0,
                    runSpacing: 8.0,
                    children:
                        _sortOptions.entries.map((entry) {
                          final key = entry.key;
                          final value = entry.value;
                          final isSelected = _currentSortOption == key;

                          return FilterChip(
                            label: Text(value),
                            selected: isSelected,
                            onSelected: (_) {
                              setState(() {
                                final parts = key.split('_');
                                _sortBy = parts[0];
                                _sortDirection = parts[1];
                              });
                            },
                            backgroundColor: AppTheme.cardColor,
                            selectedColor: AppTheme.accentColor,
                            checkmarkColor: Colors.black,
                            showCheckmark: false,
                            side: BorderSide(color: AppTheme.accentColor),
                            labelStyle: TextStyle(
                              color: Colors.white,
                              fontWeight:
                                  isSelected
                                      ? FontWeight.bold
                                      : FontWeight.normal,
                            ),
                          );
                        }).toList(),
                  ),
                  const SizedBox(height: 16),

                  // Genre filter
                  const Text(
                    'Жанры:',
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 8),

                  if (isLoadingGenres)
                    const Center(
                      child: Padding(
                        padding: EdgeInsets.all(16.0),
                        child: CircularProgressIndicator(),
                      ),
                    )
                  else
                    Wrap(
                      spacing: 8.0,
                      runSpacing: 8.0,
                      children:
                          genres.map((genre) {
                            final isSelected = _selectedGenres.contains(genre);
                            return FilterChip(
                              label: Text(genre),
                              selected: isSelected,
                              onSelected: (_) {
                                setState(() {
                                  if (genre == 'Все') {
                                    _selectedGenres = ['Все'];
                                  } else {
                                    if (isSelected) {
                                      _selectedGenres.remove(genre);
                                      if (_selectedGenres.isEmpty) {
                                        _selectedGenres = ['Все'];
                                      }
                                    } else {
                                      _selectedGenres.remove('Все');
                                      _selectedGenres.add(genre);
                                    }
                                  }
                                });
                              },
                              backgroundColor: AppTheme.cardColor,
                              selectedColor: AppTheme.accentColor,
                              checkmarkColor: Colors.black,
                              showCheckmark: false,
                              side: BorderSide(color: AppTheme.accentColor),
                              labelStyle: TextStyle(
                                color: Colors.white,
                                fontWeight:
                                    isSelected
                                        ? FontWeight.bold
                                        : FontWeight.normal,
                              ),
                            );
                          }).toList(),
                    ),
                  const SizedBox(height: 24),

                  // Action buttons
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                    children: [
                      Expanded(
                        child: ElevatedButton(
                          onPressed: () {
                            setState(() {
                              _selectedGenres = ['Все'];
                              _selectedDate = null;
                            });
                          },
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppTheme.cardColor,
                            foregroundColor: Colors.white,
                            padding: const EdgeInsets.symmetric(vertical: 16),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(8),
                              side: BorderSide(color: AppTheme.accentColor),
                            ),
                          ),
                          child: const Text('Сбросить'),
                        ),
                      ),
                      const SizedBox(width: 16),
                      Expanded(
                        child: ElevatedButton(
                          onPressed: () {
                            // Use parent setState to update the HomeScreen state
                            this.setState(() {
                              // The selected values are already updated via the StatefulBuilder's setState
                              _selectedGenres = [..._selectedGenres];
                              // Make sure we preserve the selected date
                              if (_selectedDate == null) {
                                _selectedDate = null; // Clear if set to null
                              }
                            });
                            Navigator.pop(context);
                            _applyFilters();
                          },
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppTheme.accentColor,
                            foregroundColor: Colors.white,
                            padding: const EdgeInsets.symmetric(vertical: 16),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(8),
                            ),
                          ),
                          child: const Text('Применить'),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            );
          },
        );
      },
    );
  }

  void navigateToMovieDetails(BuildContext context, String movieId) {
    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (context) => MovieDetailsScreen(movieId: movieId),
      ),
    );
  }

  // Helper method to check if date is today
  bool _isDateToday(DateTime? date) {
    if (date == null) return false;
    final now = DateTime.now();
    return date.year == now.year &&
        date.month == now.month &&
        date.day == now.day;
  }

  @override
  Widget build(BuildContext context) {
    final movieProvider = Provider.of<MovieProvider>(context);
    final moviesState = movieProvider.moviesState;

    // Determine if we should show filter chips
    final bool shouldShowFilters =
        !(_selectedGenres.contains('Все') &&
            _isDateToday(_selectedDate) &&
            _currentSortOption == 'title_asc');

    return SingleChildScrollView(
      controller: _scrollController,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Баннер с новинками
          Container(
            height: 180, // Adjusted height as the button is removed
            width: double.infinity,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                colors: [
                  AppTheme.accentColor,
                  AppTheme.accentColor.withOpacity(0.8),
                ],
              ),
            ),
            child: Stack(
              children: [
                Positioned.fill(
                  child: Padding(
                    padding: const EdgeInsets.only(top: 32),
                    child: Opacity(
                      opacity: 0.2, // Slightly more subtle background icon
                      child: Icon(
                        Icons
                            .local_movies_outlined, // A more generic movie-related icon
                        size: 100,
                        color: Colors.white.withOpacity(0.5),
                      ),
                    ),
                  ),
                ),
                Center(
                  child: Padding(
                    padding: const EdgeInsets.only(
                      top: 42,
                      bottom: 16,
                      left: 16,
                      right: 16,
                    ),
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      crossAxisAlignment: CrossAxisAlignment.center,
                      children: [
                        const Text(
                          'Добро пожаловать!',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            color: Colors.white,
                            fontSize: 28, // Larger font for welcome message
                            fontWeight: FontWeight.bold,
                            shadows: [
                              // Adding a subtle shadow for better readability
                              Shadow(
                                blurRadius: 4.0,
                                color: Colors.black38,
                                offset: Offset(2.0, 2.0),
                              ),
                            ],
                          ),
                        ),
                        const SizedBox(height: 12),
                        const Text(
                          'Откройте для себя мир кино',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            color:
                                Colors.white70, // Slightly dimmer for subtitle
                            fontSize: 18,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ),

          const SizedBox(height: 16),

          // Search bar and filter button
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            child: Row(
              children: [
                Expanded(
                  child: TextField(
                    controller: _searchController,
                    decoration: InputDecoration(
                      hintText: 'Поиск фильмов...',
                      prefixIcon: const Icon(Icons.search),
                      suffixIcon:
                          _searchController.text.isNotEmpty
                              ? IconButton(
                                icon: const Icon(Icons.clear),
                                onPressed: () {
                                  setState(() {
                                    _searchController.clear();
                                  });
                                  _applyFilters();
                                },
                              )
                              : null,
                      border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(30),
                      ),
                      contentPadding: const EdgeInsets.symmetric(vertical: 0),
                    ),
                    onSubmitted: (_) => _applyFilters(),
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.filter_list),
                  onPressed: _showFilterBottomSheet,
                  tooltip: 'Фильтры',
                ),
              ],
            ),
          ),

          const SizedBox(height: 16),

          // Filter chips display (show selected filters)
          if (shouldShowFilters)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              child: Wrap(
                spacing: 8,
                runSpacing: 8,
                children: [
                  // Date filter chip
                  if (_selectedDate != null &&
                      (!_isDateToday(_selectedDate) ||
                          !_selectedGenres.contains('Все')))
                    Chip(
                      label: Text(
                        DateFormat('dd-MM-yyyy').format(_selectedDate!),
                      ),
                      backgroundColor: AppTheme.cardColor,
                      deleteIcon: const Icon(Icons.clear, size: 16),
                      onDeleted: () {
                        setState(() {
                          _selectedDate = DateTime(
                            DateTime.now().year,
                            DateTime.now().month,
                            DateTime.now().day,
                          );
                        });
                        _applyFilters();
                      },
                      labelStyle: TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.normal,
                      ),
                      side: BorderSide(color: AppTheme.accentColor),
                    ),

                  // Sort filter chip (always show unless default)
                  if (_currentSortOption !=
                      'title_asc') // Only show if not default sort
                    Chip(
                      avatar: Icon(Icons.sort, size: 16, color: Colors.white70),
                      label: Text(_sortOptions[_currentSortOption]!),
                      backgroundColor: AppTheme.cardColor,
                      deleteIcon: const Icon(Icons.clear, size: 16),
                      onDeleted: () {
                        setState(() {
                          _sortBy = 'title';
                          _sortDirection = 'asc';
                        });
                        _applyFilters();
                      },
                      labelStyle: TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.normal,
                      ),
                      side: BorderSide(color: AppTheme.accentColor),
                    ),

                  // Genre filter chips
                  if (!_selectedGenres.contains('Все'))
                    ..._selectedGenres.map((genre) {
                      return GestureDetector(
                        onTap: () {
                          _toggleGenreSelection(genre);
                          _applyFilters();
                        },
                        child: Chip(
                          label: Text(genre),
                          backgroundColor: AppTheme.cardColor,
                          deleteIcon: const Icon(Icons.clear, size: 16),
                          onDeleted: () {
                            _toggleGenreSelection(genre);
                            _applyFilters();
                          },
                          labelStyle: TextStyle(
                            color: Colors.white,
                            fontWeight: FontWeight.normal,
                          ),
                          side: BorderSide(color: AppTheme.accentColor),
                        ),
                      );
                    }).toList(),
                ],
              ),
            ),

          // Список всех фильмов с пагинацией
          Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      _selectedGenres.contains('Все')
                          ? 'Все фильмы'
                          : 'Результаты',
                      style: const TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    if (moviesState.status == MovieStatus.loaded)
                      Text(
                        'Всего: ${moviesState.total}',
                        style: TextStyle(color: Colors.grey.shade600),
                      ),
                  ],
                ),
                // const SizedBox(height: 4),

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
                            onPressed: () => _applyFilters(),
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

  // Helper method to get the formatted date
  String _formatDate(DateTime date) {
    return DateFormat('dd-MM-yyyy').format(date);
  }
}
