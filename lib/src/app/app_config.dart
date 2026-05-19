/// Flavor ứng dụng — tương ứng nhóm người dùng / entry trong `UI_MOB.md` (Manager, Shipper, Organization, Customer).
enum AppFlavor {
  /// Bản gộp: splash → chọn vai trò (welcome).
  all,

  /// Khách cá nhân — thực đơn tuần.
  customer,

  shipper,

  organization,

  manager,
}

class AppConfig {
  final AppFlavor flavor;
  final String appName;

  const AppConfig({required this.flavor, required this.appName});
}
