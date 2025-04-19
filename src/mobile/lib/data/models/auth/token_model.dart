import '../../../domain/entities/auth/token.dart';

class TokenModel extends Token {
  const TokenModel({required String accessToken, required String refreshToken})
    : super(accessToken: accessToken, refreshToken: refreshToken);

  factory TokenModel.fromJson(Map<String, dynamic> json) {
    return TokenModel(
      accessToken: json['access_token'],
      refreshToken: json['refresh_token'],
    );
  }

  Map<String, dynamic> toJson() {
    return {'access_token': accessToken, 'refresh_token': refreshToken};
  }

  factory TokenModel.fromEntity(Token token) {
    return TokenModel(
      accessToken: token.accessToken,
      refreshToken: token.refreshToken,
    );
  }
}
