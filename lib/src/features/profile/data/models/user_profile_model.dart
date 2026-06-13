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
  final String? address;

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
    this.address,
  });

  String get displayName =>
      fullName?.isNotEmpty == true ? fullName! : username;

  bool get isOrganizationRole =>
      roles.contains('Organization') || roles.contains('Company');

  bool get isManagerRole => roles.contains('Manager');

  bool get isShipperRole => roles.contains('Shipper');

  String get unitDisplayName {
    final name = unit?.name.trim() ?? '';
    return name.isNotEmpty ? name : '';
  }

  bool get hasPersonalName {
    if (fullName?.trim().isNotEmpty == true) return true;
    return firstName?.trim().isNotEmpty == true &&
        lastName?.trim().isNotEmpty == true;
  }

  bool get hasPhone => phoneNumber?.trim().isNotEmpty == true;

  bool get hasUnitCoreInfo {
    if (!isOrganizationRole) return true;
    return unitDisplayName.isNotEmpty;
  }

  bool get hasUnitContactInfo {
    if (!isOrganizationRole) return true;
    final u = unit;
    if (u == null) return false;
    return u.address?.trim().isNotEmpty == true ||
        u.phone?.trim().isNotEmpty == true ||
        u.contactEmail?.trim().isNotEmpty == true;
  }

  /// Hồ sơ đủ điều kiện hiển thị tích xanh xác minh.
  bool get isProfileComplete =>
      email.trim().isNotEmpty &&
      hasPersonalName &&
      hasPhone &&
      hasUnitCoreInfo &&
      hasUnitContactInfo &&
      (isOrganizationRole || birthDate != null);

  int get profileCompletionPercent {
    final checks = <bool>[
      hasPersonalName,
      hasPhone,
      email.trim().isNotEmpty,
      if (isOrganizationRole) ...[
        hasUnitCoreInfo,
        hasUnitContactInfo,
      ] else
        birthDate != null,
    ];
    if (checks.isEmpty) return 0;
    final filled = checks.where((c) => c).length;
    return ((filled / checks.length) * 100).round();
  }

  List<String> get missingProfileHints {
    final missing = <String>[];
    if (!hasPersonalName) missing.add('Họ tên');
    if (!hasPhone) missing.add('Số điện thoại');
    if (isOrganizationRole) {
      if (!hasUnitCoreInfo) missing.add('Tên đơn vị');
      if (!hasUnitContactInfo) missing.add('Địa chỉ hoặc liên hệ đơn vị');
    } else if (birthDate == null) {
      missing.add('Ngày sinh');
    }
    return missing;
  }

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
      address: json['address'] as String?,
    );
  }
}
