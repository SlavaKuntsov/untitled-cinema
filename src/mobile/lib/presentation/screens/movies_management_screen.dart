import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../core/constants/api_constants.dart';
import '../../domain/entities/movie/genre.dart';
import '../../domain/entities/movie/movie.dart';
import '../providers/movie_management_provider.dart';
import '../widgets/confirmation_dialog.dart';
import 'booking_statistics_screen.dart';
import 'movie_details_screen.dart';
import 'movie_form_screen.dart';

class MoviesManagementScreen extends StatefulWidget {
  const MoviesManagementScreen({super.key});

  @override
  State<MoviesManagementScreen> createState() => _MoviesManagementScreenState();
}

class _MoviesManagementScreenState extends State<MoviesManagementScreen>
    with SingleTickerProviderStateMixin {
  bool _isLoading = true;
  String? _errorMessage;
  late TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    _loadMovies();
    _loadGenres();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _loadMovies() async {
    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      await movieManagementProvider.fetchAllMovies();
      setState(() {
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _errorMessage = 'Ошибка загрузки фильмов: ${e.toString()}';
        _isLoading = false;
      });
    }
  }

  Future<void> _loadGenres() async {
    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    try {
      await movieManagementProvider.fetchAllGenres();
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка загрузки жанров: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _showDeleteMovieConfirmation(Movie movie) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder:
          (context) => ConfirmationDialog(
            title: 'Удаление фильма',
            content: 'Вы уверены, что хотите удалить фильм "${movie.title}"?',
            confirmText: 'Удалить',
            cancelText: 'Отмена',
          ),
    );

    if (confirmed == true) {
      _deleteMovie(movie);
    }
  }

  Future<void> _deleteMovie(Movie movie) async {
    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    try {
      await movieManagementProvider.deleteMovie(movie.id);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Фильм "${movie.title}" успешно удален'),
          backgroundColor: Colors.green,
        ),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при удалении фильма: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  void _showMovieDetails(Movie movie) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => MovieDetailsScreen(movieId: movie.id),
      ),
    );
  }

  void _showAddMovie() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => const MovieFormScreen()),
    );
  }

  Future<void> _showDeleteGenreConfirmation(Genre genre) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder:
          (context) => ConfirmationDialog(
            title: 'Удаление жанра',
            content: 'Вы уверены, что хотите удалить жанр "${genre.name}"?',
            confirmText: 'Удалить',
            cancelText: 'Отмена',
          ),
    );

    if (confirmed == true) {
      _deleteGenre(genre);
    }
  }

  Future<void> _deleteGenre(Genre genre) async {
    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    try {
      await movieManagementProvider.deleteGenre(genre.id);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Жанр "${genre.name}" успешно удален'),
          backgroundColor: Colors.green,
        ),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при удалении жанра: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _showEditGenreDialog(Genre genre) async {
    final nameController = TextEditingController(text: genre.name);

    final confirmed = await showDialog<bool>(
      context: context,
      builder:
          (context) => AlertDialog(
            title: const Text('Редактировать жанр'),
            content: TextField(
              controller: nameController,
              decoration: const InputDecoration(labelText: 'Название жанра'),
              autofocus: true,
            ),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(context, false),
                child: const Text('Отмена'),
              ),
              TextButton(
                onPressed: () => Navigator.pop(context, true),
                child: const Text('Сохранить'),
              ),
            ],
          ),
    );

    if (confirmed == true) {
      _updateGenre(genre.id, nameController.text);
    }
  }

  Future<void> _updateGenre(String id, String name) async {
    if (name.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Название жанра не может быть пустым'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    try {
      await movieManagementProvider.updateGenre(id: id, name: name);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Жанр успешно обновлен'),
          backgroundColor: Colors.green,
        ),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при обновлении жанра: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Управление фильмами'),
        backgroundColor: AppTheme.primaryColor,
        bottom: TabBar(
          controller: _tabController,
          labelColor: Colors.white,
          unselectedLabelColor: Colors.grey,
          tabs: const [Tab(text: 'Фильмы'), Tab(text: 'Жанры'), Tab(text: 'Статистика')],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [_buildMoviesTab(), _buildGenresTab(), _buildStatisticsTab()],
      ),
      floatingActionButton:
          _tabController.index == 0
              ? FloatingActionButton(
                onPressed: _showAddMovie,
                backgroundColor: AppTheme.accentColor,
                child: const Icon(Icons.add),
              )
              : null,
    );
  }

  Widget _buildMoviesTab() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_errorMessage != null) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              _errorMessage!,
              style: const TextStyle(color: Colors.red),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _loadMovies,
              child: const Text('Повторить'),
            ),
          ],
        ),
      );
    }

    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
    );
    final movies = movieManagementProvider.movies;

    if (movies.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Text('Нет доступных фильмов'),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _showAddMovie,
              child: const Text('Добавить фильм'),
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _loadMovies,
      child: ListView.builder(
        padding: const EdgeInsets.all(16),
        itemCount: movies.length,
        itemBuilder: (context, index) {
          final movie = movies[index];
          return _buildMovieCard(movie);
        },
      ),
    );
  }

  Widget _buildMovieCard(Movie movie) {
    final dateFormat = DateFormat('dd-MM-yyyy');

    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: InkWell(
        onTap: () => _showMovieDetails(movie),
        borderRadius: BorderRadius.circular(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Movie poster and info
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Poster
                ClipRRect(
                  borderRadius: const BorderRadius.only(
                    topLeft: Radius.circular(12),
                    bottomLeft: Radius.circular(12),
                  ),
                  child: SizedBox(
                    width: 120,
                    height: 180,
                    child:
                        movie.poster.isNotEmpty
                            ? Image.network(
                              '${ApiConstants.moviePoster}/${movie.poster}',
                              fit: BoxFit.cover,
                              errorBuilder: (context, error, stackTrace) {
                                return Container(
                                  color: Colors.grey[300],
                                  child: const Center(
                                    child: Icon(Icons.movie, size: 50),
                                  ),
                                );
                              },
                              loadingBuilder: (context, child, loadingProgress) {
                                if (loadingProgress == null) return child;
                                return Container(
                                  color: Colors.grey.shade100,
                                  child: const Center(
                                    child: CircularProgressIndicator(),
                                  ),
                                );
                              },
                            )
                            : Container(
                              color: Colors.grey[300],
                              child: const Center(
                                child: Icon(Icons.movie, size: 50),
                              ),
                            ),
                  ),
                ),
                // Movie details
                Expanded(
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          movie.title,
                          style: const TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color: Colors.white,
                          ),
                        ),
                        const SizedBox(height: 8),
                        Text(
                          'Длительность: ${movie.durationMinutes} мин',
                          style: const TextStyle(color: Colors.white),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'Возраст: ${movie.ageLimit}+',
                          style: const TextStyle(color: Colors.white),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'Цена: ${movie.price} руб.',
                          style: const TextStyle(color: Colors.white),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'Релиз: ${dateFormat.format(movie.releaseDate)}',
                          style: const TextStyle(color: Colors.white),
                        ),
                        const SizedBox(height: 8),
                        Wrap(
                          spacing: 4,
                          runSpacing: 4,
                          children:
                              movie.genres.map((genre) {
                                return Chip(
                                  label: Text(
                                    genre,
                                    style: const TextStyle(
                                      fontSize: 12,
                                      color: Colors.white,
                                    ),
                                  ),
                                  padding: EdgeInsets.zero,
                                  materialTapTargetSize:
                                      MaterialTapTargetSize.shrinkWrap,
                                  backgroundColor: AppTheme.primaryColor,
                                  side: const BorderSide(
                                    color: AppTheme.accentColor,
                                    width: 1,
                                  ),
                                );
                              }).toList(),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
            // Actions
            ButtonBar(
              alignment: MainAxisAlignment.end,
              children: [
                TextButton.icon(
                  onPressed: () => _showMovieDetails(movie),
                  icon: const Icon(Icons.edit, color: Colors.blue),
                  label: const Text(
                    'Редактировать',
                    style: TextStyle(color: Colors.blue),
                  ),
                ),
                TextButton.icon(
                  onPressed: () => _showDeleteMovieConfirmation(movie),
                  icon: const Icon(Icons.delete, color: Colors.red),
                  label: const Text(
                    'Удалить',
                    style: TextStyle(color: Colors.red),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildGenresTab() {
    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
    );
    final genres = movieManagementProvider.genres;

    if (genres.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Text('Нет доступных жанров'),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _loadGenres,
              child: const Text('Обновить'),
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _loadGenres,
      child: ListView.builder(
        padding: const EdgeInsets.all(16),
        itemCount: genres.length,
        itemBuilder: (context, index) {
          final genre = genres[index];
          return Card(
            margin: const EdgeInsets.only(bottom: 8),
            child: ListTile(
              title: Text(genre.name),
              trailing: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  IconButton(
                    icon: const Icon(Icons.edit, color: Colors.blue),
                    onPressed: () => _showEditGenreDialog(genre),
                  ),
                  IconButton(
                    icon: const Icon(Icons.delete, color: Colors.red),
                    onPressed: () => _showDeleteGenreConfirmation(genre),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildStatisticsTab() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          const Text(
            'Статистика продаж билетов',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 20),
          const Text(
            'Здесь вы можете просмотреть статистику продаж билетов по периодам',
            textAlign: TextAlign.center,
            style: TextStyle(fontSize: 16),
          ),
          const SizedBox(height: 24),
          ElevatedButton.icon(
            onPressed: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => const BookingStatisticsScreen(),
                ),
              );
            },
            icon: const Icon(Icons.bar_chart),
            label: const Text('Открыть статистику'),
            style: ElevatedButton.styleFrom(
              foregroundColor: Colors.white,
              backgroundColor: AppTheme.accentColor,
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            ),
          ),
        ],
      ),
    );
  }
}
