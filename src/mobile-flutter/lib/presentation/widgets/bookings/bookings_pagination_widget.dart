import 'package:flutter/material.dart';

import '../../providers/booking_provider.dart';

class BookingsPaginationWidget extends StatelessWidget {
  final BookingState bookingsState;
  final int selectedPageSize;
  final List<int> pageSizes;
  final Function(int?) onPageSizeChanged;
  final Function() onNextPage;
  final Function() onPrevPage;

  const BookingsPaginationWidget({
    super.key,
    required this.bookingsState,
    required this.selectedPageSize,
    required this.pageSizes,
    required this.onPageSizeChanged,
    required this.onNextPage,
    required this.onPrevPage,
  });

  @override
  Widget build(BuildContext context) {
    bool showNavigation =
        bookingsState.bookings.isNotEmpty &&
        (bookingsState.total > selectedPageSize ||
            bookingsState.currentPage > 0);

    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(vertical: 8),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              if (bookingsState.bookings.isNotEmpty)
                Text(
                  'Стр. ${bookingsState.currentPage} из ${(bookingsState.total / selectedPageSize).ceil()}',
                  style: const TextStyle(fontWeight: FontWeight.bold),
                )
              else
                const SizedBox(),
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
        if (showNavigation)
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 16),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                if (bookingsState.prevRef.isNotEmpty)
                  IntrinsicWidth(
                    child: ElevatedButton(
                      onPressed: () => onPrevPage(),
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
                if (bookingsState.nextRef.isNotEmpty)
                  // Icon(Icons.arrow_forward, size: 16)
                  IntrinsicWidth(
                    child: ElevatedButton(
                      onPressed: () => onNextPage(),
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
              ],
            ),
          ),
      ],
    );
  }
}
