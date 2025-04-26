import '../entities/movie/movie.dart';
import '../entities/paginated_items.dart';

abstract class MovieRepository {
  Future<PaginatedItems<Movie>> getMovies({
    int limit = 10,
    int offset = 0,
    String? sortBy,
    String? sortDirection,
    DateTime? date,
    List<String>? filters,
    List<String>? filterValues,
  });

  Future<Movie> getMovieById(String id);

  Future<String> getMoviePosterUrl(String id);
}
