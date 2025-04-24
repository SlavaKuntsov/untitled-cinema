import 'package:flutter/material.dart';

class PageWidget extends StatelessWidget {
  final String title;
  final Color color;

  const PageWidget({Key? key, required this.title, required this.color})
    : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      color: color.withOpacity(0.2),
      child: Center(
        child: Text(
          title,
          style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
