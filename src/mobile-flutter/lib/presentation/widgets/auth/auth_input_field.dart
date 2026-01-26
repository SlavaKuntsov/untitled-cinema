import 'package:flutter/material.dart';

class AuthInputField extends StatefulWidget {
  final String label;
  final String? hintText;
  final bool isPassword;
  final TextEditingController controller;
  final String? Function(String?)? validator;
  final TextInputType keyboardType;
  final TextInputAction textInputAction;
  final Widget? prefixIcon;
  final FocusNode? focusNode;
  final VoidCallback? onEditingComplete;
  final Function(String)? onChanged;
  final EdgeInsets padding;

  const AuthInputField({
    super.key,
    required this.label,
    this.hintText,
    this.isPassword = false,
    required this.controller,
    this.validator,
    this.keyboardType = TextInputType.text,
    this.textInputAction = TextInputAction.next,
    this.prefixIcon,
    this.focusNode,
    this.onEditingComplete,
    this.onChanged,
    this.padding = const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
  });

  @override
  State<AuthInputField> createState() => _AuthInputFieldState();
}

class _AuthInputFieldState extends State<AuthInputField> {
  bool _obscureText = true;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: widget.padding,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            widget.label,
            style: TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w500,
              color: Theme.of(context).textTheme.bodyLarge?.color,
            ),
          ),
          const SizedBox(height: 8),
          TextFormField(
            controller: widget.controller,
            obscureText: widget.isPassword && _obscureText,
            validator: widget.validator,
            keyboardType: widget.keyboardType,
            textInputAction: widget.textInputAction,
            focusNode: widget.focusNode,
            onEditingComplete: widget.onEditingComplete,
            onChanged: widget.onChanged,
            decoration: InputDecoration(
              hintText: widget.hintText,
              prefixIcon: widget.prefixIcon,
              suffixIcon:
                  widget.isPassword
                      ? IconButton(
                        icon: Icon(
                          _obscureText
                              ? Icons.visibility_off_outlined
                              : Icons.visibility_outlined,
                          color: Theme.of(context).hintColor,
                        ),
                        onPressed: () {
                          setState(() {
                            _obscureText = !_obscureText;
                          });
                        },
                      )
                      : null,
            ),
          ),
        ],
      ),
    );
  }
}
