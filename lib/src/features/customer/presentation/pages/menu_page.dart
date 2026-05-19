import 'package:flutter/material.dart';
import '../../../../app/app_routes.dart';
import '../../../organization/data/org_repository.dart';
import '../../../organization/data/models/bulk_order_models.dart';

class MenuPage extends StatefulWidget {
  const MenuPage({super.key});

  @override
  State<MenuPage> createState() => _MenuPageState();
}

class _MenuPageState extends State<MenuPage> {
  bool _isLoadingCategories = true;
  String? _error;
  List<DishCategoryModel> _categories = [];
  
  int _selectedCategoryId = 0;
  bool _isLoadingDishes = false;
  List<Map<String, dynamic>> _dishes = [];

  @override
  void initState() {
    super.initState();
    _fetchCategories();
  }

  Future<void> _fetchCategories() async {
    try {
      setState(() {
        _isLoadingCategories = true;
        _error = null;
      });
      final data = await OrgRepository.instance.getDishCategories();
      if (mounted) {
        setState(() {
          _categories = data.categories;
          if (_categories.isNotEmpty) {
            _selectedCategoryId = _categories.first.id;
            _fetchDishes(_selectedCategoryId);
          }
          _isLoadingCategories = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _error = e.toString();
          _isLoadingCategories = false;
        });
      }
    }
  }

