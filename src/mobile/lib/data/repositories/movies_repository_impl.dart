import '../../domain/entities/movie/movie.dart';
import '../../domain/entities/paginated_items.dart';
import '../../domain/repositories/movies_repository.dart';
import '../datasources/movies_remote_data_source.dart';

class MovieRepositoryImpl implements MovieRepository {
  final MovieRemoteDataSource remoteDataSource;

  MovieRepositoryImpl({required this.remoteDataSource});

  @override
  Future<PaginatedItems<Movie>> getMovies({
    int limit = 10,
    int offset = 0,
    String? sortBy,
    String? sortDirection,
    DateTime? date,
    List<String>? filters,
    List<String>? filterValues,
  }) async {
    // Получаем данные из data source и возвращаем как есть,
    // поскольку MovieModel уже является наследником Movie
    return await remoteDataSource.getMovies(
      limit: limit,
      offset: offset,
      sortBy: sortBy,
      sortDirection: sortDirection,
      date: date,
      filters: filters,
      filterValues: filterValues,
    );
  }

  @override
  Future<Movie> getMovieById(String id) async {
    // Получаем данные из data source и возвращаем как есть,
    // поскольку MovieModel уже является наследником Movie
    return await remoteDataSource.getMovieById(id);
  }

  @override
  Future<String> getMoviePosterUrl(String id) async {
    return await remoteDataSource.getMoviePosterUrl(id);
  }

  @override
  Future<List<String>> getMovieFrames(String id) async {
    return await remoteDataSource.getMovieFrames(id);
  }
}
