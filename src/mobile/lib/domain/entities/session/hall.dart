import 'package:equatable/equatable.dart';

class Hall extends Equatable {
  final String id;
  final String name;

  const Hall({required this.id, required this.name});

  @override
  List<Object?> get props => [id, name];
}
