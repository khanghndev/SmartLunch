class LoginData {
  final String userId;
  final String username;
  final String email;
  final String accessToken;
  final String refreshToken;
  final DateTime refreshTokenExpiresAt;

  LoginData({
    required this.userId,
    required this.username,
    required this.email,
    required this.accessToken,
    required this.refreshToken,
    required this.refreshTokenExpiresAt,
  });

  factory LoginData.fromJson(Map<String, dynamic> json) {
    return LoginData(
      userId: json['userId']?.toString() ?? '',
      username: json['username']?.toString() ?? '',
      email: json['email']?.toString() ?? '',
      accessToken: json['accessToken']?.toString() ?? '',
      refreshToken: json['refreshToken']?.toString() ?? '',
      refreshTokenExpiresAt: DateTime.tryParse(
            json['refreshTokenExpiresAt']?.toString() ?? '',
          ) ??
          DateTime.now(),
    );
  }
}

class RefreshTokenData {
  final String accessToken;
  final String refreshToken;
  final DateTime refreshTokenExpiresAt;

  RefreshTokenData({
    required this.accessToken,
    required this.refreshToken,
    required this.refreshTokenExpiresAt,
  });

  factory RefreshTokenData.fromJson(Map<String, dynamic> json) {
    return RefreshTokenData(
      accessToken: json['accessToken']?.toString() ?? '',
      refreshToken: json['refreshToken']?.toString() ?? '',
      refreshTokenExpiresAt: DateTime.tryParse(
            json['refreshTokenExpiresAt']?.toString() ?? '',
          ) ??
          DateTime.now(),
    );
  }
}

class RegisterData {
  final String username;
  final String email;

  RegisterData({required this.username, required this.email});

  factory RegisterData.fromJson(Map<String, dynamic> json) {
    return RegisterData(
      username: json['username']?.toString() ?? '',
      email: json['email']?.toString() ?? '',
    );
  }
}

class AuthSession {
  final String userId;
  final String username;
  final String email;
  final String accessToken;
  final String refreshToken;
  final DateTime refreshTokenExpiresAt;
  final List<String> roles;

  AuthSession({
    required this.userId,
    required this.username,
    required this.email,
    required this.accessToken,
    required this.refreshToken,
    required this.refreshTokenExpiresAt,
    required this.roles,
  });

  AuthSession copyWith({
    String? accessToken,
    String? refreshToken,
    DateTime? refreshTokenExpiresAt,
    List<String>? roles,
  }) {
    return AuthSession(
      userId: userId,
      username: username,
      email: email,
      accessToken: accessToken ?? this.accessToken,
      refreshToken: refreshToken ?? this.refreshToken,
      refreshTokenExpiresAt:
          refreshTokenExpiresAt ?? this.refreshTokenExpiresAt,
      roles: roles ?? this.roles,
    );
  }
}
