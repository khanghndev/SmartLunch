# Entities Without CRUD Controller

Generated from `SmartLunchDBContext` vs existing API controllers.

## Summary (all covered)

| # | Entity        | DbSet in Context | Has Controller | Note                    |
|---|---------------|------------------|----------------|-------------------------|
| 1 | **UserRole**  | UserRoles        | ✅ UserRoleController | Junction (User ↔ Role)  |
| 2 | **UserPermission** | UserPermissions | ✅ UserPermissionController | Junction (User ↔ Permission) |
| 3 | **RolePermission** | RolePermissions | ✅ RolePermissionController | Junction (Role ↔ Permission) |
| 4 | **UserUnit**  | UserUnits        | ✅ UserUnitController | Junction (User ↔ Unit)  |
| 5 | **DishIngredient** | DishIngredients | ✅ DishIngredientController | Junction (Dish ↔ Ingredient) |

---

## Entities that HAVE a CRUD controller (for reference)

- User → `UserController`
- Role → `RoleController`
- Permission → `PermissionController`
- UserToken → `UserTokenController`
- MediaFile → `MediaFileController`
- Unit → `UnitController`
- Partner → `PartnerController`
- Contract → `ContractController`
- PartnerPayment → `PartnerPaymentController`
- Ingredient → `IngredientController`
- IngredientSource → `IngredientSourceController`
- Inventory → `InventoryController`
- Dish → `DishController`
- WeeklyMenu → `WeeklyMenuController`
- MenuSchedule → `MenuScheduleController`
- Order → `OrderController`
- OrderItem → `OrderItemController`
- Delivery → `DeliveryController`
- Payment → `PaymentController`
- Transaction → `TransactionController`
- Review → `ReviewController`
- Sentiment → `SentimentController`
- Complaint → `ComplaintController`
- ChatbotLog → `ChatbotLogController`
- MenuSuggestion → `MenuSuggestionController`

*(AuthController and MediaController are for auth and media upload, not entity CRUD.)*

---

## New CRUD endpoints (added)

- **UserRole**: `GET/POST/PUT/DELETE` → `api/v1/master-data/UserRole` (filter by `userId`, `roleId`, `isActive`)
- **UserPermission**: `GET/POST/PUT/DELETE` → `api/v1/master-data/UserPermission` (filter by `userId`, `permissionId`, `isActive`)
- **RolePermission**: `GET/POST/PUT/DELETE` → `api/v1/master-data/RolePermission` (filter by `roleId`, `permissionId`, `isActive`)
- **UserUnit**: `GET/POST/PUT/DELETE` → `api/v1/master-data/UserUnit` (filter by `userId`, `unitId`, `isActive`)
- **DishIngredient**: `GET/POST/PUT/DELETE` → `api/v1/master-data/DishIngredient` (filter by `dishId`, `ingredientId`)

All controllers use `[Authorize(Policy = "roles:Admin")]` and existing permission policies (`permission:roles.read/update`, `permission:permissions.read/update`, `permission:units.read/update`, `permission:dishes.read/update`).
