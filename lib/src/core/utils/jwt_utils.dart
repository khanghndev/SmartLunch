import 'dart:convert';

class JwtUtils {
  static Map<String, dynamic> decodePayload(String token) {
    try {
      final parts = token.split('.');
      if (parts.length != 3) {
        throw Exception('Invalid token');
      }

      final payload = parts[1];
      final String normalized = base64Url.normalize(payload);
      final String resp = utf8.decode(base64Url.decode(normalized));
      return json.decode(resp);
    } catch (e) {
      return {};
    }
  }

  static List<String> extractRoles(String token) {
    final payload = decodePayload(token);
    
    // Check standard .NET role claim
    const netRoleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    var rolesData = payload[netRoleClaim] ?? payload['role'] ?? payload['roles'];
    
    if (rolesData == null) {
      return [];
    }

    if (rolesData is String) {
      return [rolesData];
    } else if (rolesData is List) {
      return rolesData.map((e) => e.toString()).toList();
    }
    
    return [];
  }
}
