import 'package:flutter/material.dart';

import '../../../domain/entities/movie/movie.dart';

class MovieSessionScreen extends StatefulWidget {
  const MovieSessionScreen({super.key});

  @override
  State<MovieSessionScreen> createState() => _MovieSessionScreenState();
}

class _MovieSessionScreenState extends State<MovieSessionScreen> {
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
    // setState(() {
    //   _isLoading = true;
    //   _errorMessage = null;
    // });
    //
    // try {
    //   final provider = Provider.of<MovieProvider>(context, listen: false);
    //   // Получаем фильм по ID
    //   _movie = await provider.fetchMovieById(id: widget.movieId);
    //   _frames = await provider.fetchMoviesFrames(id: widget.movieId);
    //   setState(() {
    //     _isLoading = false;
    //   });
    // } catch (e) {
    //   setState(() {
    //     _isLoading = false;
    //     _errorMessage = 'Ошибка при загрузке данных: ${e.toString()}';
    //   });
    // }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(_movie?.title ?? 'Загрузка фильма...'),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.of(context).pop(),
        ),
      ),
      body: Text('SESSION'),
    );
  }
}
