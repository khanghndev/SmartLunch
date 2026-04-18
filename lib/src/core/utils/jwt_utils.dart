import 'dart:convert';

class JwtUtils {
  static Map<String, dynamic> decodePayload(String token) {
    final parts = token.split('.');
    if (parts.length != 3) {
      return {};
    }

    final normalized = base64Url.normalize(parts[1]);
    final payload = utf8.decode(base64Url.decode(normalized));
    final decoded = jsonDecode(payload);
    if (decoded is Map<String, dynamic>) {
      return decoded;
    }
    return {};
  }

  static List<String> extractRoles(String token) {
    final payload = decodePayload(token);
    final roleClaim =
        payload['role'] ??
        payload['roles'] ??
        payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    if (roleClaim == null) {
      return const [];
    }

    if (roleClaim is List) {
      return roleClaim.map((e) => e.toString()).toList();
    }

    return [roleClaim.toString()];
  }
}
