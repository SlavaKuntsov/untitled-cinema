import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';

import '../../../config/theme.dart';
import '../../../core/constants/api_constants.dart';
import '../../../domain/entities/movie/movie.dart';
import '../../providers/movie_provider.dart';

class MovieListWidget extends StatelessWidget {
  final MoviesState moviesState;
  final Function(String) onMovieTap;
  final List<String> Function(Movie movie) getShowTimes;
  final Color Function(int ageLimit) getAgeLimitColor;

  const MovieListWidget({
    super.key,
    required this.moviesState,
    required this.onMovieTap,
    required this.getShowTimes,
    required this.getAgeLimitColor,
  });

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      cacheExtent: 500,
      itemCount:
          moviesState.movies.length +
          (moviesState.status == MovieStatus.loading ? 1 : 0),
      itemBuilder: (context, index) {
        if (index == moviesState.movies.length) {
          return const Padding(
            padding: EdgeInsets.all(16.0),
            child: Center(child: CircularProgressIndicator()),
          );
        }

        final movie = moviesState.movies[index];
        return _MovieCard(
          movie: movie,
          showTimes: getShowTimes(movie),
          onTap: () => onMovieTap(movie.id),
          ageLimitColor: getAgeLimitColor(movie.ageLimit),
        );
      },
    );
  }
}

class _MovieCard extends StatelessWidget {
  final dynamic movie;
  final List<String> showTimes;
  final VoidCallback onTap;
  final Color ageLimitColor;

  const _MovieCard({
    required this.movie,
    required this.showTimes,
    required this.onTap,
    required this.ageLimitColor,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _MoviePoster(poster: movie.poster),
            Expanded(
              child: Padding(
                padding: const EdgeInsets.all(12),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    _MovieHeader(
                      title: movie.title,
                      ageLimit: movie.ageLimit,
                      ageLimitColor: ageLimitColor,
                    ),
                    const SizedBox(height: 8),
                    _GenreChips(genres: movie.genres),
                    const SizedBox(height: 12),
                    _MovieMetaInfo(
                      duration: movie.durationMinutes,
                      price: movie.price,
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _MoviePoster extends StatelessWidget {
  final String poster;

  const _MoviePoster({required this.poster});

  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      borderRadius: const BorderRadius.only(
        topLeft: Radius.circular(12),
        bottomLeft: Radius.circular(12),
      ),
      child:
          poster.isNotEmpty
              ? CachedNetworkImage(
                imageUrl: '${ApiConstants.moviePoster}/$poster',
                width: 130,
                height: 160,
                fit: BoxFit.cover,
                placeholder:
                    (context, url) => Container(
                      width: 130,
                      height: 160,
                      color: Colors.grey[300],
                      child: const Center(
                        child: SizedBox(
                          width: 30,
                          height: 30,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        ),
                      ),
                    ),
                errorWidget: (context, url, error) {
                  debugPrint('Error loading image: $error');
                  return const _PlaceholderPoster();
                },
                fadeInDuration: const Duration(milliseconds: 100),
              )
              : const _PlaceholderPoster(),
    );
  }
}

class _PlaceholderPoster extends StatelessWidget {
  const _PlaceholderPoster();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 130,
      height: 160,
      color: Colors.grey.shade300,
      child: const Icon(Icons.movie, size: 50, color: Colors.white),
    );
  }
}

class _MovieHeader extends StatelessWidget {
  final String title;
  final int ageLimit;
  final Color ageLimitColor;

  const _MovieHeader({
    required this.title,
    required this.ageLimit,
    required this.ageLimitColor,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Expanded(
          child: Text(
            title,
            style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
          ),
        ),
        const SizedBox(width: 8),
        if (ageLimit > 0)
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
            decoration: BoxDecoration(
              color: ageLimitColor,
              borderRadius: BorderRadius.circular(4),
            ),
            child: Text(
              '$ageLimit+',
              style: const TextStyle(
                color: Colors.white,
                fontWeight: FontWeight.bold,
                fontSize: 16,
              ),
            ),
          ),
      ],
    );
  }
}

class _GenreChips extends StatelessWidget {
  final List<String> genres;

  const _GenreChips({required this.genres});

  @override
  Widget build(BuildContext context) {
    return Wrap(
      spacing: 4,
      runSpacing: 4,
      children:
          genres
              .map(
                (genre) => Chip(
                  label: Text(genre, style: const TextStyle(fontSize: 12)),
                  materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                  padding: const EdgeInsets.all(5),
                  labelPadding: const EdgeInsets.symmetric(
                    horizontal: 8,
                    vertical: -2,
                  ),
                  backgroundColor: AppTheme.primaryColor,
                  side: BorderSide(color: AppTheme.accentColor, width: 1),
                ),
              )
              .toList(),
    );
  }
}

class _MovieMetaInfo extends StatelessWidget {
  final int duration;
  final double price;

  const _MovieMetaInfo({required this.duration, required this.price});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        if (duration > 0)
          _MetaItem(icon: Icons.access_time, text: '$duration мин'),
        if (duration > 0 && price > 0) const SizedBox(width: 16),
        if (price > 0)
          _MetaItem(
            icon: Icons.local_offer,
            text: '${price.toStringAsFixed(0)} Br',
          ),
      ],
    );
  }
}

class _MetaItem extends StatelessWidget {
  final IconData icon;
  final String text;

  const _MetaItem({required this.icon, required this.text});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Icon(icon, size: 16, color: Colors.grey),
        const SizedBox(width: 4),
        Text(text, style: TextStyle(color: Colors.grey.shade700, fontSize: 14)),
      ],
    );
  }
}
