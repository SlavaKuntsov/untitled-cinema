import 'package:flutter/material.dart';

import '../../../domain/entities/bookings/bookings.dart';
import 'booking_list_item.dart';

class BookingListWidget extends StatelessWidget {
  final List<Booking> bookings;
  final Function(String) onCancelBooking;
  final Function(String) onPayBooking;

  const BookingListWidget({
    super.key,
    required this.bookings,
    required this.onCancelBooking,
    required this.onPayBooking,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children:
          bookings.map((booking) {
            return BookingListItem(
              booking: booking,
              onCancel: () => onCancelBooking(booking.id),
              onPay: () => onPayBooking(booking.id),
            );
          }).toList(),
    );
  }
}
