class Hall {
  final String id;
  final String name;
  final int totalSeats;
  final List<List<int>> seatsArray;

  Hall({
    required this.id,
    required this.name,
    required this.totalSeats,
    required this.seatsArray,
  });

  factory Hall.fromJson(Map<String, dynamic> json) {
    return Hall(
      id: json['id'] as String,
      name: json['name'] as String,
      totalSeats: json['totalSeats'] as int,
      seatsArray:
          (json['seatsArray'] as List)
              .map((row) => (row as List).map((seat) => seat as int).toList())
              .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'totalSeats': totalSeats,
      'seatsArray': seatsArray,
    };
  }
}
