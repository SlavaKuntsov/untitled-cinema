class AccessTokenModel {
  final String accessToken;

  AccessTokenModel({required this.accessToken});

  factory AccessTokenModel.fromJson(Map<String, dynamic> json) {
    return AccessTokenModel(accessToken: json['accessToken'] as String);
  }
}
