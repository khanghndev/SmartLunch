enum AppFlavor { all, customer, courier, org }

class AppConfig {
  final AppFlavor flavor;
  final String appName;

  const AppConfig({required this.flavor, required this.appName});
}
