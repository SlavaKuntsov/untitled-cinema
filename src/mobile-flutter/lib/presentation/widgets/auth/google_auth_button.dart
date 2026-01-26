import 'package:flutter/material.dart';

class GoogleAuthButton extends StatelessWidget {
  final VoidCallback onPressed;
  final bool isLoading;
  final String text;

  const GoogleAuthButton({
    super.key,
    required this.onPressed,
    this.isLoading = false,
    this.text = 'Войти через Google',
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: SizedBox(
        height: 56,
        width: double.infinity,
        child: ElevatedButton(
          onPressed: isLoading ? null : onPressed,
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.white,
            foregroundColor: Colors.black87,
            elevation: 1,
            shadowColor: Colors.black26,
            side: const BorderSide(color: Colors.black12, width: 1),
          ),
          child:
              isLoading
                  ? const CircularProgressIndicator.adaptive()
                  : Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Container(
                        height: 24,
                        width: 24,
                        decoration: BoxDecoration(
                          color: Colors.white,
                          shape: BoxShape.circle,
                          boxShadow: [
                            BoxShadow(
                              color: Colors.black.withOpacity(0.1),
                              blurRadius: 1,
                              spreadRadius: 1,
                            ),
                          ],
                        ),
                        child: Center(
                          child: Text(
                            'G',
                            style: TextStyle(
                              color: Colors.red[700],
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(width: 12),
                      Text(
                        text,
                        style: const TextStyle(
                          fontSize: 15,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ),
        ),
      ),
    );
  }
}
