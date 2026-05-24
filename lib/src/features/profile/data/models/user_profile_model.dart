class UserUnitModel {
  final int id;
  final String name;
  final String? address;
  final String? phone;
  final String? contactEmail;
  final String? legalRepresentative;

  const UserUnitModel({
    required this.id,
    required this.name,
    this.address,
    this.phone,
    this.contactEmail,
    this.legalRepresentative,
  });

  factory UserUnitModel.fromJson(Map<String, dynamic>? json) {
    if (json == null) return const UserUnitModel(id: 0, name: '');
    return UserUnitModel(
      id: json['id'] as int? ?? 0,
      name: json['name']?.toString() ?? json['organizationName']?.toString() ?? '',
      address: json['address']?.toString(),
      phone: json['phone']?.toString(),
      contactEmail: json['contactEmail']?.toString(),
      legalRepresentative: json['legalRepresentative']?.toString(),
    );
  }
}

class UserProfileModel {
  final int id;
  final String username;
  final String email;
  final String? firstName;
  final String? lastName;
  final String? fullName;
  final String? phoneNumber;
  final String? avatarUrl;
  final bool isEmailVerified;
  final DateTime? birthDate;
  final List<String> roles;
  final UserUnitModel? unit;

  UserProfileModel({
    required this.id,
    required this.username,
    required this.email,
    this.firstName,
    this.lastName,
    this.fullName,
    this.phoneNumber,
    this.avatarUrl,
    required this.isEmailVerified,
    this.birthDate,
    required this.roles,
    this.unit,
  });

  String get displayName =>
      fullName?.isNotEmpty == true ? fullName! : username;

  factory UserProfileModel.fromJson(Map<String, dynamic> json) {
    return UserProfileModel(
      id: json['id'] as int? ?? 0,
      username: json['username'] as String? ?? '',
      email: json['email'] as String? ?? '',
      firstName: json['firstName'] as String?,
      lastName: json['lastName'] as String?,
      fullName: json['fullName'] as String?,
      phoneNumber: json['phoneNumber'] as String?,
      avatarUrl: json['avatarUrl'] as String?,
      isEmailVerified: json['isEmailVerified'] as bool? ?? false,
      birthDate: json['birthDate'] != null
          ? DateTime.tryParse(json['birthDate'] as String)
          : null,
      roles: (json['roles'] as List<dynamic>?)
              ?.map((e) => e.toString())
              .toList() ??
          [],
      unit: json['unit'] is Map<String, dynamic>
          ? UserUnitModel.fromJson(json['unit'] as Map<String, dynamic>)
          : null,
    );
  }
}
