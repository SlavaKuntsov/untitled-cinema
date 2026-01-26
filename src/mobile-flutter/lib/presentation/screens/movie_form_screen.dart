import 'dart:io';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../domain/entities/movie/movie.dart';
import '../../domain/entities/movie/genre.dart';
import '../providers/movie_management_provider.dart';

class MovieFormScreen extends StatefulWidget {
  const MovieFormScreen({super.key});

  @override
  State<MovieFormScreen> createState() => _MovieFormScreenState();
}

class _MovieFormScreenState extends State<MovieFormScreen> {
  final _formKey = GlobalKey<FormState>();
  
  final _titleController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _priceController = TextEditingController(text: '0.0');
  final _durationController = TextEditingController(text: '0');
  final _producerController = TextEditingController();
  final _inRolesController = TextEditingController();
  final _ageLimitController = TextEditingController(text: '0');
  
  DateTime _releaseDate = DateTime.now();
  List<String> _selectedGenres = [];
  File? _posterFile;
  bool _isLoading = false;
  bool _isCreating = false;

  @override
  void initState() {
    super.initState();
    _loadGenres();
  }

  @override
  void dispose() {
    _titleController.dispose();
    _descriptionController.dispose();
    _priceController.dispose();
    _durationController.dispose();
    _producerController.dispose();
    _inRolesController.dispose();
    _ageLimitController.dispose();
    super.dispose();
  }

  Future<void> _loadGenres() async {
    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    setState(() {
      _isLoading = true;
    });

    try {
      await movieManagementProvider.fetchAllGenres();
      setState(() {
        _isLoading = false;
      });
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка загрузки жанров: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future<void> _pickImage() async {
    final imagePicker = ImagePicker();
    final pickedFile = await imagePicker.pickImage(source: ImageSource.gallery);

    if (pickedFile != null) {
      setState(() {
        _posterFile = File(pickedFile.path);
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

  Future<void> _createMovie() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    if (_selectedGenres.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Выберите хотя бы один жанр'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    setState(() {
      _isCreating = true;
    });

    final movieManagementProvider = Provider.of<MovieManagementProvider>(
      context,
      listen: false,
    );

    try {
      final movie = await movieManagementProvider.createMovie(
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

      if (movie != null && _posterFile != null) {
        await movieManagementProvider.uploadMoviePoster(
          movie.id,
          _posterFile!,
        );
      }

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Фильм успешно создан'),
          backgroundColor: Colors.green,
        ),
      );

      Navigator.pop(context);
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при создании фильма: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      setState(() {
        _isCreating = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Добавление фильма'),
        backgroundColor: AppTheme.primaryColor,
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    _buildPosterSection(),
                    const SizedBox(height: 24),
                    _buildForm(),
                    const SizedBox(height: 24),
                    Center(
                      child: ElevatedButton(
                        onPressed: _isCreating ? null : _createMovie,
                        style: ElevatedButton.styleFrom(
                          foregroundColor: Colors.white,
                          backgroundColor: AppTheme.accentColor,
                          padding: const EdgeInsets.symmetric(
                            horizontal: 32,
                            vertical: 12,
                          ),
                        ),
                        child: _isCreating
                            ? const SizedBox(
                                width: 20,
                                height: 20,
                                child: CircularProgressIndicator(
                                  color: Colors.white,
                                  strokeWidth: 2,
                                ),
                              )
                            : const Text('Создать фильм'),
                      ),
                    ),
                  ],
                ),
              ),
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
              image: _posterFile != null
                  ? DecorationImage(
                      image: FileImage(_posterFile!),
                      fit: BoxFit.cover,
                    )
                  : null,
            ),
            child: _posterFile == null
                ? const Center(
                    child: Text(
                      'Постер фильма',
                      style: TextStyle(color: Colors.grey),
                    ),
                  )
                : null,
          ),
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

  Widget _buildForm() {
    final movieManagementProvider = Provider.of<MovieManagementProvider>(context);
    final allGenres = movieManagementProvider.genres;
    final dateFormat = DateFormat('dd-MM-yyyy');

    return Column(
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
          style: TextStyle(
            fontWeight: FontWeight.bold,
            fontSize: 16,
          ),
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8,
          runSpacing: 4,
          children: allGenres.map((genre) {
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
      ],
    );
  }
} 