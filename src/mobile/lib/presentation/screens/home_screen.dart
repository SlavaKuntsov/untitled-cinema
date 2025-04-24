import 'package:flutter/material.dart';

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

  // Имитация списка фильмов (в реальном приложении данные будут приходить с API)
  final List<Map<String, dynamic>> movies = [
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
    {
      'title': 'Тихое место 3',
      'genre': 'Ужасы',
      'rating': 4.3,
      'imageUrl': 'assets/images/quiet_place.jpg',
      'showTimes': ['11:00', '14:15', '17:30', '20:45'],
    },
    {
      'title': 'Дэдпул и Росомаха',
      'genre': 'Комедии',
      'rating': 4.9,
      'imageUrl': 'assets/images/deadpool.jpg',
      'showTimes': ['12:30', '15:45', '19:00', '22:15'],
    },
    {
      'title': 'Бегущий по лезвию 2099',
      'genre': 'Фантастика',
      'rating': 4.4,
      'imageUrl': 'assets/images/blade_runner.jpg',
      'showTimes': ['11:45', '15:00', '18:15', '21:30'],
    },
    {
      'title': 'Миссия невыполнима 8',
      'genre': 'Триллеры',
      'rating': 4.6,
      'imageUrl': 'assets/images/mission_impossible.jpg',
      'showTimes': ['10:15', '13:00', '16:15', '19:30'],
    },
  ];

  @override
  Widget build(BuildContext context) {
    // Фильтруем фильмы по выбранному жанру
    final filteredMovies =
        selectedGenre == 'Все'
            ? movies
            : movies.where((movie) => movie['genre'] == selectedGenre).toList();

    return SingleChildScrollView(
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
                    child: Image.network(
                      'https://via.placeholder.com/800x400',
                      fit: BoxFit.cover,
                    ),
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
                        onPressed: () {
                          // Действие при нажатии на кнопку "Купить билет"
                        },
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
                            setState(() {
                              selectedGenre = genre;
                            });
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
                    itemCount: 4, // Возьмем первые 4 фильма для популярных
                    itemBuilder: (context, index) {
                      final movie = movies[index];

                      return GestureDetector(
                        onTap: () {
                          // Переход на страницу с деталями фильма
                        },
                        child: Container(
                          width: 140,
                          margin: const EdgeInsets.only(right: 12),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              // Обложка фильма
                              Container(
                                height: 160,
                                decoration: BoxDecoration(
                                  borderRadius: BorderRadius.circular(8),
                                  color: Colors.grey.shade300,
                                  image: DecorationImage(
                                    fit: BoxFit.cover,
                                    image: NetworkImage(
                                      'https://via.placeholder.com/300x450?text=${Uri.encodeComponent(movie['title'])}',
                                    ),
                                  ),
                                ),
                              ),
                              const SizedBox(height: 8),
                              // Название фильма
                              Text(
                                movie['title'],
                                maxLines: 2,
                                overflow: TextOverflow.ellipsis,
                                style: const TextStyle(
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              // Рейтинг
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

          // Список всех фильмов
          // Padding(
          //   padding: const EdgeInsets.all(16),
          //   child: Column(
          //     crossAxisAlignment: CrossAxisAlignment.start,
          //     children: [
          //       Row(
          //         mainAxisAlignment: MainAxisAlignment.spaceBetween,
          //         children: [
          //           Text(
          //             selectedGenre == 'Все' ? 'Все фильмы' : selectedGenre,
          //             style: const TextStyle(
          //               fontSize: 18,
          //               fontWeight: FontWeight.bold,
          //             ),
          //           ),
          //           Text(
          //             '${filteredMovies.length} фильмов',
          //             style: TextStyle(color: Colors.grey.shade600),
          //           ),
          //         ],
          //       ),
          //       const SizedBox(height: 16),
          //       ListView.builder(
          //         shrinkWrap: true,
          //         physics: const NeverScrollableScrollPhysics(),
          //         itemCount: filteredMovies.length,
          //         itemBuilder: (context, index) {
          //           final movie = filteredMovies[index];
          //
          //           return Card(
          //             margin: const EdgeInsets.only(bottom: 16),
          //             elevation: 2,
          //             shape: RoundedRectangleBorder(
          //               borderRadius: BorderRadius.circular(12),
          //             ),
          //             child: InkWell(
          //               onTap: () {
          //                 // Переход на страницу с деталями фильма
          //               },
          //               borderRadius: BorderRadius.circular(12),
          //               child: Row(
          //                 crossAxisAlignment: CrossAxisAlignment.start,
          //                 children: [
          //                   // Обложка фильма
          //                   ClipRRect(
          //                     borderRadius: const BorderRadius.only(
          //                       topLeft: Radius.circular(12),
          //                       bottomLeft: Radius.circular(12),
          //                     ),
          //                     child: Image.network(
          //                       'https://via.placeholder.com/300x450?text=${Uri.encodeComponent(movie['title'])}',
          //                       width: 100,
          //                       height: 150,
          //                       fit: BoxFit.cover,
          //                     ),
          //                   ),
          //                   // Информация о фильме
          //                   Expanded(
          //                     child: Padding(
          //                       padding: const EdgeInsets.all(12),
          //                       child: Column(
          //                         crossAxisAlignment: CrossAxisAlignment.start,
          //                         children: [
          //                           // Название и рейтинг
          //                           Row(
          //                             mainAxisAlignment:
          //                                 MainAxisAlignment.spaceBetween,
          //                             children: [
          //                               Expanded(
          //                                 child: Text(
          //                                   movie['title'],
          //                                   style: const TextStyle(
          //                                     fontWeight: FontWeight.bold,
          //                                     fontSize: 16,
          //                                   ),
          //                                 ),
          //                               ),
          //                               const SizedBox(width: 8),
          //                               Row(
          //                                 children: [
          //                                   Icon(
          //                                     Icons.star,
          //                                     size: 18,
          //                                     color: Colors.amber.shade700,
          //                                   ),
          //                                   const SizedBox(width: 4),
          //                                   Text(
          //                                     '${movie['rating']}',
          //                                     style: const TextStyle(
          //                                       fontWeight: FontWeight.bold,
          //                                     ),
          //                                   ),
          //                                 ],
          //                               ),
          //                             ],
          //                           ),
          //                           const SizedBox(height: 8),
          //                           // Жанр
          //                           Text(
          //                             movie['genre'],
          //                             style: TextStyle(
          //                               color: Colors.grey.shade600,
          //                               fontSize: 14,
          //                             ),
          //                           ),
          //                           const SizedBox(height: 12),
          //                           // Расписание сеансов
          //                           const Text(
          //                             'Сеансы сегодня:',
          //                             style: TextStyle(
          //                               fontWeight: FontWeight.bold,
          //                               fontSize: 14,
          //                             ),
          //                           ),
          //                           const SizedBox(height: 8),
          //                           // Время сеансов
          //                           Wrap(
          //                             spacing: 8,
          //                             runSpacing: 8,
          //                             children:
          //                                 (movie['showTimes'] as List<String>).map<
          //                                   Widget
          //                                 >((time) {
          //                                   return OutlinedButton(
          //                                     onPressed: () {
          //                                       // Действие при выборе сеанса
          //                                     },
          //                                     style: OutlinedButton.styleFrom(
          //                                       foregroundColor:
          //                                           Colors.blue.shade700,
          //                                       side: BorderSide(
          //                                         color: Colors.blue.shade700,
          //                                       ),
          //                                       padding:
          //                                           const EdgeInsets.symmetric(
          //                                             horizontal: 12,
          //                                             vertical: 4,
          //                                           ),
          //                                       minimumSize: Size.zero,
          //                                       tapTargetSize:
          //                                           MaterialTapTargetSize
          //                                               .shrinkWrap,
          //                                     ),
          //                                     child: Text(time),
          //                                   );
          //                                 }).toList(),
          //                           ),
          //                         ],
          //                       ),
          //                     ),
          //                   ),
          //                 ],
          //               ),
          //             ),
          //           );
          //         },
          //       ),
          //     ],
          //   ),
          // ),
        ],
      ),
    );
  }
}