  Future<void> _fetchDishes(int categoryId) async {
    try {
      setState(() => _isLoadingDishes = true);
      final res = await OrgRepository.instance.getDishesByCategory(categoryId);
      if (mounted) {
        setState(() {
          _dishes = res.dishes
              .map(
                (d) => {
                  'id': d.id,
                  'name': d.name,
                  'price': d.price,
                  'imageUrl': d.imageUrl,
                },
              )
              .toList();
          _isLoadingDishes = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() => _isLoadingDishes = false);
        ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text('Lỗi tải món: $e')));
      }
    }
  }

  void _onCategorySelected(int id) {
    if (_selectedCategoryId == id) return;
    setState(() => _selectedCategoryId = id);
    _fetchDishes(id);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey.shade50,
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.menu),
          onPressed: () => Scaffold.of(context).openDrawer(),
        ),
        title: const Text('Thực đơn', style: TextStyle(fontWeight: FontWeight.bold)),
        backgroundColor: Colors.white,
        foregroundColor: Colors.black87,
        elevation: 0.5,
        centerTitle: true,
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoadingCategories) {
      return const Center(child: CircularProgressIndicator(color: Colors.blueAccent));
    }
    if (_error != null) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.error_outline, size: 48, color: Colors.red.shade300),
            const SizedBox(height: 16),
            Text('Lỗi: $_error', textAlign: TextAlign.center),
            ElevatedButton(onPressed: _fetchCategories, child: const Text('Thử lại')),
          ],
        ),
      );
    }
    if (_categories.isEmpty) {
      return const Center(child: Text('Không có danh mục món ăn nào.'));
    }

    return Column(
      children: [
        // Categories Header (Horizontal List)
        Container(
          height: 64,
          decoration: BoxDecoration(
            color: Colors.white,
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.05),
                blurRadius: 4,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: ListView.builder(
            scrollDirection: Axis.horizontal,
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            itemCount: _categories.length,
            itemBuilder: (context, index) {
              final cat = _categories[index];
              final isSelected = cat.id == _selectedCategoryId;
              return GestureDetector(
                onTap: () => _onCategorySelected(cat.id),
                child: AnimatedContainer(
                  duration: const Duration(milliseconds: 200),
                  margin: const EdgeInsets.only(right: 12),
                  padding: const EdgeInsets.symmetric(horizontal: 20),
                  decoration: BoxDecoration(
                    gradient: isSelected
                        ? const LinearGradient(colors: [Colors.blueAccent, Colors.lightBlue])
                        : null,
                    color: isSelected ? null : Colors.grey.shade100,
                    borderRadius: BorderRadius.circular(24),
                    boxShadow: isSelected
                        ? [
                            BoxShadow(
                              color: Colors.blueAccent.withValues(alpha: 0.3),
                              blurRadius: 8,
                              offset: const Offset(0, 4),
                            )
                          ]
                        : null,
                  ),
                  alignment: Alignment.center,
                  child: Text(
                    cat.name,
                    style: TextStyle(
                      color: isSelected ? Colors.white : Colors.black87,
                      fontWeight: isSelected ? FontWeight.bold : FontWeight.w500,
                    ),
                  ),
                ),
              );
            },
          ),
        ),
        
        // Dishes Grid
        Expanded(
          child: _isLoadingDishes
              ? const Center(child: CircularProgressIndicator(color: Colors.blueAccent))
              : _dishes.isEmpty
                  ? Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(Icons.restaurant_menu, size: 64, color: Colors.grey.shade300),
                          const SizedBox(height: 16),
                          Text(
                            'Chưa có món ăn trong danh mục này',
                            style: TextStyle(color: Colors.grey.shade600, fontSize: 16),
                          ),
                        ],
                      ),
                    )
                  : RefreshIndicator(
                      onRefresh: () => _fetchDishes(_selectedCategoryId),
                      color: Colors.blueAccent,
                      child: GridView.builder(
                        padding: const EdgeInsets.all(16),
                        physics: const AlwaysScrollableScrollPhysics(),
                        gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                          crossAxisCount: 2,
                          childAspectRatio: 0.75,
                          crossAxisSpacing: 16,
                          mainAxisSpacing: 16,
                        ),
                        itemCount: _dishes.length,
                        itemBuilder: (context, index) {
                          final dish = _dishes[index];
                          final id = dish['id'] ?? 0;
                          final name = dish['name'] ?? 'Món ăn';
                          final price = dish['price'] ?? 0;
                          final imageUrl = dish['imageUrl'];
                          
                          return GestureDetector(
                            onTap: () {
                              Navigator.of(context).pushNamed(
                                AppRoutes.customerMealDetail,
                                arguments: id,
                              );
                            },
                            child: Container(
                              decoration: BoxDecoration(
                                color: Colors.white,
                                borderRadius: BorderRadius.circular(20),
                                boxShadow: [
                                  BoxShadow(
                                    color: Colors.black.withValues(alpha: 0.05),
                                    blurRadius: 10,
                                    offset: const Offset(0, 4),
                                  ),
                                ],
                              ),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Expanded(
                                    child: Container(
                                      decoration: BoxDecoration(
                                        color: Colors.grey.shade200,
                                        borderRadius: const BorderRadius.vertical(top: Radius.circular(20)),
                                        image: imageUrl != null && imageUrl.toString().isNotEmpty
                                            ? DecorationImage(
                                                image: NetworkImage(imageUrl),
                                                fit: BoxFit.cover,
                                              )
                                            : null,
                                      ),
                                      child: imageUrl == null || imageUrl.toString().isEmpty
                                          ? const Center(child: Icon(Icons.fastfood, color: Colors.grey, size: 48))
                                          : null,
                                    ),
                                  ),
                                  Padding(
                                    padding: const EdgeInsets.all(12.0),
                                    child: Column(
                                      crossAxisAlignment: CrossAxisAlignment.start,
                                      children: [
                                        Text(
                                          name,
                                          maxLines: 2,
                                          overflow: TextOverflow.ellipsis,
                                          style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
                                        ),
                                        const SizedBox(height: 8),
                                        Row(
                                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                          children: [
                                            Text(
                                              '${price}đ',
                                              style: const TextStyle(
                                                color: Colors.blueAccent,
                                                fontWeight: FontWeight.bold,
                                                fontSize: 14,
                                              ),
                                            ),
                                            Container(
                                              padding: const EdgeInsets.all(4),
                                              decoration: BoxDecoration(
                                                color: Colors.blue.shade50,
                                                borderRadius: BorderRadius.circular(8),
                                              ),
                                              child: const Icon(Icons.add, size: 16, color: Colors.blueAccent),
                                            ),
                                          ],
                                        ),
                                      ],
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          );
                        },
                      ),
                    ),
        ),
      ],
    );
  }
}
