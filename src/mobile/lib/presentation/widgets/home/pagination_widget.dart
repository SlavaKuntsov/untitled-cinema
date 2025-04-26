import 'package:flutter/material.dart';

import '../../providers/movie_provider.dart';

class PaginationWidget extends StatelessWidget {
  final MoviesState moviesState;
  final int selectedPageSize;
  final List<int> pageSizes;
  final Function(int?) onPageSizeChanged;
  final VoidCallback onNextPage;
  final VoidCallback onPrevPage;

  const PaginationWidget({
    super.key,
    required this.moviesState,
    required this.selectedPageSize,
    required this.pageSizes,
    required this.onPageSizeChanged,
    required this.onNextPage,
    required this.onPrevPage,
  });

  @override
  Widget build(BuildContext context) {
    // Селектор количества элементов показываем всегда, даже если нет фильмов

    // Определяем, нужно ли показывать навигацию
    bool showNavigation =
        moviesState.movies.isNotEmpty &&
        (moviesState.total > selectedPageSize || moviesState.currentPage > 0);

    return Column(
      children: [
        // Первый уровень: всегда показываем селектор количества элементов
        Container(
          // color: Colors.red,
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: 8),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                // Индикатор страницы показываем только если есть фильмы
                if (moviesState.movies.isNotEmpty)
                  Text(
                    'Стр. ${moviesState.currentPage} из ${(moviesState.total / selectedPageSize).ceil()}',
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  )
                else
                  const SizedBox(), // Пустое место для выравнивания
                // Селектор размера страницы - показываем всегда
                Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Text('Показывать:', style: TextStyle(fontSize: 14)),
                    const SizedBox(width: 8),
                    DropdownButton<int>(
                      value: selectedPageSize,
                      isDense: true,
                      items:
                          pageSizes.map((size) {
                            return DropdownMenuItem<int>(
                              value: size,
                              child: Text('$size'),
                            );
                          }).toList(),
                      onChanged: onPageSizeChanged,
                    ),
                  ],
                ),
              ],
            ),
          ),
        ),
        // Второй уровень: показываем навигацию только если нужно
        if (showNavigation)
          Container(
            // color: Colors.green,
            child: Padding(
              padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 16),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  // Text('data'),
                  // Container(
                  //   color: Colors.blue,
                  //   child: IntrinsicWidth(
                  //     child: ElevatedButton(
                  //       onPressed: () {},
                  //       child: Text('Кнопка'),
                  //     ),
                  //   ),
                  // ),
                  // IntrinsicWidth(
                  //   child: ElevatedButton(
                  //     onPressed: () {},
                  //     child: Text('Кнопка'),
                  //   ),
                  // ),
                  // IntrinsicWidth(
                  //   child: ElevatedButton(
                  //     onPressed: () {},
                  //     child: Text('Кнопка2'),
                  //   ),
                  // ),
                  // Text('data3'),
                  // ElevatedButton(
                  //   onPressed: onPrevPage,
                  //   style: ElevatedButton.styleFrom(
                  //     maximumSize: Size(70, 30),
                  //     foregroundColor: Colors.black87,
                  //     backgroundColor: Colors.grey.shade200,
                  //   ),
                  //   child: const Row(
                  //     mainAxisSize: MainAxisSize.min,
                  //     children: [
                  //       Icon(Icons.arrow_back, size: 16),
                  //       SizedBox(width: 4),
                  //       Text('Пред.'),
                  //     ],
                  //   ),
                  // ),
                  if (moviesState.prevRef.isNotEmpty)
                    // Icon(Icons.arrow_back, size: 16)
                    IntrinsicWidth(
                      child: ElevatedButton(
                        onPressed: onPrevPage,
                        style: ElevatedButton.styleFrom(
                          foregroundColor: Colors.black87,
                          backgroundColor: Colors.grey.shade200,
                        ),
                        child: const Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(Icons.arrow_back, size: 18),
                            SizedBox(width: 14),
                            Text('Пред.'),
                          ],
                        ),
                      ),
                    )
                  else
                    const SizedBox(),
                  // // Icon(Icons.arrow_back, size: 16),
                  //
                  // // Используем Opacity для сохранения структуры
                  // // Opacity(
                  // //   opacity: 0.0,
                  // //   child: IgnorePointer(
                  // //     child: ElevatedButton(
                  // //       onPressed: () {}, // Добавляем пустую функцию
                  // //       style: ElevatedButton.styleFrom(
                  // //         foregroundColor: Colors.black87,
                  // //         backgroundColor: Colors.grey.shade200,
                  // //       ),
                  // //       child: const Row(
                  // //         mainAxisSize: MainAxisSize.min,
                  // //         children: [
                  // //           Icon(Icons.arrow_back, size: 16),
                  // //           SizedBox(width: 4),
                  // //           Text('Пред.'),
                  // //         ],
                  // //       ),
                  // //     ),
                  // //   ),
                  // // ),
                  // const SizedBox(width: 16),
                  //
                  if (moviesState.nextRef.isNotEmpty)
                    // Icon(Icons.arrow_forward, size: 16)
                    IntrinsicWidth(
                      child: ElevatedButton(
                        onPressed: onNextPage,
                        style: ElevatedButton.styleFrom(),
                        child: const Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Text('След.'),
                            SizedBox(width: 14),
                            Icon(
                              Icons.arrow_forward,
                              size: 18,
                              color: Colors.white,
                            ),
                          ],
                        ),
                      ),
                    )
                  else
                    const SizedBox(),
                  // // Используем Opacity для сохранения структуры
                  // // Opacity(
                  // //   opacity: 0.0,
                  // //   child: IgnorePointer(
                  // //     child: ElevatedButton(
                  // //       onPressed: () {}, // Добавляем пустую функцию
                  // //       style: ElevatedButton.styleFrom(),
                  // //       child: const Row(
                  // //         mainAxisSize: MainAxisSize.min,
                  // //         children: [
                  // //           Text('След.'),
                  // //           SizedBox(width: 4),
                  // //           Icon(Icons.arrow_forward, size: 16),
                  // //         ],
                  // //       ),
                  // //     ),
                  // //   ),
                  // // ),
                ],
              ),
            ),
          ),
      ],
    );
  }
}
