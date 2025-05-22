import 'dart:io';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../core/constants/api_constants.dart';
import '../../domain/entities/movie/movie.dart';
import '../../domain/entities/movie/movie_frame.dart';
import '../providers/movie_management_provider.dart';
import '../widgets/confirmation_dialog.dart';

class MovieDetailsScreen extends StatefulWidget {
  final String movieId;

  const MovieDetailsScreen({super.key, required this.movieId});

  @override
  State<MovieDetailsScreen> createState() => _MovieDetailsScreenState();
}

class _MovieDetailsScreenState extends State<MovieDetailsScreen>
    with SingleTickerProviderStateMixin {
  bool _isLoading = true;
  String? _errorMessage;
  late TabController _tabController;
  Movie? _movie;
  final _formKey = GlobalKey<FormState>();

  final _titleController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _priceController = TextEditingController();
  final _durationController = TextEditingController();
  final _producerController = TextEditingController();
  final _inRolesController = TextEditingController();
  final _ageLimitController = TextEditingController();

  DateTime _releaseDate = DateTime.now();
  List<String> _selectedGenres = [];
  bool _isEditing = false;
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _loadMovieDetails();
  }

  @override
  void dispose() {
    _tabController.dispose();
    _titleController.dispose();
    _descriptionController.dispose();
    _priceController.dispose();
    _durationController.dispose();
    _producerController.dispose();
    _inRolesController.dispose();
    _ageLimitController.dispose();
    super.dispose();
  }

  Future<void> _loadMovieDetails() async {
    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      final movie = await movieManagementProvider.fetchMovieById(
        widget.movieId,
      );
      if (movie != null) {
        _movie = movie;
        _initializeFormControllers(movie);
        await movieManagementProvider.fetchFramesByMovieId(widget.movieId);
        await movieManagementProvider.fetchAllGenres();
      }

      setState(() {
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _errorMessage = 'Ошибка загрузки данных фильма: ${e.toString()}';
        _isLoading = false;
      });
    }
  }

  void _initializeFormControllers(Movie movie) {
    _titleController.text = movie.title;
    _descriptionController.text = movie.description;
    _priceController.text = movie.price.toString();
    _durationController.text = movie.durationMinutes.toString();
    _producerController.text = movie.producer;
    _inRolesController.text = movie.inRoles;
    _ageLimitController.text = movie.ageLimit.toString();
    _releaseDate = movie.releaseDate;
    _selectedGenres = List.from(movie.genres);
  }

  Future<void> _pickImage() async {
    final imagePicker = ImagePicker();
    final pickedFile = await imagePicker.pickImage(source: ImageSource.gallery);

    if (pickedFile != null) {
      _uploadPoster(File(pickedFile.path));
    }
  }

  Future<void> _uploadPoster(File file) async {
    if (_movie == null) return;

    setState(() {
      _isLoading = true;
    });

    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    try {
      final posterUrl = await movieManagementProvider.uploadMoviePoster(
        _movie!.id,
        file,
      );

      if (posterUrl != null) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Постер успешно обновлен'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при загрузке постера: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future<void> _pickAndUploadFrame() async {
    if (_movie == null) return;

    final imagePicker = ImagePicker();
    final pickedFile = await imagePicker.pickImage(source: ImageSource.gallery);

    if (pickedFile != null) {
      _uploadFrame(File(pickedFile.path));
    }
  }

  Future<void> _uploadFrame(File file) async {
    if (_movie == null) return;

    setState(() {
      _isLoading = true;
    });

    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    try {
      final frame = await movieManagementProvider.addMovieFrame(
        _movie!.id,
        file,
      );

      if (frame != null) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Кадр успешно добавлен'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при загрузке кадра: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future<void> _deleteFrame(MovieFrame frame) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder:
          (context) => ConfirmationDialog(
            title: 'Удаление кадра',
            content: 'Вы уверены, что хотите удалить выбранный кадр?',
            confirmText: 'Удалить',
            cancelText: 'Отмена',
          ),
    );

    if (confirmed != true) return;

    setState(() {
      _isLoading = true;
    });

    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    try {
      await movieManagementProvider.deleteMovieFrame(frame.id);
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Кадр успешно удален'),
          backgroundColor: Colors.green,
        ),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при удалении кадра: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future<void> _saveChanges() async {
    if (_movie == null || !_formKey.currentState!.validate()) return;

    setState(() {
      _isSaving = true;
    });

    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    try {
      final updatedMovie = await movieManagementProvider.updateMovie(
        id: _movie!.id,
        title: _titleController.text,
        description: _descriptionController.text,
        genres: _selectedGenres,
        price: double.parse(_priceController.text),
        durationMinutes: int.parse(_durationController.text),
        producer: _producerController.text,
        inRoles: _inRolesController.text,
        ageLimit: int.parse(_ageLimitController.text),
        releaseDate: _releaseDate,
      );

      if (updatedMovie != null) {
        _movie = updatedMovie;
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Фильм успешно обновлен'),
            backgroundColor: Colors.green,
          ),
        );
        setState(() {
          _isEditing = false;
        });
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при обновлении фильма: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      setState(() {
        _isSaving = false;
      });
    }
  }

  Future<void> _selectReleaseDate(BuildContext context) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: _releaseDate,
      firstDate: DateTime(2000),
      lastDate: DateTime(2100),
    );
    if (picked != null && picked != _releaseDate) {
      setState(() {
        _releaseDate = picked;
      });
    }
  }

  void _toggleGenre(String genre) {
    setState(() {
      if (_selectedGenres.contains(genre)) {
        _selectedGenres.remove(genre);
      } else {
        _selectedGenres.add(genre);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(_movie?.title ?? 'Детали фильма'),
        backgroundColor: AppTheme.primaryColor,
        actions: [
          if (_movie != null && !_isEditing)
            IconButton(
              icon: const Icon(Icons.edit),
              onPressed: () => setState(() => _isEditing = true),
            ),
          if (_movie != null && _isEditing)
            IconButton(
              icon: const Icon(Icons.cancel),
              onPressed: () {
                _initializeFormControllers(_movie!);
                setState(() => _isEditing = false);
              },
            ),
          if (_movie != null && _isEditing)
            IconButton(
              icon:
                  _isSaving
                      ? const SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(
                          color: Colors.white,
                          strokeWidth: 2,
                        ),
                      )
                      : const Icon(Icons.save),
              onPressed: _isSaving ? null : _saveChanges,
            ),
        ],
        bottom: TabBar(
          controller: _tabController,
          tabs: const [Tab(text: 'Информация'), Tab(text: 'Кадры из фильма')],
        ),
      ),
      body:
          _isLoading
              ? const Center(child: CircularProgressIndicator())
              : _errorMessage != null
              ? Center(
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
                      onPressed: _loadMovieDetails,
                      child: const Text('Повторить'),
                    ),
                  ],
                ),
              )
              : _movie == null
              ? const Center(child: Text('Фильм не найден'))
              : TabBarView(
                controller: _tabController,
                children: [_buildInfoTab(), _buildFramesTab()],
              ),
    );
  }

  Widget _buildInfoTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildPosterSection(),
          const SizedBox(height: 24),
          _isEditing ? _buildEditForm() : _buildMovieInfo(),
        ],
      ),
    );
  }

  Widget _buildPosterSection() {
    return Center(
      child: Stack(
        children: [
          Container(
            width: 200,
            height: 300,
            decoration: BoxDecoration(
              color: Colors.grey[300],
              borderRadius: BorderRadius.circular(8),
              image:
                  _movie!.poster.isNotEmpty
                      ? DecorationImage(
                        image: NetworkImage(
                          '${ApiConstants.moviePoster}/${_movie!.poster}',
                        ),
                        fit: BoxFit.cover,
                      )
                      : null,
            ),
            child:
                _movie!.poster.isEmpty
                    ? const Center(
                      child: Icon(Icons.movie, size: 64, color: Colors.grey),
                    )
                    : null,
          ),
          if (_isEditing)
            Positioned(
              bottom: 0,
              right: 0,
              child: Container(
                decoration: BoxDecoration(
                  color: Colors.black54,
                  borderRadius: BorderRadius.circular(20),
                ),
                child: IconButton(
                  icon: const Icon(Icons.photo_camera, color: Colors.white),
                  onPressed: _pickImage,
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildMovieInfo() {
    final dateFormat = DateFormat('dd-MM-yyyy');

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          _movie!.title,
          style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 16),
        Text(_movie!.description, style: const TextStyle(fontSize: 16)),
        const SizedBox(height: 16),
        const Divider(),
        _buildInfoItem('Жанры', _buildGenreChips()),
        _buildInfoItem('Цена', '${_movie!.price} руб.'),
        _buildInfoItem('Длительность', '${_movie!.durationMinutes} минут'),
        _buildInfoItem('Режиссер', _movie!.producer),
        _buildInfoItem('В ролях', _movie!.inRoles),
        _buildInfoItem('Возрастное ограничение', '${_movie!.ageLimit}+'),
        _buildInfoItem('Дата выхода', dateFormat.format(_movie!.releaseDate)),
        _buildInfoItem('Дата создания', dateFormat.format(_movie!.createdAt)),
        _buildInfoItem('Дата изменения', dateFormat.format(_movie!.updatedAt)),
      ],
    );
  }

  Widget _buildGenreChips() {
    return Wrap(
      spacing: 4,
      runSpacing: 4,
      children:
          _movie!.genres.map((genre) {
            return Chip(
              label: Text(genre, style: const TextStyle(fontSize: 12)),
              backgroundColor: Colors.blue[100],
              padding: EdgeInsets.zero,
              materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
            );
          }).toList(),
    );
  }

  Widget _buildInfoItem(String label, dynamic value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 150,
            child: Text(
              label,
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                color: Colors.white,
              ),
            ),
          ),
          Expanded(child: value is Widget ? value : Text(value.toString())),
        ],
      ),
    );
  }

  Widget _buildEditForm() {
    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
    );
    final allGenres = movieManagementProvider.genres;
    final dateFormat = DateFormat('dd-MM-yyyy');

    return Form(
      key: _formKey,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          TextFormField(
            controller: _titleController,
            decoration: const InputDecoration(
              labelText: 'Название фильма',
              border: OutlineInputBorder(),
            ),
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'Введите название фильма';
              }
              return null;
            },
          ),
          const SizedBox(height: 16),
          TextFormField(
            controller: _descriptionController,
            decoration: const InputDecoration(
              labelText: 'Описание',
              border: OutlineInputBorder(),
              alignLabelWithHint: true,
            ),
            maxLines: 5,
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'Введите описание фильма';
              }
              return null;
            },
          ),
          const SizedBox(height: 16),
          const Text(
            'Жанры',
            style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
          ),
          const SizedBox(height: 8),
          Wrap(
            spacing: 8,
            runSpacing: 4,
            children:
                allGenres.map((genre) {
                  final isSelected = _selectedGenres.contains(genre.name);
                  return FilterChip(
                    label: Text(genre.name),
                    selected: isSelected,
                    onSelected: (_) => _toggleGenre(genre.name),
                    backgroundColor: Colors.grey[200],
                    selectedColor: Colors.blue[100],
                  );
                }).toList(),
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: TextFormField(
                  controller: _priceController,
                  decoration: const InputDecoration(
                    labelText: 'Цена (руб)',
                    border: OutlineInputBorder(),
                  ),
                  keyboardType: TextInputType.number,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Введите цену';
                    }
                    if (double.tryParse(value) == null) {
                      return 'Введите корректное число';
                    }
                    return null;
                  },
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: TextFormField(
                  controller: _durationController,
                  decoration: const InputDecoration(
                    labelText: 'Длительность (мин)',
                    border: OutlineInputBorder(),
                  ),
                  keyboardType: TextInputType.number,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Введите длительность';
                    }
                    if (int.tryParse(value) == null) {
                      return 'Введите целое число';
                    }
                    return null;
                  },
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          TextFormField(
            controller: _producerController,
            decoration: const InputDecoration(
              labelText: 'Режиссер',
              border: OutlineInputBorder(),
            ),
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'Введите имя режиссера';
              }
              return null;
            },
          ),
          const SizedBox(height: 16),
          TextFormField(
            controller: _inRolesController,
            decoration: const InputDecoration(
              labelText: 'В ролях',
              border: OutlineInputBorder(),
              alignLabelWithHint: true,
            ),
            maxLines: 2,
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'Введите список актеров';
              }
              return null;
            },
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: TextFormField(
                  controller: _ageLimitController,
                  decoration: const InputDecoration(
                    labelText: 'Возрастное ограничение',
                    border: OutlineInputBorder(),
                  ),
                  keyboardType: TextInputType.number,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Введите ограничение';
                    }
                    if (int.tryParse(value) == null) {
                      return 'Введите целое число';
                    }
                    return null;
                  },
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: InkWell(
                  onTap: () => _selectReleaseDate(context),
                  child: InputDecorator(
                    decoration: const InputDecoration(
                      labelText: 'Дата выхода',
                      border: OutlineInputBorder(),
                    ),
                    child: Text(dateFormat.format(_releaseDate)),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),
        ],
      ),
    );
  }

  Widget _buildFramesTab() {
    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
    );
    final frames = movieManagementProvider.frames;

    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(16),
          child: ElevatedButton.icon(
            onPressed: _pickAndUploadFrame,
            icon: const Icon(Icons.add_photo_alternate),
            label: const Text('Добавить кадр'),
            style: ElevatedButton.styleFrom(
              foregroundColor: Colors.white,
              backgroundColor: AppTheme.accentColor,
              padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 16),
            ),
          ),
        ),
        Expanded(
          child:
              frames.isEmpty
                  ? const Center(
                    child: Text(
                      'Нет доступных кадров.\nДобавьте новый кадр!',
                      textAlign: TextAlign.center,
                    ),
                  )
                  : GridView.builder(
                    padding: const EdgeInsets.all(16),
                    gridDelegate:
                        const SliverGridDelegateWithFixedCrossAxisCount(
                          crossAxisCount: 2,
                          childAspectRatio: 1.5,
                          crossAxisSpacing: 16,
                          mainAxisSpacing: 16,
                        ),
                    itemCount: frames.length,
                    itemBuilder: (context, index) {
                      final frame = frames[index];
                      return _buildFrameItem(frame);
                    },
                  ),
        ),
      ],
    );
  }

  Widget _buildFrameItem(MovieFrame frame) {
    return Stack(
      fit: StackFit.expand,
      children: [
        ClipRRect(
          borderRadius: BorderRadius.circular(8),
          child: Image.network(
            '${ApiConstants.movieServiceBaseUrl}/movies/frames/${frame.frameName}',
            fit: BoxFit.cover,
            errorBuilder: (context, error, stackTrace) {
              return Container(
                color: Colors.grey[300],
                child: const Center(child: Icon(Icons.broken_image, size: 40)),
              );
            },
            loadingBuilder: (context, child, loadingProgress) {
              if (loadingProgress == null) return child;
              return Container(
                color: Colors.grey.shade100,
                child: const Center(child: CircularProgressIndicator()),
              );
            },
          ),
        ),
        if (_isEditing)
          Positioned(
            top: 4,
            right: 4,
            child: Container(
              decoration: BoxDecoration(
                color: Colors.black54,
                borderRadius: BorderRadius.circular(16),
              ),
              child: IconButton(
                icon: const Icon(Icons.delete, color: Colors.white, size: 20),
                onPressed: () => _deleteFrame(frame),
                constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
                padding: EdgeInsets.zero,
              ),
            ),
          ),
      ],
    );
  }
}
